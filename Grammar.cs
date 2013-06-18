using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public Grammar()
        {
            _definitions = new GrammarDefinitionOrderedParentChildrenCollection(this);
        }

        private GrammarDefinitionOrderedParentChildrenCollection _definitions;
        public GrammarDefinitionOrderedParentChildrenCollection Definitions
        {
            get { return _definitions; }
        }

        public Grammar Clone()
        {
            Dictionary<Definition, Definition> defsMatchup = new Dictionary<Definition, Definition>();
            List<Definition> defs = new List<Definition>();
            foreach (Definition def in this.Definitions)
            {
                Definition def2 = new Definition(def.Name);
                defs.Add(def2);
                defsMatchup[def] = def2;
            }
            Dictionary<Node, Node> nodeMatchup = new Dictionary<Node, Node>();
            foreach (Definition def in this.Definitions)
            {
                Definition def2 = defsMatchup[def];
                foreach (Node node in def.Nodes)
                {
                    Node node2;
                    if (node is CharNode)
                    {
                        node2 = new CharNode((node as CharNode).CharClass, node.Tag);
                    }
                    else
                    {
                        node2 = new DefRefNode(defsMatchup[(node as DefRefNode).DefRef], node.Tag);
                    }
                    nodeMatchup[node] = node2;
                    def2.Nodes.Add(node2);
                }
                foreach (Node node in def.Nodes)
                {
                    Node node2 = nodeMatchup[node];
                    foreach (Node next in node.NextNodes)
                    {
                        node2.NextNodes.Add(nodeMatchup[next]);
                    }
                }
                foreach (Node start in def.StartNodes)
                {
                    def2.StartNodes.Add(nodeMatchup[start]);
                }
                foreach (Node end in def.EndNodes)
                {
                    def2.EndNodes.Add(nodeMatchup[end]);
                }
                def2.Directives.AddRange(def.Directives);
            }

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs);

            return grammar;
        }

        public Definition FindDefinitionByName(string name)
        {
            foreach (Definition def in Definitions)
            {
                if (def.Name == name)
                {
                    return def;
                }
            }

            return null;
        }
    }
}

