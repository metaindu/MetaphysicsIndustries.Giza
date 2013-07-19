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
            public static readonly ErrorType InvalidCharacter = new ErrorType() { Name="InvalidCharacter", Description="InvalidCharacter" };

            public string DescriptionString = string.Empty;

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
            Queue<NodeMatch> rejects = new Queue<NodeMatch>();
            Queue<NodeMatch> ends = new Queue<NodeMatch>();

            NodeMatch lastReject = null;

            currents.Enqueue(pair(root, null));

            int k;
            for (k = startIndex; k < input.Length; k++)
            {
                char ch = input[k];

                if (currents.Count < 1) break;

                bool isWhitespace = char.IsWhiteSpace(ch);

                if (mustUseAllInput)
                {
                    // move all ends to rejects
                    while (ends.Count > 0)
                    {
                        rejects.Enqueue(ends.Dequeue());
                    }
                }

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

                            rejects.Enqueue(cur);
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

                    rejects.Enqueue(cur);
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
                    rejects.Enqueue(cur);
                }
            }

            PurgeRejects(rejects, ref lastReject);

            // if nothing is in ends, then we ran aground
            // if there's something in ends and k < length + 1, then we finished but still have input
            // otherwise, eveything in ends is a valid parse


            if (ends.Count < 1)
            {
                string error = GenerateErrorString(lastReject, def, input);
                var error2 = new SpannerError() { ErrorType=SpannerError.InvalidCharacter, DescriptionString=error };
            }

            PurgeReject(lastReject);

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

        string GenerateErrorString(NodeMatch lastReject, Definition def, string input)
        {
            if (lastReject.Transition == NodeMatch.TransitionType.Root) return null;

            char errorCh = input[lastReject.Index];

            IEnumerable<Node> expectedNodes;
            Set<char> vowels = new Set<char> {
                'a', 'e', 'i', 'o', 'u',
                'A', 'E', 'I', 'O', 'U',
            };
            StringBuilder sb = new StringBuilder();

            int line;
            int linek;
            GetPosition(input, lastReject.Index, out line, out linek);
            sb.AppendFormat("Invalid character '{0}' at ({1},{2})", errorCh, line, linek);

            NodeMatch cur = null;
            if (lastReject.Previous == null)
            {
                //failed to start
                expectedNodes = def.StartNodes;

                string an = "a";
                if (vowels.Contains(def.Name[0]))
                {
                    an = "an";
                }

                sb.AppendFormat(": {0} {1} must start with ", an, def.Name);
            }
            else
            {
                cur = lastReject.Previous;
                while (cur != null &&
                       cur.Transition == NodeMatch.TransitionType.StartDef)
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
                }
                else // cur.Node is DefRefNode
                {
                    expectedNodes = (cur.Node as DefRefNode).DefRef.StartNodes;
                }
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

            return sb.ToString();
        }

        public static Span[] MakeSpans(IEnumerable<NodeMatch> matchTreeLeaves)
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

        void PurgeRejects(Queue<NodeMatch> rejects, ref NodeMatch lastReject)
        {
            while (rejects.Count > 0)
            {
                NodeMatch n = lastReject;
                lastReject = rejects.Dequeue();

                if (n == null) break;

                PurgeReject(n);
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
