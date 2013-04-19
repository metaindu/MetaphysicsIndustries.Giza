using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("[{_id}] {Node.Type}:{Node., {Transition}, {Nexts.Count} nexts")]
    public class NodeMatch
    {
        private static int __id = 0;
        public readonly int _id;

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
    }
}

