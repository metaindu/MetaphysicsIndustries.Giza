using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MetaphysicsIndustries.Giza
{
    public class GenericSpanner
    {
        public Span[] Process2(Definition[] defs2, string startName, string input)
        {
            Definition start = null;
            foreach (Definition d in defs2)
            {
                if (d.Name == startName)
                {
                    start = d;
                    break;
                }
            }

            if (start == null)
            {
                throw new KeyNotFoundException("Could not find a definition by that name");
            }

            Span[] spans = GetItem2(start, input);
            return spans;
        }

        public Node ErrorContext;
        public int ErrorCharacter;

        struct NodeMatchStackPair
        {
            public NodeMatch NodeMatch;
            public MatchStack MatchStack;
        }

        NodeMatchStackPair pair(NodeMatch nodeMatch, MatchStack matchStack)
        {
            return new NodeMatchStackPair{NodeMatch = nodeMatch, MatchStack = matchStack};
        }

        NodeMatchStackPair CreateStartDefMatch(Node node, NodeMatch match, MatchStack stack2)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.StartDef);
            match2.Previous = match;
            return pair(match2, stack2);
        }

        NodeMatchStackPair CreateEndDefMatch(NodeMatch match, MatchStack stack)
        {
            NodeMatch match2 = new NodeMatch(stack.Node, NodeMatch.TransitionType.EndDef);
            match2.Previous = match;
            return pair(match2, stack.Parent);
        }

        NodeMatchStackPair CreateFollowMatch(Node node, NodeMatch match, MatchStack stack)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.Follow);
            match2.Previous = match;
            return pair(match2, stack);
        }

        void PurgeRejects(Queue<NodeMatch> rejects, int k, ref NodeMatch lastReject, ref int lastk)
        {
            while (rejects.Count > 0)
            {
                NodeMatch n = lastReject;
                lastReject = rejects.Dequeue();
                lastk = k;

                if (n == null) break;

                while (n.Nexts.Count < 1)
                {
                    NodeMatch prev = n.Previous;
                    n.Previous = null;

                    //recycle the NodeMatch object here, if desired

                    n = prev;
                }
            }
        }

        public Span[] GetItem2(Definition def, string input)
        {
            DefRefNode implicitNode = new DefRefNode(def);
            NodeMatch root = new NodeMatch(implicitNode, NodeMatch.TransitionType.StartDef);
            root._k = -1;

            Queue<NodeMatchStackPair> currents = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> accepts = new Queue<NodeMatchStackPair>();
            Queue<NodeMatch> rejects = new Queue<NodeMatch>();
            Queue<NodeMatch> ends = new Queue<NodeMatch>();

            NodeMatch lastReject = null;

            currents.Enqueue(pair(root, null));

            int k = -1;
            int lastk = k;
            foreach (char ch in input)
            {
                k++;
                bool isWhitespace = char.IsWhiteSpace(ch);

                // move all ends to rejects
                while (ends.Count > 0)
                {
                    rejects.Enqueue(ends.Dequeue());
                }

                while (currents.Count > 0)
                {
                    NodeMatchStackPair p = currents.Dequeue();
                    NodeMatch cur = p.NodeMatch;
                    MatchStack stack = p.MatchStack;

                    if (isWhitespace &&
                        ((stack == null && cur.Node is DefRefNode && (cur.Node as DefRefNode).DefRef.IgnoreWhitespace) ||
                         (stack != null && stack.Definition.IgnoreWhitespace)))
                    {
                        accepts.Enqueue(p);
                    }
                    else if (cur.Node is CharNode)
                    {
                        if ((cur.Node as CharNode).Matches(ch))
                        {
                            cur.MatchedChar = ch;
                            cur._k = k;

                            //next nodes
                            foreach (Node n in cur.Node.NextNodes)
                            {
                                if (n is EndNode) continue;

                                accepts.Enqueue(CreateFollowMatch(n, cur, stack));
                            }

                            //end
                            if (!stack.Definition.Contiguous &&
                                cur.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                            {
                                accepts.Enqueue(CreateEndDefMatch(cur, stack));
                            }
                        }
                        else
                        {
                            if (stack.Definition.Contiguous && 
                                stack.Definition.Nodes.Contains(cur.Previous.Node) &&
                                cur.Previous.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                            {
                                currents.Enqueue(CreateEndDefMatch(cur.Previous, stack));
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
                                if (n is EndNode) continue;

                                currents.Enqueue(CreateFollowMatch(n, cur, stack));
                            }

                            if (cur.Node == implicitNode)
                            {
                                ends.Enqueue(cur);
                            }
                            else if (cur.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                            {
                                currents.Enqueue(CreateEndDefMatch(cur, stack));
                            }
                        }
                        else
                        {
                            MatchStack stack2 = new MatchStack(cur, stack);
                            foreach (Node start in (cur.Node as DefRefNode).DefRef.GetStartingNodes())
                            {
                                currents.Enqueue(CreateStartDefMatch(start, cur, stack2));
                            }
                        }
                    }
                }

                PurgeRejects(rejects, k, ref lastReject, ref lastk);

                while (accepts.Count > 0)
                {
                    currents.Enqueue(accepts.Dequeue());
                }
            }

            while (currents.Count > 0)
            {
                NodeMatchStackPair p = currents.Dequeue();
                NodeMatch cur = p.NodeMatch;
                MatchStack stack = p.MatchStack;

                if (cur.Node is CharNode ||
                    cur.Transition != NodeMatch.TransitionType.EndDef)
                {
                    rejects.Enqueue(cur);
                }
                else
                {
                    if (cur.Node == implicitNode)
                    {
                        ends.Enqueue(cur);
                    }
                    else if (cur.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                    {
                        currents.Enqueue(CreateEndDefMatch(cur, stack));
                    }
                }
            }

            PurgeRejects(rejects, k, ref lastReject, ref lastk);

            //eveything in ends is a valid parse

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

            List<List<NodeMatch>> lists = new List<List<NodeMatch>>();

            while (ends.Count > 0)
            {
                NodeMatch cur = ends.Dequeue();
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
                        s.Definition = (nm.Node as DefRefNode).DefRef;
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
                        s.Value = nm.MatchedChar.ToString();
                        stack.Peek().Subspans.Add(s);
                    }
                }

                spans.Add(rootSpan);
            }

            return spans.ToArray();
        }
    }
}
