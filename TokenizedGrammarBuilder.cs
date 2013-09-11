using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

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
            Dictionary<string, DefinitionExpression> implicitsByLiteralValue = new Dictionary<string, DefinitionExpression>();
            Dictionary<string, DefinitionExpression> implicitsByCharClassUndelimited = new Dictionary<string, DefinitionExpression>();
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

                    foreach (var literal in def.EnumerateLiterals())
                    {
                        string defname = GetImplicitDefinitionName(literal);
                        if (!implicitsByLiteralValue.ContainsKey(defname))
                        {
                            DefinitionExpression di = new DefinitionExpression {
                                Name = defname,
                            };
                            di.Items.Add(new LiteralSubExpression{Value=literal.Value});
                            di.Directives.Add(DefinitionDirective.Token);
                            implicitsByLiteralValue[defname] = di;
                        }
                    }

                    foreach (var cc in def.EnumerateCharClasses())
                    {
                        string defname = GetImplicitDefinitionName(cc);
                        if (!implicitsByCharClassUndelimited.ContainsKey(defname))
                        {
                            DefinitionExpression di = new DefinitionExpression {
                                Name = defname,
                            };
                            di.Items.Add(new CharClassSubExpression() { CharClass = cc.CharClass });
                            di.Directives.Add(DefinitionDirective.Token);
                            implicitsByCharClassUndelimited[defname] = di;
                        }
                    }
                }
            }

            tokenizedDefs.AddRange(implicitsByLiteralValue.Values);
            tokenizedDefs.AddRange(implicitsByCharClassUndelimited.Values);

            DefinitionBuilder db = new DefinitionBuilder();
            List<DefinitionExpression> tokenizedDefsDetokenized = new List<DefinitionExpression>();
            foreach (var def in tokenizedDefs)
            {
                var def2 = new DefinitionExpression {
                    Name = def.Name,
                };
                def2.Items.AddRange(def.Items);
                def2.Directives.AddRange(def.Directives);
                def2.Directives.Add(DefinitionDirective.Atomic);
                tokenizedDefsDetokenized.Add(def2);
            }
            Definition[] tokenizedDefs2 = db.BuildDefinitions(tokenizedDefsDetokenized.ToArray());

            List<Definition> defs2 = new List<Definition>();
            Dictionary<string, Definition> defsByName = new Dictionary<string, Definition>();
            Dictionary<Definition, Expression> exprsByDef = new Dictionary<Definition, Expression>();
            foreach (DefinitionExpression di in nonTokenizedDefs)
            {
                Definition def = new Definition(di.Name);
                defs2.Add(def);
                defsByName[di.Name] = def;
                def.Directives.AddRange(di.Directives);
                exprsByDef[def] = di;
            }
            foreach (Definition def in tokenizedDefs2)
            {
                defsByName[def.Name] = def;
            }

            foreach (Definition def in defs2)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[def], defsByName);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                def.StartNodes.AddRange(bundle.StartNodes);

                def.Nodes.AddRange(bundle.Nodes);

                def.EndNodes.AddRange(bundle.EndNodes);
            }

            defs2.AddRange(tokenizedDefs2);

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs2);

            return grammar;
        }

        string GetImplicitDefinitionName(LiteralSubExpression literal)
        {
            return "$implicit literal " + literal.Value;
        }
        string GetImplicitDefinitionName(CharClassSubExpression cc)
        {
            return "$implicit char class " + cc.CharClass.ToUndelimitedString();
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
                return GetNodesFromLiteralSubExpression((LiteralSubExpression)subexpr, defsByName);
            }
            else // (subexpr is CharClassSubExpression)
            {
                return GetNodesFromCharClassSubExpression((CharClassSubExpression)subexpr, defsByName);
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

        NodeBundle GetNodesFromLiteralSubExpression(LiteralSubExpression literal, Dictionary<string, Definition> defsByName)
        {
            DefRefNode node = new DefRefNode(defsByName[GetImplicitDefinitionName(literal)], literal.Tag);
            if (literal.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new Set<Node> { node };
            bundle.EndNodes = new Set<Node> { node };
            bundle.IsSkippable = literal.IsSkippable;

            return bundle;
        }

        NodeBundle GetNodesFromCharClassSubExpression(CharClassSubExpression cc, Dictionary<string, Definition> defsByName)
        {
            DefRefNode node = new DefRefNode(defsByName[GetImplicitDefinitionName(cc)], cc.Tag);
            if (cc.IsRepeatable)
            {
                node.NextNodes.Add(node);
            }

            NodeBundle bundle = new NodeBundle();
            bundle.Nodes = new List<Node> { node };
            bundle.StartNodes = new Set<Node> { node };
            bundle.EndNodes = new Set<Node> { node };
            bundle.IsSkippable = cc.IsSkippable;

            return bundle;
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
}

