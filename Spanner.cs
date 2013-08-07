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
        public class SpannerError : Error
        {
            public static readonly ErrorType InvalidCharacter =     new ErrorType() { Name="InvalidCharacter",      Description="InvalidCharacter"      };
            public static readonly ErrorType UnexpectedEndOfInput = new ErrorType() { Name="UnexpectedEndOfInput",  Description="UnexpectedEndOfInput"  };
            public static readonly ErrorType ExcessRemainingInput = new ErrorType() { Name="ExcessRemainingInput",  Description="ExcessRemainingInput"  };

            public string DescriptionString = string.Empty;
            public char OffendingCharacter;
            public int Line;
            public int Column;
            public Node PreviousNode;
            public Definition ExpectedDefinition;
            public IEnumerable<Node> ExpectedNodes;

            public override string Description
            {
                get { return DescriptionString; }
            }
        }

        public Span[] Process(Grammar grammar, string startName, string input, List<Error> errors)
        {
            Definition start = grammar.FindDefinitionByName(startName);

            if (start == null)
            {
                throw new KeyNotFoundException("Could not find a definition by that name");
            }

            Span[] spans = Process(start, input, errors);
            return spans;
        }

        public Span[] Process(Definition def, string input, List<Error> errors)
        {
            NodeMatch[] matchTreeLeaves = Match(def, input, errors);

            return MakeSpans(matchTreeLeaves);
        }

        public NodeMatch[] Match(Definition def, string input, List<Error> errors, bool mustUseAllInput=true, int startIndex=0)
        {
            // check incoming definitions
            DefinitionChecker dc = new DefinitionChecker();
            var errors2 = new List<Error>(dc.CheckDefinitions(def.ParentGrammar.Definitions));
            if (errors2.Count > 0)
            {
                errors.AddRange(errors2);
                return null;
            }

            bool endOfInput = false;

            if (startIndex >= input.Length)
            {
                endOfInput = true;
                return new NodeMatch[0];
            }

            DefRefNode implicitNode = new DefRefNode(def);
            NodeMatch root = new NodeMatch(implicitNode, NodeMatch.TransitionType.Root, null);
            root.Index = -1;


            Queue<NodeMatchStackPair> currents = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> accepts = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchSpannerErrorPair> rejects = new Queue<NodeMatchSpannerErrorPair>();
            Queue<NodeMatch> ends = new Queue<NodeMatch>();

            NodeMatchSpannerErrorPair lastReject = pair2(null, new SpannerError{ ErrorType=SpannerError.ExcessRemainingInput });

            currents.Enqueue(pair(root, null));

            int k;
            for (k = startIndex; k < input.Length; k++)
            {
                char ch = input[k];

                bool isWhitespace = char.IsWhiteSpace(ch);

                if (mustUseAllInput && ends.Count > 0)
                {
                    int line;
                    int column;
                    GetPosition(input, k-1, out line, out column);

                    // move all ends to rejects
                    while (ends.Count > 0)
                    {
                        var end = ends.Dequeue();
                        rejects.Enqueue(pair2(end, 
                            new SpannerError {
                                ErrorType=SpannerError.ExcessRemainingInput,
                                Line=line,
                                Column=column,
                                PreviousNode=end.Node,
                                OffendingCharacter=input[k-1],
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
                        if ((cur.Node as CharNode).Matches(ch))
                        {
                            cur.MatchedChar = ch;
                            cur.Index = k;

                            //next nodes
                            foreach (Node n in cur.Node.NextNodes)
                            {
                                accepts.Enqueue(CreateFollowMatch(n, cur, stack, k));
                            }

                            //end
                            if ((!stack.Definition.Atomic || cur.Node.NextNodes.Count < 1) &&
                                cur.Node.IsEndNode)
                            {
                                accepts.Enqueue(CreateEndDefMatch(cur, stack));
                            }
                        }
                        else
                        {
                            if (stack.Definition.Atomic &&
                                stack.Definition.Nodes.Contains(cur.Previous.Node) &&
                                cur.Previous.Node.IsEndNode &&
                                !enddefs.Contains(cur.Previous))
                            {
                                currents.Enqueue(CreateEndDefMatch(cur.Previous, stack));
                                enddefs.Add(cur.Previous);
                            }

                            rejects.Enqueue(pair2(cur, SpannerError.InvalidCharacter));
                        }
                    }
                    else // cur.Node is DefRefNode
                    {
                        if (cur.Transition == NodeMatch.TransitionType.EndDef)
                        {
                            foreach (Node n in cur.Node.NextNodes)
                            {
                                currents.Enqueue(CreateFollowMatch(n, cur, stack, k));
                            }

                            if (cur.Node == implicitNode)
                            {
                                ends.Enqueue(cur);
                            }
                            else if (cur.Node.IsEndNode)
                            {
                                currents.Enqueue(CreateEndDefMatch(cur, stack));
                            }
                        }
                        else
                        {
                            MatchStack stack2 = new MatchStack(cur, stack);
                            foreach (Node start in (cur.Node as DefRefNode).DefRef.StartNodes)
                            {
                                currents.Enqueue(CreateStartDefMatch(start, cur, stack2, k));
                            }
                        }
                    }
                }

                PurgeRejects(rejects, ref lastReject);

                while (accepts.Count > 0)
                {
                    currents.Enqueue(accepts.Dequeue());
                }
            }

            if (k >= input.Length)
            {
                endOfInput = true;
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
                        currents.Enqueue(CreateEndDefMatch(cur.Previous, stack));
                    }

                    int line;
                    int column;
                    GetPosition(input, k, out line, out column);
                    SpannerError se = new SpannerError {
                        PreviousNode=cur.Previous.Node,
                        ErrorType=SpannerError.UnexpectedEndOfInput,
                        Line=line,
                        Column=column,
                    };
                    rejects.Enqueue(pair2(cur, se));
                }
                else if (cur.Node == implicitNode)
                {
                    ends.Enqueue(cur);
                }
                else if (cur.Node.IsEndNode)
                {
                    currents.Enqueue(CreateEndDefMatch(cur, stack));
                }
                else
                {
                    int line;
                    int column;
                    GetPosition(input, k, out line, out column);
                    SpannerError se = new SpannerError {
                        PreviousNode=cur.Node,
                        ErrorType=SpannerError.UnexpectedEndOfInput,
                        Line=line,
                        Column=column,
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
                var error = GenerateError(lastReject, def, input);
                errors.Add(error);
            }

            PurgeReject(lastReject.NodeMatch);

//            while (ends.Count > 0)
//            {
//                NodeMatch e = ends.Dequeue();
//                List<NodeMatch> matches = new List<NodeMatch>();
//
//                Console.WriteLine("Parse:");
//                Console.WriteLine();
//                while (e != null)
//                {
//                    matches.Add(e);
//                    e = e.Previous;
//                }
//                matches.Reverse();
//                foreach (NodeMatch nm in matches)
//                {
//                    string ch;
//
//                    if (nm._k < 0) ch = " ";
//                    else ch = input[nm._k].ToString();
//                    if (ch == "\r") ch = "\\r";
//                    else if (ch == "\n") ch = "\\n";
//                    else if (ch == "\t") ch = "\\t";
//
//                    Console.WriteLine("{0}\t{1}\t{2}", ch, nm.ToString(), nm.Node.ToString());
//                }
//            }

            List<NodeMatch> matchTreeLeaves = new List<NodeMatch>();
            while (ends.Count > 0)
            {
                matchTreeLeaves.Add(ends.Dequeue());
            }

            return matchTreeLeaves.ToArray();
        }

        Error GenerateError(NodeMatchSpannerErrorPair lastReject, Definition def, string input)
        {
            NodeMatch lastRejectnm = lastReject.NodeMatch;
            SpannerError se = lastReject.Error;

            if (se.ErrorType == SpannerError.InvalidCharacter)
            {
                if (lastRejectnm.Transition == NodeMatch.TransitionType.Root) return null;

                char errorCh = input[lastRejectnm.Index];

                IEnumerable<Node> expectedNodes;
                Set<char> vowels = new Set<char> {
                    'a', 'e', 'i', 'o', 'u',
                    'A', 'E', 'I', 'O', 'U',
                };
                StringBuilder sb = new StringBuilder();

                int line;
                int linek;
                GetPosition(input, lastRejectnm.Index, out line, out linek);
                se.OffendingCharacter = errorCh;
                se.Line = line;
                se.Column = linek;
                sb.AppendFormat("Invalid character '{0}' at ({1},{2})", errorCh, line, linek);

                NodeMatch cur = null;

                cur = lastRejectnm.Previous;
                while (cur != null &&
                       cur.Transition == NodeMatch.TransitionType.StartDef)
                {
                    cur = cur.Previous;
                }

                if (cur != null)
                {
                    cur = cur.Previous;
                }

                if (cur != null &&
                    cur.Previous != null)
                {
                    string an = "a";
                    string after = "";

                    if (cur.Previous.Node is CharNode)
                    {
                        after = GetDescriptionsOfCharClass((cur.Previous.Node as CharNode).CharClass)[0];
                    }
                    else
                    {
                        after = (cur.Previous.Node as DefRefNode).DefRef.Name;
                    }

                    if (vowels.Contains(after[0]))
                    {
                        an = "an";
                    }
                    sb.AppendFormat(", after {0} {1}: expected ", an, after);
                }

                if (cur == null)
                {
                    //failed to start
                    expectedNodes = def.StartNodes;
                    se.ExpectedDefinition = def;
                    se.ExpectedNodes = def.StartNodes;

                    string an = "a";
                    if (vowels.Contains(def.Name[0]))
                    {
                        an = "an";
                    }

                    sb.AppendFormat(": {0} {1} must start with ", an, def.Name);

                }
                else if (cur.Node is CharNode)
                {
                    expectedNodes = cur.Node.NextNodes;
                    se.PreviousNode = cur.Node;
                    se.ExpectedNodes = se.PreviousNode.NextNodes;
                }
                else // cur.Node is DefRefNode
                {
                    expectedNodes = (cur.Node as DefRefNode).DefRef.StartNodes;
                    se.PreviousNode = cur.Node;
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
                    sb.Append(expects.Last());
                }

                se.DescriptionString = sb.ToString();

                return se;
            }
            else if (se.ErrorType == SpannerError.UnexpectedEndOfInput)
            {
//                var expectedNodes = new Set<Node>();
//                var cur = lastRejectnm;
//                while (cur.Transition == NodeMatch.TransitionType.EndDef)
//                {
//                    expectedNodes.AddRange(cur.Node.NextNodes);
//                    cur = cur.Previous;
//                }
//                expectedNodes.AddRange(cur.Node.NextNodes);
//                se.ExpectedNodes = expectedNodes;
                se.ExpectedNodes = se.PreviousNode.NextNodes;
                return se;
            }
            else // (se.ErrorType == SpannerError.ExcessRemainingInput)
            {
                return se;
            }
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
                        s.Value = nm.MatchedChar.ToString();
                        stack.Peek().Subspans.Add(s);
                    }
                }

                spans.Add(rootSpan);
            }

            return spans.ToArray();
        }

        static List<string> GetDescriptionsOfCharClass(CharClass expectedChars)
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

        void GetPosition(string input, int k, out int line, out int linek)
        {
            line = 1;
            linek = 1;

            int i;
            for (i = 0; i < k; i++)
            {
                char ch = input[i];

                if (ch == '\n')
                {
                    line++;
                    linek = 1;
                }
                else
                {
                    linek++;
                }
            }
        }

        public static NodeMatchStackPair pair(NodeMatch nodeMatch, MatchStack matchStack)
        {
            return new NodeMatchStackPair{NodeMatch = nodeMatch, MatchStack = matchStack};
        }

        public static NodeMatchSpannerErrorPair pair2(NodeMatch nodeMatch, SpannerError err)
        {
            return new NodeMatchSpannerErrorPair{NodeMatch=nodeMatch, Error=err};
        }

        public static NodeMatchSpannerErrorPair pair2(NodeMatch nodeMatch, ErrorType et)
        {
            return new NodeMatchSpannerErrorPair{NodeMatch=nodeMatch, Error=new SpannerError{ErrorType=et}};
        }

        NodeMatchStackPair CreateStartDefMatch(Node node, NodeMatch match, MatchStack stack2, int index)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.StartDef, match);
            match2.Index = index;
            return pair(match2, stack2);
        }

        NodeMatchStackPair CreateEndDefMatch(NodeMatch match, MatchStack stack)
        {
            NodeMatch match2 = new NodeMatch(stack.Node, NodeMatch.TransitionType.EndDef, match);
            match2.Index = match.Index;
            match2.StartDef = stack.NodeMatch;
            return pair(match2, stack.Parent);
        }

        NodeMatchStackPair CreateFollowMatch(Node node, NodeMatch match, MatchStack stack, int index)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.Follow, match);
            match2.Index = index;
            return pair(match2, stack);
        }

        void PurgeRejects(Queue<NodeMatchSpannerErrorPair> rejects, ref NodeMatchSpannerErrorPair lastReject)
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
