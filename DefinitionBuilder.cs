using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionBuilder
    {
        public Definition[] BuildDefinitions(DefinitionInfo[] defs)
        {
            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            List<Definition> defs2 = new List<Definition>();
            Dictionary<string, Definition> defsByName = new Dictionary<string, Definition>();
            Dictionary<Definition, Expression> exprsByDef = new Dictionary<Definition, Expression>();
            foreach (DefinitionInfo di in defs)
            {
                Definition def = new Definition(di.Name);
                defs2.Add(def);
                defsByName[di.Name] = def;
                def.Directives.AddRange(di.Directives);
                exprsByDef[def] = di;
            }

            foreach (Definition def in defs2)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[def], defsByName);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                def.StartNodes.AddRange(bundle.StartNodes);

                def.Nodes.AddRange(bundle.Nodes);

                def.EndNodes.AddRange(bundle.EndNodes);
            }

            return defs2.ToArray();
        }

        NodeBundle GetNodesFromExpression(Expression expr, Dictionary<string, Definition> defsByName)
        {
            NodeBundle first = null;
            NodeBundle last = null;
            List<NodeBundle> bundles = new List<NodeBundle>();
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

            Set<Node> starts = new Set<Node>();
            Set<Node> ends = new Set<Node>();
            List<Node> nodes = new List<Node>();

            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
            }
            starts.AddRange(first.StartNodes);
            ends.AddRange(last.EndNodes);

            // connect the nodes
            int i;
            // inter-bundle connections
            for (i = 1; i < bundles.Count; i++)
            {
                foreach (Node prev in bundles[i-1].EndNodes)
                {
                    prev.NextNodes.AddRange(bundles[i].StartNodes);
                }
            }

            // inter-bundle skips
            for (i = 2; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    foreach (Node prev in bundles[i-2].EndNodes)
                    {
                        prev.NextNodes.AddRange(bundles[i].StartNodes);
                    }
                }
            }

            // skip from start to inner bundle
            bool skippable = true;
            for (i = 1; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    starts.AddRange(bundles[i].StartNodes);
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
                    ends.AddRange(bundles[i - 1].EndNodes);
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

        NodeBundle GetNodesFromSubExpression(SubExpression subexpr, Dictionary<string, Definition> defsByName)
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

        NodeBundle GetNodesFromDefRefSubExpression(DefRefSubExpression defref, Dictionary<string, Definition> defsByName)
        {
            DefRefNode node = new DefRefNode(defsByName[defref.DefinitionName], defref.Tag);
            if (defref.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new Set<Node> { node };
            bundle.EndNodes = new Set<Node> { node };
            bundle.IsSkippable = defref.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromLiteralSubExpression(LiteralSubExpression literal)
        {
            List<Node> nodes = new List<Node>();

            foreach (char ch in literal.Value)
            {
                CharNode node = new CharNode(ch, literal.Tag);
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

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = nodes;
            bundle.StartNodes = new Set<Node> { nodes[0] };
            bundle.EndNodes = new Set<Node> { nodes.Last() };
            bundle.IsSkippable = literal.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromCharClassSubExpression(CharClassSubExpression cc)
        {
            CharNode node = new CharNode(cc.CharClass, cc.Tag);

            if (cc.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            return new NodeBundle {
                    Nodes = new List<Node> { node },
                    StartNodes = new Set<Node> { node },
                    EndNodes = new Set<Node>{ node },
                    IsSkippable = cc.IsSkippable
                };
        }

        NodeBundle GetNodesFromOrExpression(OrExpression orexpr, Dictionary<string, Definition> defsByName)
        {
            List<NodeBundle> bundles = new List<NodeBundle>();

            foreach (Expression expr in orexpr.Expressions)
            {
                bundles.Add(GetNodesFromExpression(expr, defsByName));
            }

            Set<Node> starts = new Set<Node>();
            Set<Node> ends = new Set<Node>();
            List<Node> nodes = new List<Node>();
            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
                starts.AddRange(bundle.StartNodes);
                ends.AddRange(bundle.EndNodes);
            }

            if (orexpr.IsRepeatable)
            {
                foreach (Node end in ends)
                {
                    end.NextNodes.AddRange(starts);
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
        public Set<Node> StartNodes;
        public Set<Node> EndNodes;
        public List<Node> Nodes;
        public bool IsSkippable = false;
    }

}
