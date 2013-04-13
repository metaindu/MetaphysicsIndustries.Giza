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

        public bool IgnoreWhitespace = false;
        public bool IgnoreCase = false;
    }
}
