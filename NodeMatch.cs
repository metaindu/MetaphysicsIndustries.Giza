using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatch
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
}

