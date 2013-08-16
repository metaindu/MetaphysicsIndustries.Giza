using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Parser
    {
        public class ParserError : Error
        {
            public static readonly ErrorType InvalidToken =         new ErrorType() { Name="InvalidToken",          Description="InvalidToken"          };
            public static readonly ErrorType UnexpectedEndOfInput = new ErrorType() { Name="UnexpectedEndOfInput",  Description="UnexpectedEndOfInput"  };
            public static readonly ErrorType ExcessRemainingInput = new ErrorType() { Name="ExcessRemainingInput",  Description="ExcessRemainingInput"  };

            public Token OffendingToken;
            public int Line;
            public int Column;
            public Node LastValidMatchingNode;
            public IEnumerable<Node> ExpectedNodes;
        }

        public Parser(Definition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            _definition = definition;
            _tokenizer = new Tokenizer(_definition.ParentGrammar);
        }

        Definition _definition;
        Tokenizer _tokenizer;


        public Span[] Parse(string input, List<Error> errors)
        {
            var sources = new Queue<NodeMatchStackPair>();
            var ends = new List<NodeMatch>();
            var rootDef = new Definition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch(rootNode, NodeMatch.TransitionType.Root, null);

            sources.Enqueue(pair(root, null));

            while (sources.Count > 0)
            {
                var nextSources = new List<NodeMatchStackPair>();

                while (sources.Count > 0)
                {
                    var sourcepair = sources.Dequeue();
                    var source = sourcepair.NodeMatch;

                    bool shouldRejectSource = true;

                    var currents = new Queue<NodeMatchStackPair>();

                    currents.Enqueue(sourcepair);

                    //find all ends
                    var ender = sourcepair;
                    while (true)
                    {
                        var nm = ender.NodeMatch;

                        if (nm == null) break;
                        if (nm.Transition == NodeMatch.TransitionType.Root) break;

                        if (ender.MatchStack == null)
                        {
                            ends.Add(ender.NodeMatch);
                            shouldRejectSource = false;
                            break;
                        }
                        else if (nm.Node.IsEndNode)
                        {
                            ender = ender.CreateEndDefMatch();
                            currents.Enqueue(ender);
                        }
                        else
                        {
                            break;
                        }
                    }

                    //get all tokens, starting at end of source's token
                    var errors2 = new List<Error>();
                    var intokens = _tokenizer.GetTokensAtLocation(
                        input,
                        source.Token.StartIndex + source.Token.Length,
                        errors2);
                    //if we get a token, set shouldRejectSource to false
                    if (intokens != null &&
                        intokens.Length > 0)
                    {
                        shouldRejectSource = false;
                    }
                    else
                    {
                        Error err;
                        if (errors2.ContainsNonWarnings())
                        {
                            //reject source with error
                            Spanner.SpannerError se = (errors2.GetFirstNonWarning() as Spanner.SpannerError);
                            ParserError pe = new ParserError();
                            err = pe;
                            pe.LastValidMatchingNode = source.Node;
                            pe.ExpectedNodes = pe.LastValidMatchingNode.NextNodes;
                            if (se.ErrorType == Spanner.SpannerError.UnexpectedEndOfInput)
                            {
                                pe.ErrorType = ParserError.UnexpectedEndOfInput;
                                pe.Column = se.Column;
                                pe.Line = se.Line;
                            }
                            else if (se.ErrorType == Spanner.SpannerError.ExcessRemainingInput)
                            {
                                // this shouldn't happen. when we read tokens,
                                // we set mustUseAllInput to false
                                throw new InvalidOperationException("Excess remaining input when reading tokens");
                            }
                            else if (se.ErrorType == Spanner.SpannerError.InvalidCharacter)
                            {
                                pe.ErrorType = Spanner.SpannerError.InvalidCharacter;
                                pe.Column = se.Column;
                                pe.Line = se.Line;
                            }
                            else
                            {
                                // in the future, this shouldn't happen. The
                                // only thing that would cause an error that's
                                // not a SpannerError would be something that
                                // DefinitionChecker found. And
                                // DefinitionChecker in Spanner.Match shouldn't
                                // find any problems in the definitions after
                                // we've already checked them in Parser.Parse
                                // or Tokenizer (which we don't do yet).
                                throw new InvalidOperationException("Errors in definitions");
                            }

                            errors.Add(err);
                            Reject(source, err);
                            continue;
                        }
                        else
                        {
                            err = new Error() {
                                ErrorType=Error.Unknown,
                            };
                            errors.Add(err);
                        }
                    }

                    //find all branches
                    var branches = new List<NodeMatchStackPair>();
                    while (currents.Count > 0)
                    {
                        NodeMatchStackPair current = currents.Dequeue();
                        NodeMatch cur = current.NodeMatch;
                        MatchStack curstack = current.MatchStack;

                        if (cur.DefRef.IsTokenized && 
                            cur != source)
                        {
                            branches.Add(current);
                            shouldRejectSource = false;
                            continue;
                        }


                        if (cur.DefRef.IsTokenized ||
                            cur.Transition == NodeMatch.TransitionType.EndDef)
                        {
                            foreach (var next in cur.Node.NextNodes)
                            {
                                NodeMatch nm = new NodeMatch(next, NodeMatch.TransitionType.Follow, cur);
                                currents.Enqueue(pair(nm, curstack));
                            }
                        }
                        else
                        {
                            var nextStack = new MatchStack(cur, curstack);
                            foreach (var start in (cur.Node as DefRefNode).DefRef.StartNodes)
                            {
                                NodeMatch nm = new NodeMatch(start, NodeMatch.TransitionType.StartDef, cur);
                                currents.Enqueue(pair(nm, nextStack));
                            }
                        }
                    }

                    //if no tokens, reject source and continue
                    //if no branches, reject source and continue
                    //however, if ends, then dont reject source
                    if (shouldRejectSource)
                    {
                        foreach (var branch in branches)
                        {
                            Reject(branch.NodeMatch, null);
                        }

                        Reject(source, null);
                        continue;
                    }

                    foreach (var branch in branches)
                    {
                        var branchnm = branch.NodeMatch;
                        var branchstack = branch.MatchStack;

                        foreach (var intoken in intokens)
                        {
                            if ((branchnm.Node is DefRefNode) && 
                                (branchnm.Node as DefRefNode).DefRef == intoken.Definition)
                            {
                                var newNext = branchnm.CloneWithNewToken(intoken);
                                nextSources.Add(pair(newNext, branchstack));
                            }
                        }

                        Reject(branch.NodeMatch, null);
                    }
                }

                foreach (var next in nextSources)
                {
                    sources.Enqueue(next);
                }
            }

            return MakeSpans(ends, input);
        }

        static Span[] MakeSpans(IEnumerable<NodeMatch> matchTreeLeaves, string input)
        {
            List<List<NodeMatch>> lists = new List<List<NodeMatch>>();
            foreach (NodeMatch leaf in matchTreeLeaves)
            {
                NodeMatch cur = leaf;
                List<NodeMatch> list = new List<NodeMatch>();

                while (cur != null)
                {
                    list.Add(cur);
                    cur = cur.Previous;
                }
                list.Reverse();

                lists.Add(list);
            }

            List<Span> spans = new List<Span>();
            foreach (List<NodeMatch> list in lists)
            {
                Stack<Span> stack = new Stack<Span>();

                Span rootSpan = null;

                foreach (NodeMatch nm in list)
                {
                    if (nm.Transition == NodeMatch.TransitionType.EndDef)
                    {
                        rootSpan = stack.Pop();
                    }
                    else if (!nm.DefRef.IsTokenized)
                    {
                        Span s = new Span();
                        s.Node = nm.Node;
                        if (stack.Count > 0)
                        {
                            stack.Peek().Subspans.Add(s);
                        }
                        stack.Push(s);
                    }
                    else
                    {
                        Span s = new Span();
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

        List<NodeMatchErrorPair> _rejects = new List<NodeMatchErrorPair>();
        public void Reject(NodeMatch reject, Error error)
        {
            _rejects.Add(new NodeMatchErrorPair(reject, error));

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

