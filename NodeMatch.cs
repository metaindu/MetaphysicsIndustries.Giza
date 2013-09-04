using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.IO;

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

            if (Token.Length > 0)
            {
                return string.Format("[{0}] {1}:{2}, {3} nexts, token '{4}' as {5}",
                                     _id,
                                     nodestr,
                                     Transition,
                                     Nexts.Count,
                                     Token.Value,
                                     Token.Definition.Name);
            }
            else
            {
                return string.Format("[{0}] {1}:{2}, {3} nexts", _id, nodestr, Transition, Nexts.Count);
            }
        }

        public string Render()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            Render(sw, string.Empty);

            return sb.ToString();
        }
        public void Render(TextWriter writer, string indent)
        {
            writer.Write(indent);
            writer.Write(this.ToString());
            writer.WriteLine();
            foreach (NodeMatch next in this.Nexts)
            {
                next.Render(writer, indent + "  ");
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
        public NodeMatchStackPair(NodeMatch nm, MatchStack stack)
        {
            NodeMatch = nm;
            MatchStack = stack;
        }

        public NodeMatch NodeMatch;
        public MatchStack MatchStack;

        public static NodeMatchStackPair CreateStartDefMatch(Node node, NodeMatch match, MatchStack stack2, int index)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.StartDef, match);
            match2.Index = index;
            return new NodeMatchStackPair(match2, stack2);
        }

        public NodeMatchStackPair CreateEndDefMatch()
        {
            return CreateEndDefMatch(this.NodeMatch, this.MatchStack);
        }

        public static NodeMatchStackPair CreateEndDefMatch(NodeMatch match, MatchStack stack)
        {
            NodeMatch match2 = new NodeMatch(stack.Node, NodeMatch.TransitionType.EndDef, match);
            match2.Index = match.Index;
            match2.StartDef = stack.NodeMatch;
            return new NodeMatchStackPair(match2, stack.Parent);
        }

        public static NodeMatchStackPair CreateFollowMatch(Node node, NodeMatch match, MatchStack stack, int index)
        {
            NodeMatch match2 = new NodeMatch(node, NodeMatch.TransitionType.Follow, match);
            match2.Index = index;
            return new NodeMatchStackPair(match2, stack);
        }

    }

    public struct NodeMatchErrorPair
    {
        public NodeMatchErrorPair(NodeMatch nm, Error error)
        {
            NodeMatch = nm;
            Error = error;
        }

        public NodeMatch NodeMatch;
        public Error Error;
    }

    public static class NodeMatchErrorPairHelper
    {
        public static void Add(this ICollection<NodeMatchErrorPair> collection, NodeMatch nm, Error err)
        {
            collection.Add(new NodeMatchErrorPair(nm, err));
        }
    }
}

