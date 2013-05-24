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
        public Span[] Process(IEnumerable<Definition> defs, string startName, string input)
        {
            Definition start = null;
            foreach (Definition d in defs)
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

            Span[] spans = Process(start, input);
            return spans;
        }

        public Span[] Process(Definition def, string input)
        {
            // check incoming definitions
            DefinitionChecker dc = new DefinitionChecker();
            List<DefinitionChecker.ErrorInfo> errors = 
                new List<DefinitionChecker.ErrorInfo>(
                    dc.CheckDefinitions(def.ParentGrammar.Definitions));
            if (errors.Count > 0) throw new InvalidOperationException("Definitions contain errors.");

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
                                cur.Node.IsEndNode)
                            {
                                accepts.Enqueue(CreateEndDefMatch(cur, stack));
                            }
                        }
                        else
                        {
                            if (stack.Definition.Contiguous &&
                                stack.Definition.Nodes.Contains(cur.Previous.Node) &&
                                cur.Previous.Node.IsEndNode)
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

        class NodeMatch
        {
            private static int __id = 0;
            public readonly int _id;
            public int _k = -1;

            public NodeMatch(Node node, TransitionType transition)
            {
                if (node == null) throw new ArgumentNullException("node");

                _nexts = new NodeMatchNodeMatchPreviousNextsCollection(this);

                Node = node;
                Transition = transition;

                _id = __id;
                __id++;
            }

            NodeMatchNodeMatchPreviousNextsCollection _nexts;
            public ICollection<NodeMatch> Nexts
            {
                get { return _nexts; }
            }

            NodeMatch _previous;
            public NodeMatch Previous
            {
                get { return _previous; }
                set
                {
                    if (value != _previous)
                    {
                        if (_previous != null)
                        {
                            _previous.Nexts.Remove(this);
                        }

                        _previous = value;

                        if (_previous != null)
                        {
                            _previous.Nexts.Add(this);
                        }
                    }
                }
            }

            public enum TransitionType
            {
                StartDef,
                EndDef,
                Follow,
            }

            public TransitionType Transition;
            public Node Node;
            public char MatchedChar;

            public override string ToString()
            {
                string nodestr;
                if (Node is StartNode) nodestr = "<start>";
                else if (Node is EndNode) nodestr = "<end>";
                else if (Node is CharNode) nodestr = (Node as CharNode).CharClass.ToString();
                else if (Node is DefRefNode) nodestr = (Node as DefRefNode).DefRef.Name;
                else nodestr = "<unknown>";

                return string.Format("[{0}] {1}:{2}, {3} nexts", _id, nodestr, Transition, Nexts.Count);
            }
        }

        class NodeMatchNodeMatchPreviousNextsCollection : ICollection<NodeMatch>
        {
            public NodeMatchNodeMatchPreviousNextsCollection(NodeMatch container)
            {
                if (container == null) throw new ArgumentNullException("container");

                _container = container;
            }

            NodeMatch _container;
            Set<NodeMatch> _collection = new Set<NodeMatch>();

            #region ICollection implementation

            public void Add(NodeMatch item)
            {
                if (!this.Contains(item))
                {
                    _collection.Add(item);
                    item.Previous = _container;
                }
            }

            public void Clear()
            {
                foreach (NodeMatch item in this.ToArray())
                {
                    this.Remove(item);
                }
            }

            public bool Contains(NodeMatch item)
            {
                return _collection.Contains(item);
            }

            public void CopyTo(NodeMatch[] array, int arrayIndex)
            {
                _collection.CopyTo(array, arrayIndex);
            }

            public bool Remove(NodeMatch item)
            {
                if (_collection.Remove(item))
                {
                    item.Previous = null;

                    return true;
                }

                return false;
            }

            public int Count
            {
                get
                {
                    return _collection.Count;
                }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            #endregion

            #region IEnumerable implementation

            public IEnumerator<NodeMatch> GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            #endregion

            #region IEnumerable implementation

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        class MatchStack
        {
            public MatchStack(NodeMatch nodeMatch, MatchStack parent)
            {
                if (nodeMatch == null) throw new ArgumentNullException("nodeMatch");

                NodeMatch = nodeMatch;
                Parent = parent;
            }

            public NodeMatch NodeMatch { get; protected set; }
            public MatchStack Parent { get; protected set; }
            public DefRefNode Node { get { return (DefRefNode)NodeMatch.Node; } }
            public Definition Definition { get { return Node.DefRef; } }
        }

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
    }
}
