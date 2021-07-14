﻿
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class Parser : ParserBase<Token>
    {
        public Parser(NDefinition definition)
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
        public ParserBase(NDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            _definition = definition;
        }

        protected readonly NDefinition _definition;

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
            var rootDef = new NDefinition("$rootDef");
            var rootNode = new DefRefNode(_definition, "$rootNode");
            rootDef.Nodes.Add(rootNode);
            rootDef.StartNodes.Add(rootNode);
            rootDef.EndNodes.Add(rootNode);
            var root = new NodeMatch<T>(rootNode, TransitionType.Root, null);
            var rejects = new List<NodeMatchErrorPair<T>>();

            sources.Enqueue(pair(root, null), -1);
//            Logger.WriteLine("Starting");

            var endCandidatesByIndex = new EndCandidatesByIndexCollection<T>();
            var branchTipsByIndex = new BranchTipsByIndexCollection<T>();


            FindBranchTipsAndEndCandidates(pair(root, null), branchTipsByIndex, endCandidatesByIndex, 0);


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

                Logger.WriteLine("Getting the next input element set");
                var inputElementSet = inputSource.GetNextValue();

                //if we get any errors, process them and reject
                if (inputElementSet.Errors.ContainsNonWarnings())
                {
                    Logger.WriteLine("Got the following errors:");
                    foreach (var err in inputElementSet.Errors.Where(x => !x.IsWarning))
                    {
                        Logger.WriteLine("  {0}", err.Description);
                    }

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
                    Logger.WriteLine("Input source is at end");

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
                    Logger.WriteLine("Got valid input elements:");
                    foreach (var inputElement in inputElementSet.InputElements)
                    {
                        Logger.WriteLine("  {0}", inputElement.Value);
                    }

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
                                BindInputElementToBranchTip(inputElement, branchTip, branchTipsByIndex, endCandidatesByIndex);
                                matched = true;
                            }
                            else if (BranchTipIgnoresInputElement(branchTip, inputElement))
                            {
                                Logger.WriteLine("Branch [{0}] ignores [{1}]", branchnm.ToString(), inputElement.Value);
                                IgnoreInputElementForBranchTip(inputElement, branchTip, branchTipsByIndex, endCandidatesByIndex);
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

        protected virtual bool BranchTipIgnoresInputElement(BranchTip<T> branchTip, T inputElement)
        {
            return false;
        }

        protected virtual void IgnoreInputElementForBranchTip(T inputElement, BranchTip<T> branchTip, BranchTipsByIndexCollection<T> branchTipsByIndex, EndCandidatesByIndexCollection<T> endCandidatesByIndex)
        {
            throw new NotImplementedException();
        }

        protected virtual void BindInputElementToBranchTip(
            T inputElement,
            BranchTip<T> branchTip,
            BranchTipsByIndexCollection<T> branchTipsByIndex,
            EndCandidatesByIndexCollection<T> endCandidatesByIndex)
        {
            var newNext = branchTip.Branch.NodeMatch.CloneWithNewInputElement(inputElement);
            FindBranchTipsAndEndCandidates(
                pair(newNext, branchTip.Branch.MatchStack),
                branchTipsByIndex,
                endCandidatesByIndex,
                inputElement.IndexOfNextElement);
        }

        void FindBranchTipsAndEndCandidates(
            NodeMatchStackPair<T> source,
            BranchTipsByIndexCollection<T> branchTipsByIndex,
            EndCandidatesByIndexCollection<T> endCandidatesByIndex,
            int index)
        {
            Logger.WriteLine("Branching from [{0}]", source.NodeMatch.ToString());

            var currents = new Queue<NodeMatchStackPair<T>>();

            currents.Enqueue(source);

            // find all ends
            var enders = new List<NodeMatchStackPair<T>>();
            NodeMatch<T> endCandidate = null;
            if (source.NodeMatch.Transition != TransitionType.Root)
            {
                var ender = source;

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
                    endCandidate = ender.NodeMatch;
                }
            }

            foreach (var ender in enders)
            {
                currents.Enqueue(ender);
            }

            //find all branches
            var branches = new List<NodeMatchStackPair<T>>();
            while (currents.Count > 0)
            {
                var current = currents.Dequeue();
                var cur = current.NodeMatch;
                var curstack = current.MatchStack;

                var isBranchTip = IsBranchTip(cur);

                if (isBranchTip &&
                    cur != source.NodeMatch)
                {
                    Logger.WriteLine("Found branch tip [{0}]", current.NodeMatch.ToString());
                    branches.Add(current);
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

            if (endCandidate != null)
            {
                endCandidatesByIndex[index].Enqueue(endCandidate);
                Logger.WriteLine("Adding end candidate at index {0}", index);
            }

            foreach (var branchTip in branches)
            {
                Logger.WriteLine("Adding branch tip [{0}] at index {1}", branchTip.NodeMatch.ToString(), index);
                branchTipsByIndex[index].Enqueue(
                    new BranchTip<T> {
                        Branch = branchTip,
                        Source = source
                    });
            }
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

