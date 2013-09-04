using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class Parser
    {
        public class ParserError : Error
        {
            public static readonly ErrorType InvalidToken =         new ErrorType(name:"InvalidToken",          descriptionFormat:"InvalidToken"          );
            public static readonly ErrorType UnexpectedEndOfInput = new ErrorType(name:"UnexpectedEndOfInput",  descriptionFormat:"UnexpectedEndOfInput"  );
            public static readonly ErrorType ExcessRemainingInput = new ErrorType(name:"ExcessRemainingInput",  descriptionFormat:"ExcessRemainingInput"  );

            public Token OffendingToken;
            public int Index;
            public int Line;
            public int Column;
            public Node LastValidMatchingNode;
            public IEnumerable<Node> ExpectedNodes;

            public override string Description
            {
                get
                {
                    if (ErrorType == InvalidToken)
                    {
                        string expect = "";
                        if (ExpectedNodes.Count() > 1)
                        {
                            var expects = new List<string>();
                            foreach (var enode in ExpectedNodes)
                            {
                                expects.Add(((DefRefNode)ExpectedNodes.First()).DefRef.Name);
                            }

                            StringBuilder sb = new StringBuilder();
                            sb.Append("Expected ");
                            int i;
                            for (i = 1; i < expects.Count; i++)
                            {
                                sb.Append(expects[i - 1]);
                                sb.Append(", ");
                            }
                            sb.Append("or ");
                            sb.Append(expects.Last());
                            expect = sb.ToString();
                        }
                        else if (ExpectedNodes.Count() > 0)
                        {
                            expect = string.Format("Expected {0}.", ((DefRefNode)ExpectedNodes.First()).DefRef.Name);
                        }

                        return string.Format("Invalid token '{0}' at position {1},{2}.",
                                             OffendingToken.Value,
                                             Line,
                                             Column,
                                             expect);
                    }

                    return base.Description;
                }
            }
        }

        public Parser(Definition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            _definition = definition;
            _tokenizer = new Tokenizer(_definition.ParentGrammar);
        }

        Definition _definition;
        Tokenizer _tokenizer;

        struct ParseInfo
        {
            public NodeMatchStackPair SourcePair;
            public NodeMatch Source { get { return SourcePair.NodeMatch; } }
            public MatchStack SourceStack { get { return SourcePair.MatchStack; } }

            public List<NodeMatchStackPair> Enders;
            public bool LastEnderIsEndCandidate;

            public bool EndOfInput;
            public int EndOfInputIndex;
            public Token[] Tokens;
            public List<Error> TokenizationErrors;

            public List<NodeMatchStackPair> Branches;
        }

        public Span[] Parse(string input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            var sources = new Queue<NodeMatchStackPair>();
            var ends = new List<NodeMatch>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch(rootNode, NodeMatch.TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair>();

            sources.Enqueue(pair(root, null));

            while (sources.Count > 0)
            {
                var nextSources = new List<NodeMatchStackPair>();

                while (sources.Count > 0)
                {
                    var info = new ParseInfo();
                    info.SourcePair = sources.Dequeue();

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
                    var parseIndex = info.Source.Token.StartIndex + info.Source.Token.Length;
                    info.TokenizationErrors = new List<Error>();
                    info.Tokens = _tokenizer.GetTokensAtLocation(
                        input,
                        parseIndex,
                        info.TokenizationErrors,
                        out info.EndOfInput,
                        out info.EndOfInputIndex);

                    //if we get any tokenization errors, process them and reject
                    if (info.TokenizationErrors.ContainsNonWarnings())
                    {
                        //reject branches with error
                        Spanner.SpannerError se = (info.TokenizationErrors.GetFirstNonWarning() as Spanner.SpannerError);
                        var err = new ParserError();
                        err.LastValidMatchingNode = info.Source.Node;

                        err.ExpectedNodes = GetExpectedNodes(info);

                        if (se.ErrorType == Spanner.SpannerError.UnexpectedEndOfInput)
                        {
                            err.ErrorType = ParserError.UnexpectedEndOfInput;
                            err.Column = se.Column;
                            err.Line = se.Line;
                            err.Index = se.Index;
                        }
                        else if (se.ErrorType == Spanner.SpannerError.ExcessRemainingInput)
                        {
                            // this shouldn't happen. when we read tokens,
                            // we set mustUseAllInput to false
                            throw new InvalidOperationException("Excess remaining input when reading tokens");
                        }
                        else if (se.ErrorType == Spanner.SpannerError.InvalidCharacter)
                        {
                            err.ErrorType = ParserError.InvalidToken;
                            err.Column = se.Column;
                            err.Line = se.Line;
                            err.Index = se.Index;
                            err.OffendingToken.StartIndex = se.Index;
                            err.OffendingToken.Length = 1;
                            err.OffendingToken.Definition = null;
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
                        int line;
                        int column;

                        Spanner.GetPosition(input, info.EndOfInputIndex, out line, out column);

                        var err = new ParserError {
                            ErrorType = ParserError.UnexpectedEndOfInput,
                            LastValidMatchingNode = info.Source.Node,
                            ExpectedNodes = GetExpectedNodes(info),
                            Line = line,
                            Column = column,
                            Index = info.EndOfInputIndex,
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

                            int line;
                            int column;

                            Spanner.GetPosition(input, offendingToken.StartIndex,
                                                out line, out column);

                            var err = new ParserError {
                                ErrorType = ParserError.ExcessRemainingInput,
                                LastValidMatchingNode = info.Source.Node,
                                Line = line,
                                Column = column,
                                Index = offendingToken.StartIndex,
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
                                int line;
                                int column;

                                Spanner.GetPosition(input, info.Tokens[0].StartIndex,
                                                    out line, out column);

                                err = new ParserError {
                                    ErrorType = ParserError.InvalidToken,
                                    LastValidMatchingNode = info.Source.Node,
                                    OffendingToken = info.Tokens[0],
                                    ExpectedNodes = info.Source.Node.NextNodes,
                                    Line = line,
                                    Column = column,
                                    Index = info.Tokens[0].StartIndex,
                                };
                            }

                            rejects.Add(branch.NodeMatch, err);
                        }
                    }
                }

                foreach (var next in nextSources)
                {
                    sources.Enqueue(next);
                }
            }

            if (ends.Count > 0)
            {
                foreach (var reject in rejects)
                {
                    StripReject(reject.NodeMatch);
                }

                return MakeSpans(ends, input);
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

        static Span[] MakeSpans(IEnumerable<NodeMatch> matchTreeLeaves, string input)
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
                        s.Value = input.Substring(nm.Token.StartIndex, nm.Token.Length);
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

