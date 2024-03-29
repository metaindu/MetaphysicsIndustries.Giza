
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
    public class GrammarCompiler
    {
        public NGrammar Compile(Grammar g)
        {
            if (g.ImportStatements != null && g.ImportStatements.Count > 0)
                throw new ArgumentException(
                    "The grammar must not contain import statements.");

            return Compile(g.Definitions);
        }
        public NGrammar Compile(IEnumerable<Definition> defs)
        {
            var defs1 = defs.ToList();
            var ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionsForSpanning(defs1);
            if (errors.ContainsNonWarnings())
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            var defs2 = new List<NDefinition>();
            var defsByName = new Dictionary<string, NDefinition>();
            var exprsByDef = new Dictionary<NDefinition, Expression>();
            foreach (var def in defs1)
            {
                var ndef = new NDefinition(def.Name);
                ndef.IsImported = def.IsImported;
                defs2.Add(ndef);
                defsByName[def.Name] = ndef;
                ndef.Directives.UnionWith(def.Directives);
                exprsByDef[ndef] = def.Expr;
            }

            foreach (var ndef in defs2)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[ndef], defsByName);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                ndef.StartNodes.UnionWith(bundle.StartNodes);

                ndef.Nodes.AddRange(bundle.Nodes);

                ndef.EndNodes.UnionWith(bundle.EndNodes);
            }

            return new NGrammar(defs2);
        }

        public NodeBundle GetNodesFromExpression(Expression expr,
            Dictionary<string, NDefinition> defsByName)
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
            var prevs = new HashSet<Node>(bundles[0].EndNodes);
            for (i = 2; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                    foreach (var prev in prevs)
                        prev.NextNodes.UnionWith(bundles[i].StartNodes);
                else
                    prevs.Clear();
                prevs.AddRange(bundles[i - 1].EndNodes);
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

        static NodeBundle GetNodesFromSubExpression(SubExpression subexpr, 
            Dictionary<string, NDefinition> defsByName)
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

        static NodeBundle GetNodesFromDefRefSubExpression(
            DefRefSubExpression defref, 
            IDictionary<string, NDefinition> defsByName)
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

        public NodeBundle GetNodesFromOrExpression(OrExpression orexpr,
            Dictionary<string, NDefinition> defsByName)
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
