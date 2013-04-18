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

        public Span GetItem2(Definition def, string input)
        {
            DefRefNode implicitNode = new DefRefNode(def);
            NodeMatch root = new NodeMatch(implicitNode, NodeMatch.TransitionType.StartDef);

            MatchStack defStack = new MatchStack(root, null);

            Queue<NodeMatchStackPair> starts = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> currents = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> accepts = new Queue<NodeMatchStackPair>();
            Queue<NodeMatchStackPair> pass = new Queue<NodeMatchStackPair>();
            Queue<NodeMatch> rejects = new Queue<NodeMatch>();

            starts.Enqueue(pair(root, null));

            int k = -1;
            foreach (char ch in input)
            {
                k++;
                bool isWhitespace = char.IsWhiteSpace(ch);

                while (starts.Count > 0)
                {
                    NodeMatchStackPair p = starts.Dequeue();

                    if (isWhitespace &&
                        ((p.MatchStack == null && p.NodeMatch.Node is DefRefNode && (p.NodeMatch.Node as DefRefNode).DefRef.IgnoreWhitespace) ||
                         (p.MatchStack != null && p.MatchStack.Definition.IgnoreWhitespace)))
                    {
                        pass.Enqueue(p);
                    }
                    else if (p.NodeMatch.Node is CharNode)
                    {
                        NodeMatch cur = p.NodeMatch;
                        MatchStack stack = p.MatchStack;

                        //next nodes
                        foreach (Node n in cur.Node.NextNodes)
                        {
                            if (n is EndNode) continue;

                            NodeMatch match2 = new NodeMatch(n,NodeMatch.TransitionType.Follow);
                            match2.Previous = cur;
                            currents.Enqueue(pair(match2, stack));
                        }

                        //end
                        if (cur.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                        {
                            //add def end nodematch, pop the stack
                            NodeMatch match2 = new NodeMatch(stack.NodeMatch.Node, NodeMatch.TransitionType.EndDef);
                            match2.Previous = cur;
                            currents.Enqueue(pair(match2, stack.Parent));
                        }
                    }
                    else // defrefnode
                    {
                        currents.Enqueue(p);
                    }
                }

                while (currents.Count > 0)
                {
                    NodeMatchStackPair p = currents.Dequeue();

                    if (isWhitespace &&
                        ((p.MatchStack == null && p.NodeMatch.Node is DefRefNode && (p.NodeMatch.Node as DefRefNode).DefRef.IgnoreWhitespace) ||
                         (p.MatchStack != null && p.MatchStack.Definition.IgnoreWhitespace)))
                    {
                        pass.Enqueue(p);
                        continue;
                    }

                    NodeMatch cur = p.NodeMatch;
                    MatchStack stack = p.MatchStack;

                    if (cur.Node is CharNode)
                    {
                        if ((cur.Node as CharNode).Matches(ch))
                        {
                            accepts.Enqueue(p);
                        }
                        else
                        {
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

                                NodeMatch match2 = new NodeMatch(n,NodeMatch.TransitionType.Follow);
                                match2.Previous = cur;
                                currents.Enqueue(pair(match2, stack));
                            }
                        }
                        else
                        {
                            MatchStack stack2 = new MatchStack(cur, stack);
                            foreach (Node start in (cur.Node as DefRefNode).DefRef.GetStartingNodes())
                            {
                                NodeMatch match2 = new NodeMatch(start, NodeMatch.TransitionType.StartDef);
                                match2.Previous = cur;
                                currents.Enqueue(pair(match2, stack2));
                            }
                        }

                        if (stack == null)
                        {
                            //throw new NotImplementedException();
                        }
                        else if (cur.Node.IsAnEndOf((stack.NodeMatch.Node as DefRefNode).DefRef))
                        {
                            //add def end nodematch, pop the stack
                            NodeMatch match2 = new NodeMatch(stack.NodeMatch.Node, NodeMatch.TransitionType.EndDef);
                            match2.Previous = cur;
                            currents.Enqueue(pair(match2, stack.Parent));
                        }
                    }
                }

                while (rejects.Count > 0)
                {
                    NodeMatch n = rejects.Dequeue();

                    if (n.Nexts.Count <= 0)
                    {
                        NodeMatch prev = n.Previous;
                        n.Previous = null;

                        //recycle the NodeMatch object here, if desired

                        if (prev != null && prev.Nexts.Count <= 0)
                        {
                            rejects.Enqueue(prev);
                        }
                    }
                }

                while (accepts.Count > 0)
                {
                    var p = accepts.Dequeue();
                    starts.Enqueue(p);
                }
                while (pass.Count > 0)
                {
                    var p = pass.Dequeue();
                    starts.Enqueue(p);
                }
            }

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
