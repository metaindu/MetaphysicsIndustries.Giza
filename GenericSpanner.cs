using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MetaphysicsIndustries.Giza
{
    public class GenericSpanner
    {
        public Span Process(Definition[] defs2, string startName, string input)
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

            int i = 0;
            Span span = GetItem(start, input, ref i);
            if (span != null) span.Tag = start.Name;
            return span;
        }

        public Span Process2(Definition[] defs2, string startName, string input)
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

            int i = 0;
            Span span = GetItem2(start, input);
            if (span != null) span.Tag = start.Name;
            return span;
        }

        class Backtrack
        {
            public Backtrack(int _i, Node _prev, Node[] _nexts, int _next)
            {
                i = _i;
                prev = _prev;
                nexts = (Node[])_nexts.Clone();
                next = _next;
            }
            public int i;
            public Node prev;
            public Node[] nexts;
            public int next;
            public List<Span> Spans = new List<Span>();
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

        public Span GetItem2(Definition def, string input)
        {
            DefRefNode implicitNode = new DefRefNode(def);
            NodeMatch root = new NodeMatch(implicitNode, NodeMatch.TransitionType.StartDef);
            root._k = -1;

            MatchStack defStack = new MatchStack(root, null);

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

            return null;
        }

        public Span GetItem(Definition def, string input, ref int i)
        {
            Node cur = def.start;
            List<Backtrack> backtracks = new List<Backtrack>();

            int start = i;
            List<Span> subspans = new List<Span>();
            int lastValueChar = i;

            for (; i < input.Length; i++)
            {
                if (cur == def.end) break;

                char ch = input[i];
                if (def.IgnoreWhitespace && char.IsWhiteSpace(ch)) continue;

                int ii = i;
                i = ii;

                List<Node> tnexts = new List<Node>();
                bool linksToEnd = false;
                foreach (Node node in cur.NextNodes)
                {
                    if (node == def.end)
                    {
                        linksToEnd = true;
                    }
                    else if (node.Matches(ch))
                    {
                        tnexts.Add(node);
                    }
                }

                Node next = null;
                if (tnexts.Count < 1)
                {
                    //invalid characters
                    if (linksToEnd)
                    {
                        break;
                    }
                    if (!DoBacktrack(ref i, backtracks, ref next))
                    {
                        ErrorContext = cur;
                        ErrorCharacter = i;
                        return null;
                    }
                }
                else
                {
                    if (tnexts.Count > 1)
                    {
                        backtracks.Add(new Backtrack(i, cur, tnexts.ToArray(), 0));
                    }

                    next = tnexts[0];
                }
                ErrorContext = null;

                Span item = null;
                while (item == null)
                {
                    if (next.Type == NodeType.defref)
                    {
                        if (def.Name == "grammar")
                        {
                        }

                        int j = i;
                        i = j;
                        item = GetItem(((DefRefNode)next).DefRef, input, ref i);
                        if (item == null)
                        {
                            if (linksToEnd)
                            {
                                break;
                            }
                            if (!DoBacktrack(ref i, backtracks, ref next))
                            {
                                return null;
                            }
                            
                        }

                    }
                    else
                    {
                        item = new Span(i, 1, input);
                    }
                }

                item.Definition = def.ToString();
                //item.NodeText = next.Tag;
                item.Tag = next.Tag;
                if (backtracks.Count > 0)
                {
                    backtracks[backtracks.Count - 1].Spans.Add(item);
                }
                else
                {
                    subspans.Add(item);
                }

                cur = next;
                lastValueChar = i;
            }

            foreach (Backtrack bt in backtracks)
            {
                subspans.AddRange(bt.Spans);
            }

            i--;

            return new Span(start, lastValueChar - start + 1, input, subspans.ToArray());
        }

        private static bool DoBacktrack(ref int i, List<Backtrack> backtracks, ref Node next)
        {
            while (backtracks.Count > 0)
            {
                Backtrack bt = backtracks[backtracks.Count - 1];
                if (bt.next >= bt.nexts.Length)
                {
                    //backtrack fail
                    //go to the previous backtrack

                    backtracks.RemoveAt(backtracks.Count - 1);
                }
                else
                {
                    i = bt.i;
                    bt.next++;
                    next = bt.nexts[bt.next];

                    break;
                }
            }

            if (backtracks.Count < 1)
            {
                //all backtracks failed
                return false;
            }

            return true;
        }
    }
}
