using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class Parser : ParserBase<Token>
    {
        public Parser(Definition definition)
            : base(definition)
        {
        }

        public Span[] Parse(IInputSource<InputChar> input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            var tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            return Parse(tokenSource, errors);
        }
        public NodeMatch<Token>[] Match(IInputSource<InputChar> input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            var tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            return Match(tokenSource, errors);
        }

        protected override bool BranchTipMatchesInputElement(NodeMatch<Token> branchTip, Token inputElement)
        {
            return (branchTip.Node is DefRefNode) &&
                (branchTip.Node as DefRefNode).DefRef == inputElement.Definition;
        }
    }

    public abstract class ParserBase<T>
        where T : IInputElement
    {
        public ParserBase(Definition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            _definition = definition;
        }

        protected Definition _definition;

        class ParseInfo
        {
            public NodeMatchStackPair<Token> SourcePair;
            public NodeMatch<Token> Source { get { return SourcePair.NodeMatch; } }
            public MatchStack<Token> SourceStack { get { return SourcePair.MatchStack; } }

            public NodeMatch<Token> EndCandidate;

            public List<NodeMatchStackPair<Token>> Branches;

            public IEnumerable<Node> GetExpectedNodes()
            {
                if (this.Source.Node.NextNodes.Count > 0)
                {
                    return this.Source.Node.NextNodes;
                }

                var stack = this.SourceStack;
                while (stack != null &&
                       stack.Node.NextNodes.Count < 1)
                {
                    stack = stack.Parent;
                }

                if (stack != null)
                {
                    return stack.Node.NextNodes;
                }

                return new Node[0];
            }
        }

        public Span[] Parse(IInputSource<Token> tokenSource, ICollection<Error> errors)
        {
            if (tokenSource == null) throw new ArgumentNullException("tokenSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var matchTreeLeaves = Match(tokenSource, errors);

            return MakeSpans(matchTreeLeaves);
        }
        public NodeMatch<Token>[] Match(IInputSource<Token> tokenSource, ICollection<Error> errors)
        {
            if (tokenSource == null) throw new ArgumentNullException("tokenSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var sources = new PriorityQueue<NodeMatchStackPair<Token>, int>(lowToHigh: true);
            var ends = new List<NodeMatch<Token>>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch<Token>(rootNode, NodeMatch<Token>.TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair<Token>>();

            var branches2 = new PriorityQueue<Tuple<NodeMatch<Token>, MatchStack<Token>, ParseInfo>, int>();

            sources.Enqueue(pair(root, null), -1);
//            Logger.WriteLine("Starting");

            while (sources.Count > 0)
            {

                var nextSources = new List<NodeMatchStackPair<Token>>();
                while (sources.Count > 0)
                {
                    var sourcepair = sources.Dequeue();

                    var info = GetParseInfoFromSource(sourcepair);

                    if (info.EndCandidate != null)
                    {
                        ends.Add(info.EndCandidate);
                    }

                    //get all tokens, starting at end of source's token
                    var tokenization = tokenSource.GetInputAtLocation(info.Source.Token.IndexOfNextTokenization);

                    //if we get any tokenization errors, process them and reject
                    if (tokenization.Errors.ContainsNonWarnings())
                    {
                        //reject branches with errors

                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, tokenization.Errors);
                        }

                        RejectEndCandidate(info, rejects, ends, tokenization.Errors);
                    }
                    else if (tokenization.EndOfInput)
                    {
                        var err = new ParserError<Token> {
                            ErrorType = ParserError<Token>.UnexpectedEndOfInput,
                            LastValidMatchingNode = info.Source.Node,
                            ExpectedNodes = info.GetExpectedNodes(),
                            Position = tokenization.EndOfInputPosition,
                        };
                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, err);
                        }
                    }
                    else // we have valid tokens
                    {
                        var offendingToken = tokenization.InputElements.First();

                        var err = new ParserError<Token> {
                            ErrorType = ParserError<Token>.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingToken.StartPosition,
                            OffendingInputElement = offendingToken,
                        };

                        RejectEndCandidate(info, rejects, ends, err);

                        foreach (var branch in info.Branches)
                        {
                            branches2.Enqueue(new Tuple<NodeMatch<Token>, MatchStack<Token>, ParseInfo>(
                                branch.NodeMatch,
                                branch.MatchStack,
                                info),
                                info.Source.Token.IndexOfNextTokenization);
                        }
                    }
                }

                while (branches2.Count > 0)
                {
                    var branchtuple = branches2.Dequeue();
                    var branchnm = branchtuple.Item1;
                    var branchstack = branchtuple.Item2;
                    var info = branchtuple.Item3;

                    var tokenization = tokenSource.GetInputAtLocation(info.Source.Token.IndexOfNextTokenization);

                    if (!tokenization.Errors.ContainsNonWarnings() &&
                        !tokenization.EndOfInput)
                    {
                        // we have valid tokens
                        var offendingToken = tokenization.InputElements.First();
                        var err = new ParserError<Token> {
                            ErrorType = ParserError<Token>.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingToken.StartPosition,
                            OffendingInputElement = offendingToken,
                        };

                        RejectEndCandidate(info, rejects, ends, err);

                        // try to match branch to tokens
                        bool matched = false;
                        foreach (var intoken in tokenization.InputElements)
                        {
                            if (BranchTipMatchesInputElement(branchnm, intoken))
                            {
                                var newNext = branchnm.CloneWithNewToken(intoken);
                                nextSources.Add(pair(newNext, branchstack));
                                matched = true;
                            }
                        }

                        ParserError<Token> err2 = null;
                        // if the branch didn't match, reject it with InvalidToken
                        // otherwise, reject it with null since it's a duplicate
                        if (!matched)
                        {
                            err2 = new ParserError<Token> {
                                ErrorType = ParserError<Token>.InvalidToken,
                                LastValidMatchingNode = info.Source.Node,
                                OffendingInputElement = offendingToken,
                                ExpectedNodes = info.Source.Node.NextNodes,
                                Position = offendingToken.StartPosition,
                            };
                        }

                        rejects.Add(branchnm, err2);
                    }
                }

                foreach (var next in nextSources)
                {
                    sources.Enqueue(next, next.NodeMatch.Token.IndexOfNextTokenization);
//                    Logger.WriteLine("Enqueuing source with next index {0}", next.NodeMatch.Token.IndexOfNextTokenization);
                }
            }

            if (ends.Count > 0)
            {
                foreach (var reject in rejects)
                {
                    StripReject(reject.NodeMatch);
                }
            }
            else
            {
                if (rejects.Count > 0)
                {
                    IEnumerable<Error> errorsToUse = null;
                    foreach (var reject in (rejects as IEnumerable<NodeMatchErrorPair<Token>>).Reverse())
                    {
                        if (reject.Errors != null && reject.Errors.Count > 0)
                        {
                            errorsToUse = reject.Errors;
                            break;
                        }
                    }

                    if (errorsToUse != null)
                    {
                        errors.AddRange(errorsToUse);
                    }
                    else
                    {
                        throw new InvalidOperationException("No errors among the rejects");
                    }
                }
                else
                {
                    // failed to start?
                    throw new NotImplementedException();
                }
            }

            return ends.ToArray();
        }

        void RejectEndCandidate(ParseInfo info, List<NodeMatchErrorPair<Token>> rejects, List<NodeMatch<Token>> ends, Error err)
        {
            if (info.EndCandidate != null)
            {
                ends.Remove(info.EndCandidate);
                rejects.Add(info.EndCandidate, err);
                info.EndCandidate = null;
            }
        }
        void RejectEndCandidate(ParseInfo info, List<NodeMatchErrorPair<Token>> rejects, List<NodeMatch<Token>> ends, ICollection<Error> errors)
        {
            if (info.EndCandidate != null)
            {
                ends.Remove(info.EndCandidate);
                rejects.Add(info.EndCandidate, errors);
                info.EndCandidate = null;
            }
        }

        ParseInfo GetParseInfoFromSource(NodeMatchStackPair<Token> source)
        {
            var info = new ParseInfo();
            info.SourcePair = source;

            var currents = new Queue<NodeMatchStackPair<Token>>();

            currents.Enqueue(info.SourcePair);

            // find all ends
            var enders = new List<NodeMatchStackPair<Token>>();
            if (info.Source.Transition != NodeMatch<Token>.TransitionType.Root)
            {
                var ender = info.SourcePair;

                while (ender.NodeMatch != null &&
                       ender.MatchStack != null &&
                       ender.NodeMatch.Node.IsEndNode)
                {
                    ender = ender.CreateEndDefMatch();
                    enders.Add(ender);
                }

                if (ender.NodeMatch != null &&
                    ender.MatchStack == null)
                {
                    info.EndCandidate = ender.NodeMatch;
                }
            }

            foreach (var ender in enders)
            {
                currents.Enqueue(ender);
            }

            //find all branches
            info.Branches = new List<NodeMatchStackPair<Token>>();
            while (currents.Count > 0)
            {
                var current = currents.Dequeue();
                var cur = current.NodeMatch;
                var curstack = current.MatchStack;

                if (IsBranchTip(cur) &&
                    cur != info.Source)
                {
                    info.Branches.Add(current);
                    continue;
                }

                if (cur.DefRef.IsTokenized ||
                    cur.Transition == NodeMatch<Token>.TransitionType.EndDef)
                {
                    foreach (var next in cur.Node.NextNodes)
                    {
                        var nm = new NodeMatch<Token>(next, NodeMatch<Token>.TransitionType.Follow, cur);
                        currents.Enqueue(pair(nm, curstack));
                    }
                }
                else
                {
                    var nextStack = new MatchStack<Token>(cur, curstack);
                    foreach (var start in (cur.Node as DefRefNode).DefRef.StartNodes)
                    {
                        var nm = new NodeMatch<Token>(start, NodeMatch<Token>.TransitionType.StartDef, cur);
                        currents.Enqueue(pair(nm, nextStack));
                    }
                }
            }

            return info;
        }

        protected virtual bool IsBranchTip(NodeMatch<Token> cur)
        {
            return cur.DefRef.IsTokenized;
        }
        protected abstract bool BranchTipMatchesInputElement(NodeMatch<Token> branchTip, Token inputElement);

        static Span[] MakeSpans(IEnumerable<NodeMatch<Token>> matchTreeLeaves)
        {
            var lists = new List<List<NodeMatch<Token>>>();
            foreach (var leaf in matchTreeLeaves)
            {
                var cur = leaf;
                var list = new List<NodeMatch<Token>>();

                while (cur != null)
                {
                    list.Add(cur);
                    cur = cur.Previous;
                }
                list.Reverse();

                lists.Add(list);
            }

            var spans = new List<Span>();
            foreach (var list in lists)
            {
                var stack = new Stack<Span>();

                Span rootSpan = null;

                foreach (var nm in list)
                {
                    if (nm.Transition == NodeMatch<Token>.TransitionType.EndDef)
                    {
                        rootSpan = stack.Pop();
                    }
                    else if (!nm.DefRef.IsTokenized)
                    {
                        var s = new Span();
                        s.Node = nm.Node;
                        if (stack.Count > 0)
                        {
                            stack.Peek().Subspans.Add(s);
                        }
                        stack.Push(s);
                    }
                    else
                    {
                        var s = new Span();
                        s.Node = nm.Node;
                        s.Value = nm.Token.Value;
                        stack.Peek().Subspans.Add(s);
                    }
                }

                spans.Add(rootSpan);
            }

            return spans.ToArray();
        }

        public static NodeMatchStackPair<Token> pair(NodeMatch<Token> nodeMatch, MatchStack<Token> matchStack)
        {
            return new NodeMatchStackPair<Token>{NodeMatch = nodeMatch, MatchStack = matchStack};
        }


        public static void StripReject(NodeMatch<Token> reject)
        {
            var cur = reject;
            var next = cur;
            while (cur != null &&
                   cur.Nexts.Count < 2)
            {
                next = cur;
                cur = cur.Previous;
            }
            if (cur != null && cur != next)
            {
                next.Previous = null;
            }
        }
    }
}

