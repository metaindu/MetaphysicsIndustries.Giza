
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


namespace MetaphysicsIndustries.Giza
{
    public class TokenizedGrammarBuilder
    {
        // tokenized grammars differ from non-tokenized by virtue of 
        // 'implicit token' definitions. That is, any occurence of 
        // LiteralSubExpression or CharClassSubExpression within a 
        // non-tokenized definition will be converted into a defref 
        // pointing to a new tokenized definition created to take the 
        // place of that subexpr. In this way, non-tokenized definitions
        // are composed entirely of defrefs.

        public Grammar BuildTokenizedGrammar(DefinitionExpression[] defs)
        {
            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);
            if (errors.GetNonWarningsCount() > 0)
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            // get the implicit tokens
            Dictionary<string, DefinitionExpression> implicitTokenDefs = new Dictionary<string, DefinitionExpression>();
            List<DefinitionExpression> tokenizedDefs = new List<DefinitionExpression>();
            List<DefinitionExpression> nonTokenizedDefs = new List<DefinitionExpression>();

            foreach (DefinitionExpression def in defs)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Subtoken) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    tokenizedDefs.Add(def);
                }
                else
                {
                    nonTokenizedDefs.Add(def);

                    bool ignoreCase = def.Directives.Contains(DefinitionDirective.IgnoreCase);

                    foreach (var literal in def.EnumerateLiterals())
                    {
                        string defname = GetImplicitDefinitionName(literal, ignoreCase);
                        if (!implicitTokenDefs.ContainsKey(defname))
                        {
                            DefinitionExpression di = new DefinitionExpression {
                                Name = defname,
                                IsImported = def.IsImported,
                            };
                            di.Items.Add(new LiteralSubExpression{Value=literal.Value});
                            di.Directives.Add(DefinitionDirective.Token);
                            if (ignoreCase)
                            {
                                di.Directives.Add(DefinitionDirective.IgnoreCase);
                            }
                            implicitTokenDefs[defname] = di;
                        }
                    }

                    foreach (var cc in def.EnumerateCharClasses())
                    {
                        string defname = GetImplicitDefinitionName(cc, ignoreCase);
                        if (!implicitTokenDefs.ContainsKey(defname))
                        {
                            DefinitionExpression di = new DefinitionExpression {
                                Name = defname,
                                IsImported = def.IsImported,
                            };
                            di.Items.Add(new CharClassSubExpression() { CharClass = cc.CharClass });
                            di.Directives.Add(DefinitionDirective.Token);
                            if (ignoreCase)
                            {
                                di.Directives.Add(DefinitionDirective.IgnoreCase);
                            }
                            implicitTokenDefs[defname] = di;
                        }
                    }
                }
            }

            tokenizedDefs.AddRange(implicitTokenDefs.Values);

            DefinitionBuilder db = new DefinitionBuilder();
            List<DefinitionExpression> tokenizedDefsDetokenized = new List<DefinitionExpression>();
            foreach (var def in tokenizedDefs)
            {
                var def2 = new DefinitionExpression {
                    Name = def.Name,
                    IsImported = def.IsImported,
                };
                def2.Items.AddRange(def.Items);
                def2.Directives.UnionWith(def.Directives);
                if (!def.Directives.Contains(DefinitionDirective.Subtoken))
                {
                    def2.Directives.Add(DefinitionDirective.Atomic);
                }
                def2.Directives.Add(DefinitionDirective.MindWhitespace);
                tokenizedDefsDetokenized.Add(def2);
            }

            // TODO: somehow remove the call to db.BuildDefinitions.
            // TODO: change this class into TokenizeTransform, which implements
            //       ITransform. return a collection of DefinitionExpression,
            //       instead of Definition
            Definition[] tokenizedDefs2 = db.BuildDefinitions(tokenizedDefsDetokenized.ToArray());

            List<Definition> defs2 = new List<Definition>();
            Dictionary<string, Definition> defsByName = new Dictionary<string, Definition>();
            Dictionary<Definition, DefinitionExpression> exprsByDef = new Dictionary<Definition, DefinitionExpression>();
            foreach (DefinitionExpression di in nonTokenizedDefs)
            {
                Definition def = new Definition(di.Name);
                def.IsImported = di.IsImported;
                defs2.Add(def);
                defsByName[di.Name] = def;
                def.Directives.UnionWith(di.Directives);
                def.Directives.Remove(DefinitionDirective.Atomic);
                def.Directives.Remove(DefinitionDirective.IgnoreCase);
                exprsByDef[def] = di;
            }
            foreach (Definition def in tokenizedDefs2)
            {
                defsByName[def.Name] = def;
            }

            foreach (Definition def in defs2)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[def], defsByName, exprsByDef[def]);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                def.StartNodes.UnionWith(bundle.StartNodes);

                def.Nodes.AddRange(bundle.Nodes);

                def.EndNodes.UnionWith(bundle.EndNodes);
            }

            defs2.AddRange(tokenizedDefs2);

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs2);

            return grammar;
        }

        string GetImplicitDefinitionName(LiteralSubExpression literal, bool ignoreCase)
        {

            return
                string.Format(
                    "$implicit {0}literal {1}",
                    ignoreCase ? "ignore case " : "",
                    ignoreCase ? literal.Value.ToLower() : literal.Value);
        }
        string GetImplicitDefinitionName(CharClassSubExpression cc, bool ignoreCase)
        {
            return
                string.Format(
                    "$implicit {0}char class {1}",
                    ignoreCase ? "ignore case " : "",
                    ignoreCase ? cc.CharClass.ToUndelimitedString().ToLower() : cc.CharClass.ToUndelimitedString());
        }

        NodeBundle GetNodesFromExpression(Expression expr, Dictionary<string, Definition> defsByName, DefinitionExpression parentDefinition)
        {
            NodeBundle first = null;
            NodeBundle last = null;
            List<NodeBundle> bundles = new List<NodeBundle>();
            foreach (ExpressionItem item in expr.Items)
            {
                NodeBundle bundle = null;
                if (item is SubExpression)
                {
                    bundle = GetNodesFromSubExpression((SubExpression)item, defsByName, parentDefinition);
                }
                else // (item is OrExpression)
                {
                    bundle = GetNodesFromOrExpression((OrExpression)item, defsByName, parentDefinition);
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

            HashSet<Node> starts = new HashSet<Node>();
            HashSet<Node> ends = new HashSet<Node>();
            List<Node> nodes = new List<Node>();

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

        NodeBundle GetNodesFromSubExpression(SubExpression subexpr, Dictionary<string, Definition> defsByName, DefinitionExpression parentDefinition)
        {
            if (subexpr is DefRefSubExpression)
            {
                return GetNodesFromDefRefSubExpression((DefRefSubExpression)subexpr, defsByName);
            }
            else if (subexpr is LiteralSubExpression)
            {
                return GetNodesFromLiteralSubExpression((LiteralSubExpression)subexpr, defsByName, parentDefinition);
            }
            else // (subexpr is CharClassSubExpression)
            {
                return GetNodesFromCharClassSubExpression((CharClassSubExpression)subexpr, defsByName, parentDefinition);
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
            bundle.StartNodes = new HashSet<Node> { node };
            bundle.EndNodes = new HashSet<Node> { node };
            bundle.IsSkippable = defref.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromLiteralSubExpression(LiteralSubExpression literal, Dictionary<string, Definition> defsByName, DefinitionExpression parentDefinition)
        {
            var defname = GetImplicitDefinitionName(literal, parentDefinition.Directives.Contains(DefinitionDirective.IgnoreCase));
            DefRefNode node = new DefRefNode(defsByName[defname], literal.Tag);
            if (literal.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new HashSet<Node> { node };
            bundle.EndNodes = new HashSet<Node> { node };
            bundle.IsSkippable = literal.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromCharClassSubExpression(CharClassSubExpression cc, Dictionary<string, Definition> defsByName, DefinitionExpression parentDefinition)
        {
            var defname = GetImplicitDefinitionName(cc, parentDefinition.Directives.Contains(DefinitionDirective.IgnoreCase));
            DefRefNode node = new DefRefNode(defsByName[defname], cc.Tag);
            if (cc.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new HashSet<Node> { node };
            bundle.EndNodes = new HashSet<Node> { node };
            bundle.IsSkippable = cc.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromOrExpression(OrExpression orexpr, Dictionary<string, Definition> defsByName, DefinitionExpression parentDefinition)
        {
            List<NodeBundle> bundles = new List<NodeBundle>();

            foreach (Expression expr in orexpr.Expressions)
            {
                bundles.Add(GetNodesFromExpression(expr, defsByName, parentDefinition));
            }

            HashSet<Node> starts = new HashSet<Node>();
            HashSet<Node> ends = new HashSet<Node>();
            List<Node> nodes = new List<Node>();
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
}

