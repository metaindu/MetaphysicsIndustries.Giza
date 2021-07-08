
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

using System;
using System.Collections.Generic;

using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionBuilder
    {
        public Definition[] BuildDefinitions(DefinitionExpression[] defs)
        {
            var ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForSpanning(defs);
            if (errors.ContainsNonWarnings())
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            var defs2 = new List<Definition>();
            var defsByName = new Dictionary<string, Definition>();
            var exprsByDef = new Dictionary<Definition, Expression>();
            foreach (DefinitionExpression di in defs)
            {
                var def = new Definition(di.Name);
                def.IsImported = di.IsImported;
                defs2.Add(def);
                defsByName[di.Name] = def;
                def.Directives.UnionWith(di.Directives);
                exprsByDef[def] = di;
            }

            foreach (Definition def in defs2)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[def], defsByName);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                def.StartNodes.UnionWith(bundle.StartNodes);

                def.Nodes.AddRange(bundle.Nodes);

                def.EndNodes.UnionWith(bundle.EndNodes);
            }

            return defs2.ToArray();
        }

        NodeBundle GetNodesFromExpression(Expression expr, Dictionary<string, Definition> defsByName)
        {
            NodeBundle first = null;
            NodeBundle last = null;
            var bundles = new List<NodeBundle>();
            foreach (ExpressionItem item in expr.Items)
            {
                NodeBundle bundle = null;
                if (item is SubExpression)
                {
                    bundle = GetNodesFromSubExpression((SubExpression)item, defsByName);
                }
                else // (item is OrExpression)
                {
                    bundle = GetNodesFromOrExpression((OrExpression)item, defsByName);
                }

                if (bundle != null)
                {
                    if (first == null)
                    {
                        first = bundle;
                    }

                    last = bundle;

                    bundles.Add(bundle);
                }
            }

            var starts = new HashSet<Node>();
            var ends = new HashSet<Node>();
            var nodes = new List<Node>();

            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
            }
            starts.UnionWith(first.StartNodes);
            ends.UnionWith(last.EndNodes);

            // connect the nodes
            int i;
            // inter-bundle connections
            for (i = 1; i < bundles.Count; i++)
            {
                foreach (Node prev in bundles[i-1].EndNodes)
                {
                    prev.NextNodes.UnionWith(bundles[i].StartNodes);
                }
            }

            // inter-bundle skips
            for (i = 2; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    foreach (Node prev in bundles[i-2].EndNodes)
                    {
                        prev.NextNodes.UnionWith(bundles[i].StartNodes);
                    }
                }
            }

            // skip from start to inner bundle
            bool skippable = true;
            for (i = 1; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    starts.UnionWith(bundles[i].StartNodes);
                }
                else
                {
                    skippable = false;
                    break;
                }
            }
            if (skippable)
            {
                if (bundles[bundles.Count - 1].IsSkippable)
                {
                }
                else
                {
                    skippable = false;
                }
            }

            // skip from inner bundle to end
            for (i = bundles.Count - 1; i > 0; i--)
            {
                if (bundles[i].IsSkippable)
                {
                    ends.UnionWith(bundles[i - 1].EndNodes);
                }
                else
                {
                    break;
                }
            }

            return new NodeBundle{StartNodes = starts,
                EndNodes = ends,
                Nodes = nodes,
                IsSkippable = skippable};
        }

        static NodeBundle GetNodesFromSubExpression(SubExpression subexpr, Dictionary<string, Definition> defsByName)
        {
            if (subexpr is DefRefSubExpression)
            {
                return GetNodesFromDefRefSubExpression((DefRefSubExpression)subexpr, defsByName);
            }
            else if (subexpr is LiteralSubExpression)
            {
                return GetNodesFromLiteralSubExpression((LiteralSubExpression)subexpr);
            }
            else // (subexpr is CharClassSubExpression)
            {
                return GetNodesFromCharClassSubExpression((CharClassSubExpression)subexpr);
            }
        }

        static NodeBundle GetNodesFromDefRefSubExpression(DefRefSubExpression defref, IDictionary<string, Definition> defsByName)
        {
            var node = new DefRefNode(defsByName[defref.DefinitionName], defref.Tag);
            if (defref.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            var bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new HashSet<Node> { node };
            bundle.EndNodes = new HashSet<Node> { node };
            bundle.IsSkippable = defref.IsSkippable;

            return bundle;
        }

        static NodeBundle GetNodesFromLiteralSubExpression(LiteralSubExpression literal)
        {
            var nodes = new List<Node>();

            foreach (char ch in literal.Value)
            {
                var node = new CharNode(ch, literal.Tag);
                nodes.Add(node);
            }

            int i;
            for (i = 1; i < nodes.Count; i++)
            {
                nodes[i - 1].NextNodes.Add(nodes[i]);
            }

            if (literal.IsRepeatable)
            {
                nodes.Last().NextNodes.Add(nodes[0]);
            }

            var bundle = new NodeBundle();
            bundle.Nodes = nodes;
            bundle.StartNodes = new HashSet<Node> { nodes[0] };
            bundle.EndNodes = new HashSet<Node> { nodes.Last() };
            bundle.IsSkippable = literal.IsSkippable;

            return bundle;
        }

        static NodeBundle GetNodesFromCharClassSubExpression(CharClassSubExpression cc)
        {
            var node = new CharNode(cc.CharClass, cc.Tag);

            if (cc.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            return new NodeBundle {
                    Nodes = new List<Node> { node },
                    StartNodes = new HashSet<Node> { node },
                    EndNodes = new HashSet<Node>{ node },
                    IsSkippable = cc.IsSkippable
                };
        }

        NodeBundle GetNodesFromOrExpression(OrExpression orexpr, Dictionary<string, Definition> defsByName)
        {
            var bundles = new List<NodeBundle>();

            foreach (Expression expr in orexpr.Expressions)
            {
                bundles.Add(GetNodesFromExpression(expr, defsByName));
            }

            var starts = new HashSet<Node>();
            var ends = new HashSet<Node>();
            var nodes = new List<Node>();
            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
                starts.UnionWith(bundle.StartNodes);
                ends.UnionWith(bundle.EndNodes);
            }

            if (orexpr.IsRepeatable)
            {
                foreach (Node end in ends)
                {
                    end.NextNodes.UnionWith(starts);
                }
            }

            return new NodeBundle{
                StartNodes=starts,
                EndNodes=ends,
                Nodes=nodes,
                IsSkippable=orexpr.IsSkippable
            };
        }
    }

    public class NodeBundle
    {
        public HashSet<Node> StartNodes;
        public HashSet<Node> EndNodes;
        public List<Node> Nodes;
        public bool IsSkippable;
    }

}
