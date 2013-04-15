using System;

namespace MetaphysicsIndustries.Giza
{
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
    }
}

