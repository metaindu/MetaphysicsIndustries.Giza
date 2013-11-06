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

        struct ParseInfo
        {
            public NodeMatchStackPair SourcePair;
            public NodeMatch Source { get { return SourcePair.NodeMatch; } }
            public MatchStack SourceStack { get { return SourcePair.MatchStack; } }

            public List<NodeMatchStackPair> Enders;
            public bool LastEnderIsEndCandidate;

            public bool EndOfInput;
            public InputPosition EndOfInputPosition;
            public IEnumerable<Token> Tokens;
            public List<Error> TokenizationErrors;

            public List<NodeMatchStackPair> Branches;
        }

        public Span[] Parse(string input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            ITokenSource tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            var sources = new PriorityQueue<NodeMatchStackPair, int>(lowToHigh: true);
            var ends = new List<NodeMatch>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch(rootNode, NodeMatch.TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair>();

            sources.Enqueue(pair(root, null), -1);
//            Logger.WriteLine("Starting");

            while (sources.Count > 0)
            {

                while (sources.Count > 0)
                {
                    var nextSources = new List<NodeMatchStackPair>();

                    var info = new ParseInfo();
                    info.SourcePair = sources.Dequeue();
//                    Logger.WriteLine("Dequeuing source with next index {0}", info.Source.Token.IndexOfNextTokenization);

                    var currents = new Queue<NodeMatchStackPair>();

                    currents.Enqueue(info.SourcePair);

                    // find all ends
                    var ender = info.SourcePair;
                    info.Enders = new List<NodeMatchStackPair>();
                    while (true)
                    {
                        var nm = ender.NodeMatch;

                        if (nm == null)
                            break;
                        if (nm.Transition == NodeMatch.TransitionType.Root)
                            break;

                        if (ender.MatchStack == null)
                        {
                            info.LastEnderIsEndCandidate = true;
                            break;
                        }

                        if (nm.Node.IsEndNode)
                        {
                            ender = ender.CreateEndDefMatch();
                            currents.Enqueue(ender);
                            info.Enders.Add(ender);
                        }
                        else
                        {
                            break;
                        }
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

                    //get all tokens, starting at end of source's token
                    info.TokenizationErrors = new List<Error>();
                    info.Tokens = tokenSource.GetTokensAtLocation(
                        info.Source.Token.IndexOfNextTokenization,
                        info.TokenizationErrors,
                        out info.EndOfInput,
                        out info.EndOfInputPosition);

                    //if we get any tokenization errors, process them and reject
                    if (info.TokenizationErrors.ContainsNonWarnings())
                    {
                        //reject branches with error
                        SpannerError se = (info.TokenizationErrors.GetFirstNonWarning() as SpannerError);
                        var err = new ParserError();
                        err.LastValidMatchingNode = info.Source.Node;

                        err.ExpectedNodes = GetExpectedNodes(info);

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
                            err.OffendingToken.Value = input[err.Index].ToString();
                        }
                        else
                        {
                            throw new InvalidOperationException("Errors in definitions");
                        }

                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, err);
                        }

                        if (info.LastEnderIsEndCandidate)
                        {
                            rejects.Add(info.Enders.Last().NodeMatch, err);
                        }
                    }
                    else if (info.EndOfInput)
                    {
//                        int line;
//                        int column;
//
//                        Spanner.GetPosition(input, info.EndOfInputIndex, out line, out column);

                        var err = new ParserError {
                            ErrorType = ParserError.UnexpectedEndOfInput,
                            LastValidMatchingNode = info.Source.Node,
                            ExpectedNodes = GetExpectedNodes(info),
                            Position = info.EndOfInputPosition,
                        };
                        foreach (var branch in info.Branches)
                        {
                            rejects.Add(branch.NodeMatch, err);
                        }

                        if (info.LastEnderIsEndCandidate)
                        {
                            ends.Add(info.Enders.Last().NodeMatch);
                        }
                    }
                    else // we get valid tokens
                    {
                        if (info.LastEnderIsEndCandidate)
                        {
                            var offendingToken = info.Tokens.First();

                            var err = new ParserError {
                                ErrorType = ParserError.ExcessRemainingInput,
                                LastValidMatchingNode = info.Source.Node,
                                Position = offendingToken.StartPosition,
                                OffendingToken=offendingToken,
                            };
                            rejects.Add(info.Enders.Last().NodeMatch, err);
                        }

                        // try to match branches to tokens
                        var matchedBranches = new Set<NodeMatchStackPair>();
                        var unmatchedBranches = new Set<NodeMatchStackPair>();
                        unmatchedBranches.AddRange(info.Branches);
                        foreach (var branch in info.Branches)
                        {
                            var branchnm = branch.NodeMatch;
                            var branchstack = branch.MatchStack;

                            bool matched = false;
                            foreach (var intoken in info.Tokens)
                            {
                                if ((branchnm.Node is DefRefNode) &&
                                    (branchnm.Node as DefRefNode).DefRef == intoken.Definition)
                                {
                                    var newNext = branchnm.CloneWithNewToken(intoken);
                                    nextSources.Add(pair(newNext, branchstack));
                                    matched = true;
                                    matchedBranches.Add(branch);
                                    unmatchedBranches.Remove(branch);
                                }
                            }

                            ParserError err = null;
                            // if the branch didn't match, reject it with InvalidToken
                            // otherwise, reject it with null since it's a duplicate
                            if (!matched)
                            {
                                var offendingToken = info.Tokens.First();

                                err = new ParserError {
                                    ErrorType = ParserError.InvalidToken,
                                    LastValidMatchingNode = info.Source.Node,
                                    OffendingToken = offendingToken,
                                    ExpectedNodes = info.Source.Node.NextNodes,
                                    Position = offendingToken.StartPosition,
                                };
                            }

                            rejects.Add(branch.NodeMatch, err);
                        }
                    }

                    foreach (var next in nextSources)
                    {
                        sources.Enqueue(next, next.NodeMatch.Token.IndexOfNextTokenization);
//                        Logger.WriteLine("Enqueuing source with next index {0}", next.NodeMatch.Token.IndexOfNextTokenization);
                    }
                }
            }

            if (ends.Count > 0)
            {
                foreach (var reject in rejects)
                {
                    StripReject(reject.NodeMatch);
                }

                return MakeSpans(ends);
            }

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

            return new Span[0];
        }

        static IEnumerable<Node> GetExpectedNodes(ParseInfo info)
        {
            if (info.Source.Node.NextNodes.Count > 0)
            {
                return info.Source.Node.NextNodes;
            }

            foreach (var ender in info.Enders)
            {
                if (ender.NodeMatch.Node.NextNodes.Count > 0)
                {
                    return ender.NodeMatch.Node.NextNodes;
                }
            }

            return new Node[0];
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

