using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("{Name}, {Nodes.Count} Nodes")]
    public class SimpleDefinitionNode
    {
        public string Name;
        public Set<SimpleNode> Nodes = new Set<SimpleNode>();
        public SimpleNode start;
        public SimpleNode end;

        public bool IgnoreWhitespace = false;
        public bool IgnoreCase = false;
    }
}
