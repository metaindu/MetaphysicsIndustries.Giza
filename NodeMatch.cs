using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.IO;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatch<T>
        where T : IInputElement
    {
        private static int __id = 0;

        public readonly int _id;

        public NodeMatch<T> StartDef;
        public T InputElement;
        public TransitionType Transition;
        public Node Node;

        public Action WhenRejected = null;

        public InputPosition StartPosition
        {
            get
             {
                if (!string.IsNullOrEmpty(InputElement.Value) &&
                         InputElement.Value != "\0")
                {
                    return InputElement.Position;
                }
                else
                {
                    return AlternateStartPosition;
                }
            }
        }
        public InputPosition AlternateStartPosition = new InputPosition(-1);


        public NodeMatch(Node node, TransitionType transition, NodeMatch<T> previous)
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
        public ICollection<NodeMatch<T>> Nexts
        {
            get { return _nexts; }
        }

        NodeMatch<T> _previous;
        public NodeMatch<T> Previous
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

        public NodeMatch<T> CloneWithNewInputElement(T inputElement)
        {
            var nm = new NodeMatch<T>(this.Node, this.Transition, this.Previous);
            nm.StartDef = this.StartDef;
            nm.InputElement = inputElement;

            return nm;
        }

        public override string ToString()
        {
            string nodestr;
            if (Node is CharNode) nodestr = (Node as CharNode).CharClass.ToString();
            else if (Node is DefRefNode) nodestr = (Node as DefRefNode).DefRef.Name;
            else nodestr = "<unknown>";

            if (!string.IsNullOrEmpty(InputElement.Value))
            {
                return string.Format("[{0}] {1}:{2}, {3} nm nexts, input element '{4}' as {{unknown definition}}",
                                     _id,
                                     nodestr,
                                     Transition,
                                     Nexts.Count,
                                     InputElement.Value);
            }
            else
            {
                return string.Format("[{0}] {1}:{2}, {3} nm nexts", _id, nodestr, Transition, Nexts.Count);
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
            foreach (var next in this.Nexts)
            {
                next.Render(writer, indent + "  ");
            }
        }

        public static string RenderPathToLeaf(NodeMatch<T> leaf)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            RenderPathToLeaf(leaf, sw);

            return sb.ToString();
        }
        public static void RenderPathToLeaf(NodeMatch<T> leaf, TextWriter writer)
        {
            var nodeMatches = new List<NodeMatch<T>>();
            var cur = leaf;
            while (cur != null)
            {
                nodeMatches.Add(cur);
                cur = cur.Previous;
            }
            nodeMatches.Reverse();

            int depth = 0;
            string indent = "";
            foreach (var nm in nodeMatches)
            {
                if (nm.Transition == TransitionType.StartDef)
                {
                    depth++;
                    indent = new string(' ', depth);
                }
                else if (nm.Transition == TransitionType.EndDef)
                {
                    depth--;
                    indent = new string(' ', depth);
                }

                writer.Write(indent);
                writer.Write(nm.ToString());
                writer.WriteLine();
            }
        }

        class NodeMatchNodeMatchPreviousNextsCollection : ICollection<NodeMatch<T>>
        {
            public NodeMatchNodeMatchPreviousNextsCollection(NodeMatch<T> container)
            {
                if (container == null) throw new ArgumentNullException("container");

                _container = container;
            }

            NodeMatch<T> _container;
            Set<NodeMatch<T>> _collection = new Set<NodeMatch<T>>();

            #region ICollection implementation

            public void Add(NodeMatch<T> item)
            {
                if (!this.Contains(item))
                {
                    _collection.Add(item);
                    item.Previous = _container;
                }
            }

            public void Clear()
            {
                foreach (var item in this.ToArray())
                {
                    this.Remove(item);
                }
            }

            public bool Contains(NodeMatch<T> item)
            {
                return _collection.Contains(item);
            }

            public void CopyTo(NodeMatch<T>[] array, int arrayIndex)
            {
                _collection.CopyTo(array, arrayIndex);
            }

            public bool Remove(NodeMatch<T> item)
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

            public IEnumerator<NodeMatch<T>> GetEnumerator()
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
    }

    public class MatchStack<T>
        where T : IInputElement
    {
        public MatchStack(NodeMatch<T> nodeMatch, MatchStack<T> parent)
        {
            if (nodeMatch == null) throw new ArgumentNullException("nodeMatch");

            NodeMatch = nodeMatch;
            Parent = parent;
        }

        public NodeMatch<T> NodeMatch { get; protected set; }
        public MatchStack<T> Parent { get; protected set; }
        public DefRefNode Node { get { return (DefRefNode)NodeMatch.Node; } }
        public Definition Definition { get { return Node.DefRef; } }
    }

    public struct NodeMatchStackPair<T>
        where T : IInputElement
    {
        public NodeMatchStackPair(NodeMatch<T> nm, MatchStack<T> stack)
        {
            NodeMatch = nm;
            MatchStack = stack;
        }

        public NodeMatch<T> NodeMatch;
        public MatchStack<T> MatchStack;

        public static NodeMatchStackPair<T> CreateStartDefMatch(Node node, NodeMatch<T> match, MatchStack<T> stack2, InputPosition pos)
        {
            var match2 = new NodeMatch<T>(node, TransitionType.StartDef, match);
            match2.AlternateStartPosition = pos;
            return new NodeMatchStackPair<T>(match2, stack2);
        }

        public NodeMatchStackPair<T> CreateEndDefMatch()
        {
            return CreateEndDefMatch(this.NodeMatch, this.MatchStack);
        }

        public static NodeMatchStackPair<T> CreateEndDefMatch(NodeMatch<T> match, MatchStack<T> stack)
        {
            var match2 = new NodeMatch<T>(stack.Node, TransitionType.EndDef, match);
            match2.AlternateStartPosition = match.StartPosition;
            match2.StartDef = stack.NodeMatch;
            return new NodeMatchStackPair<T>(match2, stack.Parent);
        }

        public static NodeMatchStackPair<T> CreateFollowMatch(Node node, NodeMatch<T> match, MatchStack<T> stack, InputPosition pos)
        {
            var match2 = new NodeMatch<T>(node, TransitionType.Follow, match);
            match2.AlternateStartPosition = pos;
            return new NodeMatchStackPair<T>(match2, stack);
        }

    }

    public struct NodeMatchErrorPair<T>
        where T : IInputElement
    {
        public NodeMatchErrorPair(NodeMatch<T> nm, params Error[] errors)
            : this(nm, (ICollection<Error>)errors)
        {
        }
        public NodeMatchErrorPair(NodeMatch<T> nm, ICollection<Error> errors)
        {
            NodeMatch = nm;
            Errors = errors;
        }

        public NodeMatch<T> NodeMatch;
        public ICollection<Error> Errors;
        public Error Error
        {
            get { return ((Errors != null && Errors.Count > 0) ? Errors.First() : null); }
            set { Errors = new Error[] { value }; }
        }
    }

    public static class NodeMatchErrorPairHelper
    {
        public static void Add<T>(this ICollection<NodeMatchErrorPair<T>> collection, NodeMatch<T> nm, params Error[] errors)
            where T : IInputElement
        {
            // if all errors are null, replace with an empty array
            if (errors.Length > 0 &&
                errors.All(x => x == null))
            {
                errors = new Error[0];
            }

            collection.Add(new NodeMatchErrorPair<T>(nm, errors));
        }
        public static void Add<T>(this ICollection<NodeMatchErrorPair<T>> collection, NodeMatch<T> nm, ICollection<Error> errors)
            where T : IInputElement
        {
            // if all errors are null, replace with an empty collection
            if (errors.Count > 0 &&
                errors.All(x => x == null))
            {
                errors = new List<Error>();
            }

            collection.Add(new NodeMatchErrorPair<T>(nm, errors));
        }
    }
}

