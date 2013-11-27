using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MetaphysicsIndustries.Collections;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Spanner
    {
        public Spanner(Definition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            // check incoming definitions
            DefinitionChecker dc = new DefinitionChecker();
            var errors2 = new List<Error>(dc.CheckDefinitions(definition.ParentGrammar.Definitions));
            if (errors2.Count > 0)
            {
                //errors.AddRange(errors2);
            }

            _definition = definition;
        }

        Definition _definition;

        public Span[] Process(CharacterSource input, List<Error> errors)
        {
            NodeMatch[] matchTreeLeaves = Match(input, errors);

            return MakeSpans(matchTreeLeaves);
        }

        public NodeMatch[] Match(CharacterSource input, List<Error> errors, bool mustUseAllInput=true, int startIndex=0)
        {
            bool endOfInput;
            InputPosition endOfInputPosition;
            return Match(input, errors, out endOfInput, out endOfInputPosition, mustUseAllInput, startIndex);
        }
        public NodeMatch[] Match(CharacterSource input, List<Error> errors, out bool endOfInput, out InputPosition endOfInputPosition, bool mustUseAllInput=true, int startIndex=0)
        {

            if (startIndex >= input.Length)
            {
                endOfInput = true;
                endOfInputPosition = input.GetPosition(input.Length);
                return new NodeMatch[0];
            }

            DefRefNode implicitNode = new DefRefNode(_definition);
            NodeMatch root = new NodeMatch(implicitNode, NodeMatch.TransitionType.Root, null);


            Queue<NodeMatchStackPair> currents = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> accepts = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchErrorPair> rejects = new Queue<NodeMatchErrorPair>();
            Queue<NodeMatch> ends = new Queue<NodeMatch>();

            NodeMatchErrorPair lastReject = pair2(null, new SpannerError{ ErrorType=SpannerError.ExcessRemainingInput });

            currents.Enqueue(pair(root, null));

            input.SetCurrentIndex(startIndex);

            var prevpos = input.CurrentPosition;
            var ch = input.Peek();
            var prevch = ch;

            while (!input.IsAtEnd)
            {
                prevch = ch;
                ch = input.GetNextValue();

                bool isWhitespace = char.IsWhiteSpace(ch.Value);

                if (mustUseAllInput && ends.Count > 0)
                {
                    // move all ends to rejects
                    while (ends.Count > 0)
                    {
                        var end = ends.Dequeue();
                        rejects.Enqueue(pair2(end,
                            new SpannerError {
                                ErrorType=SpannerError.ExcessRemainingInput,
                                Position = prevpos,
                                PreviousNode=end.Node,
                                OffendingCharacter=prevch.Value,
                            }));
                    }
                }

                if (currents.Count < 1) break;

                var enddefs = new Set<NodeMatch>();
                while (currents.Count > 0)
                {
                    NodeMatchStackPair p = currents.Dequeue();
                    NodeMatch cur = p.NodeMatch;
                    MatchStack stack = p.MatchStack;

                    if (isWhitespace &&
                        ((stack == null && cur.Node is DefRefNode && !(cur.Node as DefRefNode).DefRef.MindWhitespace) ||
                         (stack != null && !stack.Definition.MindWhitespace)))
                    {
                        accepts.Enqueue(p);
                    }
                    else if (cur.Node is CharNode)
                    {
                        if ((cur.Node as CharNode).Matches(ch.Value))
                        {
                            cur.MatchedChar = ch;

                            //next nodes
                            foreach (Node n in cur.Node.NextNodes)
                            {
                                accepts.Enqueue(NodeMatchStackPair.CreateFollowMatch(n, cur, stack, ch.Position));
                            }

                            //end
                            if ((!stack.Definition.Atomic || cur.Node.NextNodes.Count < 1) &&
                                cur.Node.IsEndNode)
                            {
                                accepts.Enqueue(p.CreateEndDefMatch());
                            }
                        }
                        else
                        {
                            if (stack.Definition.Atomic &&
                                stack.Definition.Nodes.Contains(cur.Previous.Node) &&
                                cur.Previous.Node.IsEndNode &&
                                !enddefs.Contains(cur.Previous))
                            {
                                currents.Enqueue(NodeMatchStackPair.CreateEndDefMatch(cur.Previous, stack));
                                enddefs.Add(cur.Previous);
                            }





                            // GenerateError

                            SpannerError se = new SpannerError();
                            se.ErrorType = SpannerError.InvalidCharacter;

                            if (cur.Transition == NodeMatch.TransitionType.Root)
                            {
                                throw new NotImplementedException();
                            }

                            char errorCh = input[cur.StartPosition.Index];

                            IEnumerable<Node> expectedNodes;
                            Set<char> vowels = new Set<char> {
                                'a', 'e', 'i', 'o', 'u',
                                'A', 'E', 'I', 'O', 'U',
                            };
                            StringBuilder sb = new StringBuilder();

                            var pos = cur.StartPosition;
                            se.OffendingCharacter = errorCh;
                            se.Position = pos;
                            sb.AppendFormat("Invalid character '{0}' at ({1},{2})", errorCh, pos.Line, pos.Column);

                            NodeMatch cur2 = null;

                            cur2 = cur.Previous;
                            while (cur2 != null &&
                                cur2.Transition == NodeMatch.TransitionType.StartDef)
                            {
                                cur2 = cur2.Previous;
                            }

                            if (cur2 != null)
                            {
                                cur2 = cur2.Previous;
                            }

                            if (cur2 != null &&
                                cur2.Previous != null)
                            {
                                string an = "a";
                                string after = "";

                                if (cur2.Previous.Node is CharNode)
                                {
                                    after = GetDescriptionsOfCharClass((cur2.Previous.Node as CharNode).CharClass)[0];
                                }
                                else
                                {
                                    after = (cur2.Previous.Node as DefRefNode).DefRef.Name;
                                }

                                if (vowels.Contains(after[0]))
                                {
                                    an = "an";
                                }
                                sb.AppendFormat(", after {0} {1}: expected ", an, after);
                            }

                            if (cur2 == null)
                            {
                                //failed to start
                                expectedNodes = _definition.StartNodes;
                                se.ExpectedDefinition = _definition;
                                se.ExpectedNodes = _definition.StartNodes;

                                string an = "a";
                                if (vowels.Contains(_definition.Name[0]))
                                {
                                    an = "an";
                                }

                                sb.AppendFormat(": {0} {1} must start with ", an, _definition.Name);

                            }
                            else if (cur2.Node is CharNode)
                            {
                                expectedNodes = cur2.Node.NextNodes;
                                se.PreviousNode = cur2.Node;
                                se.ExpectedNodes = se.PreviousNode.NextNodes;
                            }
                            else // cur.Node is DefRefNode
                            {
                                expectedNodes = (cur2.Node as DefRefNode).DefRef.StartNodes;
                                se.PreviousNode = cur2.Node;
                                se.ExpectedNodes = se.PreviousNode.NextNodes;
                            }

                            if (expectedNodes != null)
                            {
                                CharClass expectedChars = new CharClass(new char[0]);
                                Set<Definition> expectedDefs = new Set<Definition>();
                                foreach (Node node in expectedNodes)
                                {
                                    if (node is CharNode)
                                    {
                                        expectedChars =
                                            CharClass.Union(
                                                (node as CharNode).CharClass,
                                                expectedChars);
                                    }
                                    else
                                    {
                                        expectedDefs.Add((node as DefRefNode).DefRef);
                                    }
                                }

                                List<string> expects = new List<string>();

                                foreach (Definition expdef in expectedDefs)
                                {
                                    expects.Add(expdef.Name);
                                }

                                expects.AddRange(GetDescriptionsOfCharClass(expectedChars));

                                int i;
                                for (i = 2; i < expects.Count; i++)
                                {
                                    sb.AppendFormat("{0}, ", expects[i-2]);
                                }
                                if (expects.Count > 1)
                                {
                                    sb.Append(expects[expects.Count - 2]);
                                    if (expects.Count > 2)
                                    {
                                        sb.Append(",");
                                    }
                                    sb.Append(" or ");
                                }
                                if (expects.Count > 0)
                                {
                                    sb.Append(expects.Last());
                                }
                                else
                                {
                                    sb.AppendLine("[We don't expect anything?]");
                                }
                            }

                            se.DescriptionString = sb.ToString();








                            rejects.Enqueue(pair2(cur, se));
                        }
                    }
                    else // cur.Node is DefRefNode
                    {
                        if (cur.Transition == NodeMatch.TransitionType.EndDef)
                        {
                            foreach (Node n in cur.Node.NextNodes)
                            {
                                currents.Enqueue(NodeMatchStackPair.CreateFollowMatch(n, cur, stack, ch.Position));
                            }

                            if (cur.Node == implicitNode)
                            {
                                if (!input.IsAtEnd || !mustUseAllInput)
                                {
                                    ends.Enqueue(cur);
                                }
                                else
                                {
                                    rejects.Enqueue(pair2(cur,
                                                          new SpannerError {
                                        ErrorType=SpannerError.ExcessRemainingInput,
                                        Position = ch.Position,
                                        PreviousNode=cur.Node,
                                        OffendingCharacter=ch.Value,
                                    }));
                                }
                            }
                            else if (cur.Node.IsEndNode)
                            {
                                currents.Enqueue(p.CreateEndDefMatch());
                            }
                        }
                        else
                        {
                            MatchStack stack2 = new MatchStack(cur, stack);
                            foreach (Node start in (cur.Node as DefRefNode).DefRef.StartNodes)
                            {
                                currents.Enqueue(NodeMatchStackPair.CreateStartDefMatch(start, cur, stack2, ch.Position));
                            }
                        }
                    }
                }

                PurgeRejects(rejects, ref lastReject);

                while (accepts.Count > 0)
                {
                    currents.Enqueue(accepts.Dequeue());
                }

                prevpos = ch.Position;
            }

            if (input.IsAtEnd &&
                currents.Count == 1 &&
                currents.Peek().NodeMatch.Transition == NodeMatch.TransitionType.Root)
            {
                //end of input on the root node
                endOfInput = true;
                endOfInputPosition = input.GetPosition(input.Length);
                return new NodeMatch[0];
            }
            else
            {
                endOfInput = false;
                endOfInputPosition = new InputPosition(-1);
            }

            // at this point, the only things in `currents` would be the
            // previous contents of `accepts`, which are all the nodes
            // immediately after the CharNodes that matched a char.
            while (currents.Count > 0)
            {
                NodeMatchStackPair p = currents.Dequeue();
                NodeMatch cur = p.NodeMatch;
                MatchStack stack = p.MatchStack;

                if (cur.Node is CharNode ||
                    cur.Transition != NodeMatch.TransitionType.EndDef)
                {
                    if (stack != null &&
                        stack.Definition != null &&
                        stack.Definition.Atomic &&
                        stack.Definition.Nodes.Contains(cur.Previous.Node) &&
                        cur.Previous != null &&
                        cur.Previous.Node.IsEndNode)
                    {
                        currents.Enqueue(NodeMatchStackPair.CreateEndDefMatch(cur.Previous, stack));
                    }

                    SpannerError se = new SpannerError {
                        PreviousNode=cur.Previous.Node,
                        ErrorType=SpannerError.UnexpectedEndOfInput,
                        Position = input.CurrentPosition,
                        ExpectedNodes = cur.Previous.Node.NextNodes,
                    };
                    rejects.Enqueue(pair2(cur, se));
                }
                else if (cur.Node == implicitNode)
                {
                    ends.Enqueue(cur);
                }
                else if (cur.Node.IsEndNode)
                {
                    currents.Enqueue(NodeMatchStackPair.CreateEndDefMatch(cur, stack));
                }
                else
                {
                    SpannerError se = new SpannerError {
                        PreviousNode=cur.Node,
                        ErrorType=SpannerError.UnexpectedEndOfInput,
                        Position = input.CurrentPosition,
                        ExpectedNodes = cur.Node.NextNodes,
                    };
                    rejects.Enqueue(pair2(cur, se));
                }
            }

            PurgeRejects(rejects, ref lastReject);

            // if nothing is in ends, then we ran aground, either on an invalid
            //  char or end-of-input
            // if there's something in ends and k < length + 1, then we
            //  finished but still have input
            // otherwise, eveything in ends is a valid parse

            if (ends.Count < 1)
            {
                errors.AddRange(lastReject.Errors);
            }

            PurgeReject(lastReject.NodeMatch);

            List<NodeMatch> matchTreeLeaves = new List<NodeMatch>();
            while (ends.Count > 0)
            {
                matchTreeLeaves.Add(ends.Dequeue());
            }

            return matchTreeLeaves.ToArray();
        }

        static Span[] MakeSpans(IEnumerable<NodeMatch> matchTreeLeaves)
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
                    else if (nm.Node is DefRefNode)
                    {
                        Span s = new Span();
                        s.Node = nm.Node;
                        if (stack.Count > 0)
                        {
                            stack.Peek().Subspans.Add(s);
                        }
                        stack.Push(s);
                    }
                    else // nm.Node is CharNode
                    {
                        Span s = new Span();
                        s.Node = nm.Node;
                        s.Value = nm.MatchedChar.Value.ToString();
                        stack.Peek().Subspans.Add(s);
                    }
                }

                spans.Add(rootSpan);
            }

            return spans.ToArray();
        }

        public static List<string> GetDescriptionsOfCharClass(CharClass expectedChars)
        {
            List<string> expects2 = new List<string>();
            if (!expectedChars.Exclude &&
                !expectedChars.Digit &&
                !expectedChars.Letter &&
                !expectedChars.Whitespace &&
                expectedChars.GetNonClassCharsCount() > 0 &&
                expectedChars.GetNonClassCharsCount() <= 3)
            {
                // only a few characters - list each of them in quotes
                foreach (char ch in expectedChars.GetNonClassChars())
                {
                    expects2.Add(string.Format("'{0}'", ch.ToString()));
                }
            }
            else if (expectedChars.Digit ||
                     expectedChars.Letter ||
                     expectedChars.Whitespace ||
                     expectedChars.GetNonClassCharsCount() > 0)
            {
                // treat as char class
                expects2.Add(string.Format("a character that matches {0}", expectedChars));
            }
            else
            {
                // empty char class - don't do anything
            }
            return expects2;
        }



        public static NodeMatchStackPair pair(NodeMatch nodeMatch, MatchStack matchStack)
        {
            return new NodeMatchStackPair{NodeMatch = nodeMatch, MatchStack = matchStack};
        }

        public static NodeMatchErrorPair pair2(NodeMatch nodeMatch, SpannerError err)
        {
            return new NodeMatchErrorPair{NodeMatch=nodeMatch, Error=err};
        }


        void PurgeRejects(Queue<NodeMatchErrorPair> rejects, ref NodeMatchErrorPair lastReject)
        {
            while (rejects.Count > 0)
            {
                var n = lastReject;
                lastReject = rejects.Dequeue();

                if (n.NodeMatch == null) break;

                PurgeReject(n.NodeMatch);
            }
        }

        void PurgeReject(NodeMatch reject)
        {
            while (reject != null &&
                   reject.Nexts.Count < 1)
            {
                NodeMatch prev = reject.Previous;
                reject.Previous = null;
                //recycle the NodeMatch object here, if desired
                reject = prev;
            }
        }
    }
}
