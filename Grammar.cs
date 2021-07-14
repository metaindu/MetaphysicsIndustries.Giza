
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public Grammar(params NDefinition[] definitions)
            : this((IEnumerable<NDefinition>)definitions)
        {
        }
        public Grammar(IEnumerable<NDefinition> definitions=null)
        {
            _definitions = new GrammarDefinitionOrderedParentChildrenCollection(this);

            if (definitions != null)
            {
                Definitions.AddRange(definitions);
            }
        }

        private GrammarDefinitionOrderedParentChildrenCollection _definitions;
        public GrammarDefinitionOrderedParentChildrenCollection Definitions
        {
            get { return _definitions; }
        }

        public Grammar Clone()
        {
            var defsMatchup = new Dictionary<NDefinition, NDefinition>();
            var defs = new List<NDefinition>();
            foreach (var def in Definitions)
            {
                var def2 = new NDefinition(def.Name);
                defs.Add(def2);
                defsMatchup[def] = def2;
            }
            Dictionary<Node, Node> nodeMatchup = new Dictionary<Node, Node>();
            foreach (var def in this.Definitions)
            {
                var def2 = defsMatchup[def];
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
                def2.Directives.UnionWith(def.Directives);
            }

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs);

            return grammar;
        }

        public NDefinition FindDefinitionByName(string name)
        {
            return Definitions.FirstOrDefault(def => def.Name == name);
        }
    }
}

