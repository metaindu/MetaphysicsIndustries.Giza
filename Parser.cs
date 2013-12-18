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
        void LogState(
            EndCandidatesByIndexCollection<T> endCandidatesByIndex,
            BranchTipsByIndexCollection<T> branchTipsByIndex)
        {
            foreach (var kvp in branchTipsByIndex)
            {
                foreach (var branchTip in kvp.Value)
                {
                    Logger.WriteLine("b index {0}: [{1}]", kvp.Key, branchTip.Branch.NodeMatch.ToString());
                }
            }
            foreach (var kvp in endCandidatesByIndex)
            {
                foreach (var endCandidate in kvp.Value)
                {
                    Logger.WriteLine("e index {0}: [{1}]", kvp.Key, endCandidate.ToString());
                }
            }
        }
        public NodeMatch<T>[] Match(IInputSource<T> inputSource, ICollection<Error> errors)
        {
            if (inputSource == null) throw new ArgumentNullException("inputSource");
            if (errors == null) throw new ArgumentNullException("errors");

            var sources = new PriorityQueue<NodeMatchStackPair<T> , int>(lowToHigh: true);
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

            var endCandidatesByIndex = new EndCandidatesByIndexCollection<T>();
            var branchTipsByIndex = new BranchTipsByIndexCollection<T>();


            var rootInfo = GetParseInfoFromSource(pair(root, null), branchTipsByIndex, endCandidatesByIndex);


            while (!inputSource.IsAtEnd)
            {
                var index = inputSource.CurrentPosition.Index;

                Logger.WriteLine("");
                Logger.WriteLine("Main Parse Loop");
                Logger.WriteLine("index {0}, source is {1}at end", index, (inputSource.IsAtEnd ? "" : "not "));
                LogState(endCandidatesByIndex, branchTipsByIndex);

                if (!branchTipsByIndex.Values.SelectMany(x => x).Any() &&
                    !endCandidatesByIndex.Values.SelectMany(x => x).Any())
                {
                    // no branch tips or end candidates left
                    Logger.WriteLine("Out of branch tips and end candidates. Leaving main parse loop.");
                    break;
                }

                var inputElementSet = inputSource.GetNextValue();

                //if we get any errors, process them and reject
                if (inputElementSet.Errors.ContainsNonWarnings())
                {
                    while (branchTipsByIndex[index].Count > 0)
                    {
                        var branchTip = branchTipsByIndex[index].Dequeue();
                        rejects.AddReject(branchTip.Branch.NodeMatch, inputElementSet.Errors);
                    }

                    while (endCandidatesByIndex[index].Count > 0)
                    {
                        var endCandidate = endCandidatesByIndex[index].Dequeue();
                        rejects.AddReject(endCandidate, inputElementSet.Errors);
                    }
                }
                else if (inputElementSet.EndOfInput)
                {
                    while (branchTipsByIndex[index].Count > 0)
                    {
                        var branchTip = branchTipsByIndex[index].Dequeue();
                        var source = branchTip.Source;
                        var err = new ParserError<T> {
                            ErrorType = ParserError.UnexpectedEndOfInput,
                            LastValidMatchingNode = source.NodeMatch.Node,
                            ExpectedNodes = source.GetExpectedNodes(),
                            Position = inputElementSet.EndOfInputPosition,
                        };
                        rejects.AddReject(branchTip.Branch.NodeMatch, err);
                    }
                }
                else // we have valid input elements
                {
                    var offendingInputElement = inputElementSet.InputElements.First();

                    while (endCandidatesByIndex[index].Count > 0)
                    {
                        var endCandidate = endCandidatesByIndex[index].Dequeue();
                        var err = new ParserError<T> {
                            ErrorType = ParserError.ExcessRemainingInput,
                            LastValidMatchingNode = endCandidate.GetLastValidMatch().Node,
                            Position = offendingInputElement.Position,
                            OffendingInputElement = offendingInputElement,
                        };
                        rejects.AddReject(endCandidate, err);
                    }

                    while (branchTipsByIndex[index].Count > 0)
                    {
                        var branchTip = branchTipsByIndex[index].Dequeue();
                        var branchnm = branchTip.Branch.NodeMatch;
                        var branchstack = branchTip.Branch.MatchStack;

                        // try to match branch to input elements
                        bool matched = false;
                        foreach (var inputElement in inputElementSet.InputElements)
                        {
                            if (BranchTipMatchesInputElement(branchnm, inputElement))
                            {
                                Logger.WriteLine("Branch [{0}] matches [{1}]", branchnm.ToString(), inputElement.Value);
                                var newNext = branchnm.CloneWithNewInputElement(inputElement);
                                var newNextInfo = GetParseInfoFromSource(pair(newNext, branchstack), branchTipsByIndex, endCandidatesByIndex);
                                matched = true;
                            }
                        }

                        ParserError<T> err2 = null;
                        // if the branch didn't match, reject it with InvalidInputElement
                        // otherwise, reject it with null since it's a duplicate
                        if (!matched)
                        {
                            Logger.WriteLine("Branch [{0}] does not match", branchnm.ToString());
                            var source = branchnm.GetLastValidMatch();
                            err2 = new ParserError<T> {
                                ErrorType = ParserError.InvalidInputElement,
                                LastValidMatchingNode = source.Node,
                                OffendingInputElement = offendingInputElement,
                                ExpectedNodes = source.Node.NextNodes,
                                Position = offendingInputElement.Position,
                            };
                        }

                        rejects.AddReject(branchnm, err2);
                    }
                }
            }

            if (inputSource.IsAtEnd)
            {
                Logger.WriteLine("input source is at end.");
            }

            Logger.WriteLine("Main parse loop ended.");

            var ends = new List<NodeMatch<T>>();
            foreach (var collection in endCandidatesByIndex.Values)
            {
                ends.AddRange(collection);
            }
            foreach (var collection in branchTipsByIndex.Values)
            {
                foreach (var branchTip in collection)
                {
                    var source = branchTip.Source;
                    var err = new ParserError<T> {
                        ErrorType = ParserError.UnexpectedEndOfInput,
                        LastValidMatchingNode = source.NodeMatch.Node,
                        ExpectedNodes = source.GetExpectedNodes(),
                        Position = inputSource.CurrentPosition,
                    };
                    rejects.AddReject(branchTip.Branch.NodeMatch, err);
                }
            }

            if (ends.Count > 0)
            {
                Logger.WriteLine("Stripping rejects");
                foreach (var reject in rejects)
                {
                    StripReject(reject.NodeMatch);
                }
            }
            else
            {
                if (rejects.Count > 0)
                {
                    NodeMatch<T> rejectToUse = null;
                    IEnumerable<Error> errorsToUse = null;
                    foreach (var reject in (rejects as IEnumerable<NodeMatchErrorPair<T>>).Reverse())
                    {
                        if (reject.Errors != null && reject.Errors.Any())
                        {
                            rejectToUse = reject.NodeMatch;
                            errorsToUse = reject.Errors;
                            break;
                        }
                    }

                    Logger.WriteLine("Chosen reject: [{0}]", rejectToUse);
                    foreach (var error in errorsToUse)
                    {
                        Logger.WriteLine("Rejecting with error: {0}", error.Description);
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

        ParseInfo GetParseInfoFromSource(
            NodeMatchStackPair<T> source,
            BranchTipsByIndexCollection<T> branchTipsByIndex,
            EndCandidatesByIndexCollection<T> endCandidatesByIndex)
        {
            Logger.WriteLine("Branching from [{0}]", source.NodeMatch.ToString());
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
                    Logger.WriteLine("Found end candidate [{0}]", ender.NodeMatch.ToString());
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
                    Logger.WriteLine("Found branch tip [{0}]", current.NodeMatch.ToString());
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

            var endCandidate = info.EndCandidate;
            var branches = info.Branches;
            int index = info.Source.InputElement.IndexOfNextElement;

            if (endCandidate != null)
            {
//                ends.Add(endCandidate);
                endCandidatesByIndex[index].Enqueue(endCandidate);
                Logger.WriteLine("Adding end candidate at index {0}", index);

                endCandidate.WhenRejected +=
                    () => {
//                    ends.Remove(endCandidate);
                    info.EndCandidate = null;
//                    endCandidatesByIndex[index].Remove(endCandidate);
                };
            }

            foreach (var branchTip in branches)
            {
                Logger.WriteLine("Adding branch tip [{0}] at index {1}", branchTip.NodeMatch.ToString(), index);
                branchTipsByIndex[index].Enqueue(
                    new BranchTip<T> {
                        Branch = branchTip,
                        Source = source
                    });
//                var tempBranchTip = branchTip;
//                branchTip.NodeMatch.WhenRejected += () => branchTipsByIndex[index].Remove(tempBranchTip);
//                branchTip.NodeMatch.WhenMatched += () => branchTipsByIndex[index].Remove(tempBranchTip);
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

