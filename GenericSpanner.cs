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
