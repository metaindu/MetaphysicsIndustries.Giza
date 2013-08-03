using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatch
    {
        private static int __id = 0;
        public readonly int _id;
        public int Index = -1;
        public NodeMatch StartDef;
        public Token Token;

        public NodeMatch(Node node, TransitionType transition, NodeMatch previous)
        {
            if (node == null) throw new ArgumentNullException("node");

            _nexts = new NodeMatchNodeMatchPreviousNextsCollection(this);

            Node = node;
            Transition = transition;
            Previous = previous;

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
            Root,
        }

        public TransitionType Transition;
        public Node Node;
        public Definition DefRef
        {
            get
            {
                if (Node is DefRefNode)
                {
                    return (Node as DefRefNode).DefRef;
                }

                return null;
            }
        }
        public char MatchedChar;

        public override string ToString()
        {
            string nodestr;
            if (Node is CharNode) nodestr = (Node as CharNode).CharClass.ToString();
            else if (Node is DefRefNode) nodestr = (Node as DefRefNode).DefRef.Name;
            else nodestr = "<unknown>";

            return string.Format("[{0}] {1}:{2}, {3} nexts", _id, nodestr, Transition, Nexts.Count);
        }

        public static string Render(NodeMatch nm)
        {
            StringBuilder sb = new StringBuilder();
            Render(nm, sb, "");
            return sb.ToString();
        }
        protected static void Render(NodeMatch nm, StringBuilder sb, string indent)
        {
            sb.Append(indent);
            sb.Append(nm.ToString());
            sb.AppendLine();
            foreach (NodeMatch next in nm.Nexts)
            {
                Render(next, sb, indent + "  ");
            }
        }

        public NodeMatch CloneWithNewToken(Token token)
        {
            NodeMatch nm = new NodeMatch(this.Node, this.Transition, this.Previous);
            nm.Index = this.Index;
            nm.StartDef = this.StartDef;
            nm.Token = token;

            return nm;
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

    public class MatchStack
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

    public struct NodeMatchStackPair
    {
        public NodeMatch NodeMatch;
        public MatchStack MatchStack;
    }

    public struct NodeMatchSpannerErrorPair
    {
        public NodeMatch NodeMatch;
        public Spanner.SpannerError Error;
    }
}

