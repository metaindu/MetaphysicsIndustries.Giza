using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatch
    {
        public NodeMatch()
        {
            _nexts = new NodeMatchNodeMatchPreviousNextsCollection(this);
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
    }
}

