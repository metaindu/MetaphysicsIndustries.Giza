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

        protected override bool IsBranchTip(NodeMatch<Token> cur)
        {
            return cur.DefRef.IsTokenized;
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
            public NodeMatchStackPair<T> SourcePair;
            public NodeMatch<T> Source { get { return SourcePair.NodeMatch; } }
            public MatchStack<T> SourceStack { get { return SourcePair.MatchStack; } }

            public NodeMatch<T> EndCandidate;

            public List<NodeMatchStackPair<T>> Branches;

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

        public Span[] Parse(IInputSource<T> inputSource, ICollection<Error> errors)
        {
            if (inputSource == null) throw new ArgumentNullException("inputSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var matchTreeLeaves = Match(inputSource, errors);

            return MakeSpans(matchTreeLeaves);
        }
        public NodeMatch<T>[] Match(IInputSource<T> inputSource, ICollection<Error> errors)
        {
            if (inputSource == null) throw new ArgumentNullException("inputSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var sources = new PriorityQueue<NodeMatchStackPair<T> , int>(lowToHigh: true);
            var ends = new List<NodeMatch<T>>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch<T>(rootNode, TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair<T>>();

            var branches2 = new PriorityQueue<Tuple<NodeMatch<T> , MatchStack<T> , ParseInfo>, int>();

            sources.Enqueue(pair(root, null), -1);
//            Logger.WriteLine("Starting");

            while (sources.Count > 0)
            {

                var nextSources = new List<NodeMatchStackPair<T>>();
                while (sources.Count > 0)
                {
                    var sourcepair = sources.Dequeue();

                    var info = GetParseInfoFromSource(sourcepair);

                    if (info.EndCandidate != null)
                    {
                        ends.Add(info.EndCandidate);
                    }

                    //get all input elements and errors and end-of-input, starting at end of source's element
                    var inputElementSet = inputSource.GetInputAtLocation(info.Source.InputElement.IndexOfNextElement);

                    //if we get any errors, process them and reject
                    if (inputElementSet.Errors.ContainsNonWarnings())
                    {
                        //reject branches with errors

                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, inputElementSet.Errors);
                        }

                        RejectEndCandidate(info, rejects, ends, inputElementSet.Errors);
                    }
                    else if (inputElementSet.EndOfInput)
                    {
                        var err = new ParserError<T> {
                            ErrorType = ParserError.UnexpectedEndOfInput,
                            LastValidMatchingNode = info.Source.Node,
                            ExpectedNodes = info.GetExpectedNodes(),
                            Position = inputElementSet.EndOfInputPosition,
                        };
                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, err);
                        }
                    }
                    else // we have valid input elements
                    {
                        var offendingInputElement = inputElementSet.InputElements.First();

                        var err = new ParserError<T> {
                            ErrorType = ParserError.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingInputElement.Position,
                            OffendingInputElement = offendingInputElement,
                        };

                        RejectEndCandidate(info, rejects, ends, err);

                        foreach (var branch in info.Branches)
                        {
                            branches2.Enqueue(new Tuple<NodeMatch<T> , MatchStack<T> , ParseInfo>(
                                branch.NodeMatch,
                                branch.MatchStack,
                                info),
                                info.Source.InputElement.IndexOfNextElement);
                        }
                    }
                }

                while (branches2.Count > 0)
                {
                    var branchtuple = branches2.Dequeue();
                    var branchnm = branchtuple.Item1;
                    var branchstack = branchtuple.Item2;
                    var info = branchtuple.Item3;

                    var inputElementSet = inputSource.GetInputAtLocation(info.Source.InputElement.IndexOfNextElement);

                    if (!inputElementSet.Errors.ContainsNonWarnings() &&
                        !inputElementSet.EndOfInput)
                    {
                        // we have valid input elements
                        var offendingInputElement = inputElementSet.InputElements.First();
                        var err = new ParserError<T> {
                            ErrorType = ParserError.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingInputElement.Position,
                            OffendingInputElement = offendingInputElement,
                        };

                        RejectEndCandidate(info, rejects, ends, err);

                        // try to match branch to input elements
                        bool matched = false;
                        foreach (var intoken in inputElementSet.InputElements)
                        {
                            if (BranchTipMatchesInputElement(branchnm, intoken))
                            {
                                var newNext = branchnm.CloneWithNewInputElement(intoken);
                                nextSources.Add(pair(newNext, branchstack));
                                matched = true;
                            }
                        }

                        ParserError<T> err2 = null;
                        // if the branch didn't match, reject it with InvalidToken
                        // otherwise, reject it with null since it's a duplicate
                        if (!matched)
                        {
                            err2 = new ParserError<T> {
                                ErrorType = ParserError.InvalidToken,
                                LastValidMatchingNode = info.Source.Node,
                                OffendingInputElement = offendingInputElement,
                                ExpectedNodes = info.Source.Node.NextNodes,
                                Position = offendingInputElement.Position,
                            };
                        }

                        rejects.Add(branchnm, err2);
                    }
                }

                foreach (var next in nextSources)
                {
                    sources.Enqueue(next, next.NodeMatch.InputElement.IndexOfNextElement);
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
                    foreach (var reject in (rejects as IEnumerable<NodeMatchErrorPair<T>>).Reverse())
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

        void RejectEndCandidate(ParseInfo info, List<NodeMatchErrorPair<T>> rejects, List<NodeMatch<T>> ends, Error err)
        {
            if (info.EndCandidate != null)
            {
                ends.Remove(info.EndCandidate);
                rejects.Add(info.EndCandidate, err);
                info.EndCandidate = null;
            }
        }
        void RejectEndCandidate(ParseInfo info, List<NodeMatchErrorPair<T>> rejects, List<NodeMatch<T>> ends, ICollection<Error> errors)
        {
            if (info.EndCandidate != null)
            {
                ends.Remove(info.EndCandidate);
                rejects.Add(info.EndCandidate, errors);
                info.EndCandidate = null;
            }
        }

        ParseInfo GetParseInfoFromSource(NodeMatchStackPair<T> source)
        {
            var info = new ParseInfo();
            info.SourcePair = source;

            var currents = new Queue<NodeMatchStackPair<T>>();

            currents.Enqueue(info.SourcePair);

            // find all ends
            var enders = new List<NodeMatchStackPair<T>>();
            if (info.Source.Transition != TransitionType.Root)
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
            info.Branches = new List<NodeMatchStackPair<T>>();
            while (currents.Count > 0)
            {
                var current = currents.Dequeue();
                var cur = current.NodeMatch;
                var curstack = current.MatchStack;

                var isBranchTip = IsBranchTip(cur);

                if (isBranchTip &&
                    cur != info.Source)
                {
                    info.Branches.Add(current);
                    continue;
                }

                if (isBranchTip ||
                    cur.Transition == TransitionType.EndDef)
                {
                    foreach (var next in cur.Node.NextNodes)
                    {
                        var nm = new NodeMatch<T>(next, TransitionType.Follow, cur);
                        currents.Enqueue(pair(nm, curstack));
                    }
                }
                else
                {
                    var nextStack = new MatchStack<T>(cur, curstack);
                    foreach (var start in (cur.Node as DefRefNode).DefRef.StartNodes)
                    {
                        var nm = new NodeMatch<T>(start, TransitionType.StartDef, cur);
                        currents.Enqueue(pair(nm, nextStack));
                    }
                }
            }

            return info;
        }

        protected abstract bool IsBranchTip(NodeMatch<T> cur);
        protected abstract bool BranchTipMatchesInputElement(NodeMatch<T> branchTip, T inputElement);

        Span[] MakeSpans(IEnumerable<NodeMatch<T>> matchTreeLeaves)
        {
            var lists = new List<List<NodeMatch<T>>>();
            foreach (var leaf in matchTreeLeaves)
            {
                var cur = leaf;
                var list = new List<NodeMatch<T>>();

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
                    if (nm.Transition == TransitionType.EndDef)
                    {
                        rootSpan = stack.Pop();
                    }
                    else if (!IsBranchTip(nm))
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
                        s.Value = nm.InputElement.Value;
                        stack.Peek().Subspans.Add(s);
                    }
                }

                spans.Add(rootSpan);
            }

            return spans.ToArray();
        }

        public static NodeMatchStackPair<T> pair(NodeMatch<T> nodeMatch, MatchStack<T> matchStack)
        {
            return new NodeMatchStackPair<T> {NodeMatch = nodeMatch, MatchStack = matchStack};
        }


        public static void StripReject(NodeMatch<T> reject)
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

