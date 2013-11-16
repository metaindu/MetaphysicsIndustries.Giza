using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class Parser
    {
        public Parser(Definition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            _definition = definition;
        }

        Definition _definition;

        class ParseInfo
        {
            public NodeMatchStackPair SourcePair;
            public NodeMatch Source { get { return SourcePair.NodeMatch; } }
            public MatchStack SourceStack { get { return SourcePair.MatchStack; } }

            public NodeMatch EndCandidate;

            public List<NodeMatchStackPair> Branches;

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

        public Span[] Parse(CharacterSource input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            ITokenSource tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            return Parse(tokenSource, errors);
        }
        public Span[] Parse(ITokenSource tokenSource, ICollection<Error> errors)
        {
            if (tokenSource == null) throw new ArgumentNullException("tokenSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var sources = new PriorityQueue<NodeMatchStackPair, int>(lowToHigh: true);
            var ends = new List<NodeMatch>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch(rootNode, NodeMatch.TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair>();

            var branches2 = new PriorityQueue<Tuple<NodeMatch, MatchStack, ParseInfo>, int>();

            sources.Enqueue(pair(root, null), -1);
//            Logger.WriteLine("Starting");

            while (sources.Count > 0)
            {

                var nextSources = new List<NodeMatchStackPair>();
                while (sources.Count > 0)
                {
                    var sourcepair = sources.Dequeue();

                    var info = GetParseInfoFromSource(sourcepair);

                    if (info.EndCandidate != null)
                    {
                        ends.Add(info.EndCandidate);
                    }

                    //get all tokens, starting at end of source's token
                    var tokenization = tokenSource.GetTokensAtLocation(info.Source.Token.IndexOfNextTokenization);

                    //if we get any tokenization errors, process them and reject
                    if (tokenization.Errors.ContainsNonWarnings())
                    {
                        //reject branches with error
                        SpannerError se = (tokenization.Errors.GetFirstNonWarning() as SpannerError);
                        var err = new ParserError();
                        err.LastValidMatchingNode = info.Source.Node;

                        err.ExpectedNodes = info.GetExpectedNodes();

                        if (se.ErrorType == SpannerError.UnexpectedEndOfInput)
                        {
                            err.ErrorType = ParserError.UnexpectedEndOfInput;
                            err.Position = se.Position;
                        }
                        else if (se.ErrorType == SpannerError.ExcessRemainingInput)
                        {
                            // this shouldn't happen. when we read tokens,
                            // we set mustUseAllInput to false
                            throw new InvalidOperationException("Excess remaining input when reading tokens");
                        }
                        else if (se.ErrorType == SpannerError.InvalidCharacter)
                        {
                            err.ErrorType = ParserError.InvalidToken;
                            err.Position = se.Position;
                            err.OffendingToken.StartPosition = err.Position;
                            err.OffendingToken.Definition = null;
                            err.OffendingToken.Value = se.OffendingCharacter.ToString();
                        }
                        else
                        {
                            throw new InvalidOperationException("Errors in definitions");
                        }

                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, err);
                        }

                        RejectEndCandidate(info, rejects, ends, err);
                    }
                    else if (tokenization.EndOfInput)
                    {
                        var err = new ParserError {
                            ErrorType = ParserError.UnexpectedEndOfInput,
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
                        var offendingToken = tokenization.Tokens.First();

                        var err = new ParserError {
                            ErrorType = ParserError.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingToken.StartPosition,
                            OffendingToken = offendingToken,
                        };

                        RejectEndCandidate(info, rejects, ends, err);
                    }

                    foreach (var branch in info.Branches)
                    {
                        branches2.Enqueue(new Tuple<NodeMatch, MatchStack, ParseInfo>(
                                            branch.NodeMatch,
                                            branch.MatchStack,
                                            info),
                                          info.Source.Token.IndexOfNextTokenization);
                    }
                }

                while (branches2.Count > 0)
                {
                    var branchtuple = branches2.Dequeue();
                    var branchnm = branchtuple.Item1;
                    var branchstack = branchtuple.Item2;
                    var info = branchtuple.Item3;

                    var tokenization = tokenSource.GetTokensAtLocation(info.Source.Token.IndexOfNextTokenization);

                    if (!tokenization.Errors.ContainsNonWarnings() &&
                        !tokenization.EndOfInput)
                    {
                        // we have valid tokens
                        var offendingToken = tokenization.Tokens.First();
                        var err = new ParserError {
                            ErrorType = ParserError.ExcessRemainingInput,
                            LastValidMatchingNode = info.Source.Node,
                            Position = offendingToken.StartPosition,
                            OffendingToken = offendingToken,
                        };

                        RejectEndCandidate(info, rejects, ends, err);

                        // try to match branch to tokens
                        bool matched = false;
                        foreach (var intoken in tokenization.Tokens)
                        {
                            if ((branchnm.Node is DefRefNode) &&
                                (branchnm.Node as DefRefNode).DefRef == intoken.Definition)
                            {
                                var newNext = branchnm.CloneWithNewToken(intoken);
                                nextSources.Add(pair(newNext, branchstack));
                                matched = true;
                            }
                        }

                        ParserError err2 = null;
                        // if the branch didn't match, reject it with InvalidToken
                        // otherwise, reject it with null since it's a duplicate
                        if (!matched)
                        {
                            err2 = new ParserError {
                                ErrorType = ParserError.InvalidToken,
                                LastValidMatchingNode = info.Source.Node,
                                OffendingToken = offendingToken,
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
                    Error errorToUse = null;
                    foreach (var reject in (rejects as IEnumerable<NodeMatchErrorPair>).Reverse())
                    {
                        if (reject.Error != null)
                        {
                            errorToUse = reject.Error;
                            break;
                        }
                    }

                    if (errorToUse != null)
                    {
                        errors.Add(errorToUse);
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

            return MakeSpans(ends);
        }

        void RejectEndCandidate(ParseInfo info, List<NodeMatchErrorPair> rejects, List<NodeMatch> ends, Error err)
        {
            if (info.EndCandidate != null)
            {
                ends.Remove(info.EndCandidate);
                rejects.Add(info.EndCandidate, err);
                info.EndCandidate = null;
            }
        }

        ParseInfo GetParseInfoFromSource(NodeMatchStackPair source)
        {
            var info = new ParseInfo();
            info.SourcePair = source;

            var currents = new Queue<NodeMatchStackPair>();

            currents.Enqueue(info.SourcePair);

            // find all ends
            var enders = new List<NodeMatchStackPair>();
            if (info.Source.Transition != NodeMatch.TransitionType.Root)
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
            info.Branches = new List<NodeMatchStackPair>();
            while (currents.Count > 0)
            {
                NodeMatchStackPair current = currents.Dequeue();
                NodeMatch cur = current.NodeMatch;
                MatchStack curstack = current.MatchStack;

                if (cur.DefRef.IsTokenized &&
                    cur != info.Source)
                {
                    info.Branches.Add(current);
                    continue;
                }

                if (cur.DefRef.IsTokenized ||
                    cur.Transition == NodeMatch.TransitionType.EndDef)
                {
                    foreach (var next in cur.Node.NextNodes)
                    {
                        var nm = new NodeMatch(next, NodeMatch.TransitionType.Follow, cur);
                        currents.Enqueue(pair(nm, curstack));
                    }
                }
                else
                {
                    var nextStack = new MatchStack(cur, curstack);
                    foreach (var start in (cur.Node as DefRefNode).DefRef.StartNodes)
                    {
                        var nm = new NodeMatch(start, NodeMatch.TransitionType.StartDef, cur);
                        currents.Enqueue(pair(nm, nextStack));
                    }
                }
            }

            return info;
        }

        static Span[] MakeSpans(IEnumerable<NodeMatch> matchTreeLeaves)
        {
            var lists = new List<List<NodeMatch>>();
            foreach (NodeMatch leaf in matchTreeLeaves)
            {
                NodeMatch cur = leaf;
                var list = new List<NodeMatch>();

                while (cur != null)
                {
                    list.Add(cur);
                    cur = cur.Previous;
                }
                list.Reverse();

                lists.Add(list);
            }

            var spans = new List<Span>();
            foreach (List<NodeMatch> list in lists)
            {
                var stack = new Stack<Span>();

                Span rootSpan = null;

                foreach (NodeMatch nm in list)
                {
                    if (nm.Transition == NodeMatch.TransitionType.EndDef)
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

        public static NodeMatchStackPair pair(NodeMatch nodeMatch, MatchStack matchStack)
        {
            return new NodeMatchStackPair{NodeMatch = nodeMatch, MatchStack = matchStack};
        }


        public static void StripReject(NodeMatch reject)
        {
            NodeMatch cur = reject;
            NodeMatch next = cur;
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

