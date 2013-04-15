using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatch
    {
        public NodeMatch(Node node, TransitionType transition)
        {
            if (node == null) throw new ArgumentNullException("node");

            _nexts = new NodeMatchNodeMatchPreviousNextsCollection(this);

            Node = node;
            Transition = transition;
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

