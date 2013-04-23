using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Definition
    {
        private static int __id = 0;
        public readonly int _id;

        public Definition()
        {
            _id = __id;
            __id++;
        }

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

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2} nodes", _id, Name, Nodes.Count);
        }
    }
}
