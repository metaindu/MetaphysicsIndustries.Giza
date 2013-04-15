using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("{Name}, {Nodes.Count} Nodes")]
    public class Definition
    {
        public string Name;
        public Set<Node> Nodes = new Set<Node>();
        public Node start;
        public Node end;

        public IEnumerable<Node> GetStartingNodes()
        {
            foreach (Node n in start.NextNodes)
            {
                if (n is EndNode) continue;
                if (n is StartNode) continue;

                yield return n;
            }
        }

        public IEnumerable<Node> GetEndingNodes()
        {
            foreach (Node n in Nodes)
            {
                if (n is EndNode) continue;
                if (n is StartNode) continue;

                if (n.IsAnEndOf(this))
                {
                    yield return n;
                }
            }
        }

        public bool IgnoreWhitespace = false;
        public bool IgnoreCase = false;
    }
}
