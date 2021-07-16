
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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

using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class GrammarComparer
    {
        public struct Discrepancy
        {
            public NDefinition DefinitionA;
            public NDefinition DefinitionB;
            public Node NodeA;
            public Node NodeB;

            public DiscrepancyType DiscrepancyType;
        }

        public enum DiscrepancyType
        {
            DefinitionNamesAreDifferent,
            DefinitionDirectivesAreDifferent,
            DefinitionNodeCountsAreDifferent,
            DefinitionStartNodesAreDifferent,
            DefinitionEndNodesAreDifferent,
            NodesHaveDifferentTypes,
            NodeCharClassesAreDifferent,
            NodeDefRefsAreDifferent,
            NodeTagsAreDifferent,
            NodeNextLinksAreDifferent,
        }

//        public bool AreEquivalent(NGrammar a, NGrammar b)
//        {
//            if (a.Definitions.Count != b.Definitions.Count) return false;
//
//            Dictionary<NDefinition, NDefinition> defMatchup =
//                new Dictionary<NDefinition, NDefinition>();
//            int i;
//            for (i = 0; i < a.Definitions.Count; i++)
//            {
//                defMatchup[a.Definitions[i]] = b.Definitions[i];
//                defMatchup[b.Definitions[i]] = a.Definitions[i];
//            }
//
//            for (i = 0; i < a.Definitions.Count; i++)
//            {
//                if (!AreEquivalent(a.Definitions[i], b.Definitions[i], defMatchup)) return false;
//            }
//
//            return true;
//        }

        public Discrepancy[] AreEquivalent(NDefinition a, NDefinition b, 
            Dictionary<NDefinition, NDefinition> defmatch)
        {
            List<Discrepancy> discrepancies = new List<Discrepancy>();

            if (a.Name != b.Name)
            {
                discrepancies.Add(new Discrepancy{
                    DefinitionA = a,
                    DefinitionB = b,
                    DiscrepancyType = DiscrepancyType.DefinitionNamesAreDifferent,
                });
            }

            if (a.Directives.Count != b.Directives.Count)
            {
                discrepancies.Add(new Discrepancy{
                    DefinitionA = a,
                    DefinitionB = b,
                    DiscrepancyType = DiscrepancyType.DefinitionDirectivesAreDifferent,
                });
            }
            else
            {
                foreach (DefinitionDirective dir in a.Directives)
                {
                    if (!b.Directives.Contains(dir))
                    {
                        discrepancies.Add(new Discrepancy{
                            DefinitionA = a,
                            DefinitionB = b,
                            DiscrepancyType = DiscrepancyType.DefinitionDirectivesAreDifferent,
                        });
                        break;
                    }
                }
            }

            if (a.Nodes.Count != b.Nodes.Count)
            {
                discrepancies.Add(new Discrepancy{
                    DefinitionA = a,
                    DefinitionB = b,
                    DiscrepancyType = DiscrepancyType.DefinitionNodeCountsAreDifferent,
                });
            }
            int i;
            Dictionary<Node, Node> nodematchup = new Dictionary<Node, Node>();
            for (i = 0; i < Math.Min(a.Nodes.Count, b.Nodes.Count); i++)
            {
                nodematchup[a.Nodes[i]] = b.Nodes[i];
                nodematchup[b.Nodes[i]] = a.Nodes[i];
            }
            for (i = 0; i < Math.Min(a.Nodes.Count, b.Nodes.Count); i++)
            {
                discrepancies.AddRange(AreEquivalent(a.Nodes[i], b.Nodes[i], defmatch, nodematchup));
            }

            if (a.StartNodes.Count != b.StartNodes.Count)
            {
                discrepancies.Add(new Discrepancy{
                    DefinitionA = a,
                    DefinitionB = b,
                    DiscrepancyType = DiscrepancyType.DefinitionStartNodesAreDifferent,
                });
            }
            else
            {
                foreach (Node n in a.StartNodes)
                {
                    if (!b.StartNodes.Contains(nodematchup[n]))
                    {
                        discrepancies.Add(new Discrepancy{
                            DefinitionA = a,
                            DefinitionB = b,
                            DiscrepancyType = DiscrepancyType.DefinitionStartNodesAreDifferent,
                        });
                        break;
                    }
                }
            }

            if (a.EndNodes.Count != b.EndNodes.Count)
            {

                discrepancies.Add(new Discrepancy{
                    DefinitionA = a,
                    DefinitionB = b,
                    DiscrepancyType = DiscrepancyType.DefinitionEndNodesAreDifferent,
                });
            }
            else
            {
                foreach (Node n in a.EndNodes)
                {
                    if (!b.EndNodes.Contains(nodematchup[n]))
                    {
                        discrepancies.Add(new Discrepancy{
                            DefinitionA = a,
                            DefinitionB = b,
                            DiscrepancyType = DiscrepancyType.DefinitionEndNodesAreDifferent,
                        });
                        break;
                    }
                }
            }

            return discrepancies.ToArray();
        }

        public Discrepancy[] AreEquivalent(Node a, Node b, 
            Dictionary<NDefinition, NDefinition> defmatchup, 
            Dictionary<Node, Node> nodematchup)
        {
            List<Discrepancy> discrepancies = new List<Discrepancy>();

            if (a.GetType() != b.GetType())
            {
                discrepancies.Add(new Discrepancy{
                    NodeA = a,
                    NodeB = b,
                    DiscrepancyType = DiscrepancyType.NodesHaveDifferentTypes,
                });
            }
            else
            {
                if (a is CharNode)
                {
                    CharNode aa = (CharNode)a;
                    CharNode bb = (CharNode)b;

                    if (aa.CharClass.ToUndelimitedString() != bb.CharClass.ToUndelimitedString())
                    {
                        discrepancies.Add(new Discrepancy{
                            NodeA = a,
                            NodeB = b,
                            DiscrepancyType = DiscrepancyType.NodeCharClassesAreDifferent,
                        });
                    }
                }
                else // (a is DefRefNode)
                {
                    DefRefNode aa = (DefRefNode)a;
                    DefRefNode bb = (DefRefNode)b;

                    if (aa.DefRef != defmatchup[bb.DefRef])
                    {
                        discrepancies.Add(new Discrepancy{
                            NodeA = a,
                            NodeB = b,
                            DiscrepancyType = DiscrepancyType.NodeDefRefsAreDifferent,
                        });
                    }
                }
            }

            if (a.Tag != b.Tag)
            {
                discrepancies.Add(new Discrepancy{
                    NodeA = a,
                    NodeB = b,
                    DiscrepancyType = DiscrepancyType.NodeTagsAreDifferent,
                });
            }

            if (a.NextNodes.Count != b.NextNodes.Count)
            {
                discrepancies.Add(new Discrepancy{
                    NodeA = a,
                    NodeB = b,
                    DiscrepancyType = DiscrepancyType.NodeNextLinksAreDifferent,
                });
            }
            else
            {
                foreach (Node next in a.NextNodes)
                {
                    if (!b.NextNodes.Contains(nodematchup[next]))
                    {
                        discrepancies.Add(new Discrepancy{
                            NodeA = a,
                            NodeB = b,
                            DiscrepancyType = DiscrepancyType.NodeNextLinksAreDifferent,
                        });
                        break;
                    }
                }
            }

            return discrepancies.ToArray();
        }
    }
}

