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
        public DefinitionInfo[] BuildExpressions(Supergrammar grammar, Span span)
        {
            if (!(span.Node is DefRefNode) ||
                (span.Node as DefRefNode).DefRef != grammar.def_0_grammar)
            {
                throw new InvalidOperationException();
            }

            SpanChecker sc = new SpanChecker();
            List<SpanChecker.Error> errors = sc.CheckSpan(span, grammar);
            if (errors.Count > 0) throw new InvalidOperationException();

            List<DefinitionInfo> defs = new List<DefinitionInfo>();

            foreach (Span defspan in span.Subspans)
            {
                if (defspan.Node != grammar.node_grammar_0_definition) continue;

                DefinitionInfo def = new DefinitionInfo();
                defs.Add(def);

                List<DefinitionDirective> directives = new List<DefinitionDirective>();
                foreach (Span sub in defspan.Subspans)
                {
                    if (sub.Node == grammar.node_definition_1_identifier)
                    {
                        def.Name = GetIdentifier(grammar, sub);
                    }
                    else if (sub.Node == grammar.node_definition_0_directive)
                    {
                        directives.AddRange(GetDirectives(grammar, sub));
                    }
                    else if (sub.Node == grammar.node_definition_3_expr)
                    {
                        def.Expression = GetExpressionFromSpan(grammar, sub);
                    }
                }

                def.Directives = directives.ToArray();
            }

            return defs.ToArray();
        }
        public Definition[] BuildDefinitions(Supergrammar grammar, Span span)
        {
            DefinitionInfo[] dis = BuildExpressions(grammar, span);

            List<Definition> defs = new List<Definition>();
            Dictionary<string, Definition> defsByName = new Dictionary<string, Definition>();
            Dictionary<Definition, Expression> exprsByDef = new Dictionary<Definition, Expression>();
            foreach (DefinitionInfo di in dis)
            {
                Definition def = new Definition(di.Name);
                defs.Add(def);
                defsByName[di.Name] = def;
                def.Directives.AddRange(di.Directives);
                exprsByDef[def] = di.Expression;
            }

            foreach (Definition def in defs)
            {
                NodeBundle bundle = GetNodesFromExpression(exprsByDef[def], defsByName);

                if (bundle.IsSkippable) throw new InvalidOperationException();

                def.StartNodes.AddRange(bundle.StartNodes);

                def.Nodes.AddRange(bundle.Nodes);

                def.EndNodes.AddRange(bundle.EndNodes);
            }

            return defs.ToArray();
        }

        IEnumerable<DefinitionDirective> GetDirectives(Supergrammar grammar, Span span)
        {
            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_directive_1_directive_002D_item ||
                    sub.Node == grammar.node_directive_3_directive_002D_item)
                {
                    yield return GetDirectiveItem(grammar, sub);
                }
            }
        }

        DefinitionDirective GetDirectiveItem(Supergrammar grammar, Span span)
        {
            if (span.Subspans[0].Node == grammar.node_directive_item_0_id_002D_mind &&
                span.Subspans[1].Node == grammar.node_directive_item_1_id_002D_whitespace)
            {
                return DefinitionDirective.IncludeWhitespace;
            }
            if (span.Subspans[0].Node == grammar.node_directive_item_2_id_002D_ignore &&
                span.Subspans[1].Node == grammar.node_directive_item_3_id_002D_case)
            {
                return DefinitionDirective.IgnoreCase;
            }
            if (span.Subspans[0].Node == grammar.node_directive_item_4_id_002D_atomic)
            {
                return DefinitionDirective.Atomic;
            }
            if (span.Subspans[0].Node == grammar.node_directive_item_5_id_002D_token)
            {
                return DefinitionDirective.Token;
            }

            throw new InvalidOperationException();
        }

        class NodeBundle
        {
            public Set<Node> StartNodes;
            public Set<Node> EndNodes;
            public List<Node> Nodes;
            public bool IsSkippable = false;
        }

        Expression GetExpressionFromSpan(Supergrammar grammar, Span exprSpan)
        {
            Expression expr = new Expression();

            foreach (Span sub in exprSpan.Subspans)
            {
                if (sub.Node == grammar.node_expr_0_subexpr)
                {
                    expr.Items.Add(GetSubExpressionFromSpan(grammar, sub));
                }
                else if (sub.Node == grammar.node_expr_1_orexpr)
                {
                    expr.Items.Add(GetOrExpressionFromSpan(grammar, sub));
                }
            }

            return expr;
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

        SubExpression GetSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            bool skippable = false;
            bool loop = false;
            string tag = null;
            SubExpression subexpr = null;

            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_subexpr_0_identifier)
                {
                    subexpr = GetDefRefSubExpressionFromSpan(grammar, sub);
                }
                else if (sub.Node == grammar.node_subexpr_1_literal)
                {
                    subexpr = GetLiteralSubExpressionFromSpan(grammar, sub);
                }
                else if (sub.Node == grammar.node_subexpr_2_charclass)
                {
                    subexpr = GetCharClassSubExpressionFromSpan(grammar, sub);
                }
                else if (sub.Node == grammar.node_subexpr_3_modifier)
                {
                    char mod = GetModifier(grammar, sub);
                    switch (mod)
                    {
                        case '?': skippable = true; loop = false; break;
                        case '+': skippable = false; loop = true; break;
                        case '*': skippable = true; loop = true; break;
                    }
                }
                else if (sub.Node == grammar.node_subexpr_5_tag)
                {
                    tag = GetIdentifier(grammar, sub);
                }
            }

            if (tag != null)
            {
                subexpr.Tag = tag;
            }

            subexpr.IsSkippable = skippable;
            subexpr.IsRepeatable = loop;

            return subexpr;
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

        DefRefSubExpression GetDefRefSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            string ident = GetIdentifier(grammar, span);
            return new DefRefSubExpression {
                DefinitionName = ident,
                Tag = ident
            };
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

        LiteralSubExpression GetLiteralSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            List<char> chs = new List<char>();

            int i;
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];
                if (sub.Node == grammar.node_literal_0__0027_) continue;
                if (sub.Node == grammar.node_literal_5__0027_) continue;

                char ch = ' ';
                if (sub.Node == grammar.node_literal_4_unicodechar)
                {
                    ch = GetUnicodeChar(grammar, sub);
                }
                else if (sub.Node == grammar.node_literal_1__005E__005C__005C__0027_) // [^\\']
                {
                    ch = sub.Value[0];
                }
                else if (sub.Node == grammar.node_literal_2__005C_) // '\\'
                {
                    Span sub2 = span.Subspans[i+1];

                    if (sub2.Value == "r") ch = '\r';
                    else if (sub2.Value == "n") ch = '\n';
                    else if (sub2.Value == "t") ch = '\t';
                    else if (sub2.Value == "\\") ch = '\\';
                    else if (sub2.Value == "'") ch = '\'';

                    i++;
                }
                chs.Add(ch);
            }

            string value = new string(chs.ToArray());

            return new LiteralSubExpression {
                Value = value,
            };
        }
        NodeBundle GetNodesFromLiteralSubExpression(LiteralSubExpression literal)
        {
            List<Node> nodes = new List<Node>();

            foreach (char ch in literal.Value)
            {
                CharNode node = new CharNode(ch);
                node.SetTag(literal.Value);
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

        char GetUnicodeChar(Supergrammar grammar, Span span)
        {
            string s =
                span.Subspans[2].Value +
                span.Subspans[3].Value +
                span.Subspans[4].Value +
                span.Subspans[5].Value;

            int i;
            if (!int.TryParse(s, NumberStyles.HexNumber, null, out i)) throw new InvalidOperationException();

            return (char)i;
        }

        CharClassSubExpression GetCharClassSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            int i;
            List<char> items = new List<char>();
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];

                if (sub.Node == grammar.node_charclass_1__005E__005C__005C__005C__005B__005C__005D_) // [^\\\[\]]
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_2__005C_) // '\\'
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_) // [wldsrnt\\\[\]]
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_4_unicodechar)
                {
                    items.Add(GetUnicodeChar(grammar, sub));
                }
            }

            string charClassText = new string(items.ToArray());
            CharClass cc = CharClass.FromUndelimitedCharClassText(charClassText);

            return new CharClassSubExpression {
                CharClass = cc,
                Tag = cc.ToUndelimitedString(),
            };
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

        char GetModifier(Supergrammar grammar, Span span)
        {
            return span.Subspans[0].Value[0];
        }

        string GetIdentifier(Supergrammar grammar, Span span)
        {
            List<char> chs = new List<char>();
            foreach (Span sub in span.Subspans)
            {
                chs.Add(sub.Value[0]);
            }
            return new string(chs.ToArray());
        }

        OrExpression GetOrExpressionFromSpan(Supergrammar grammar, Span span)
        {
            bool skippable = false;
            bool loopable = false;
            OrExpression orexpr = new OrExpression();

            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_orexpr_1_expr ||
                    sub.Node == grammar.node_orexpr_3_expr)
                {
                    orexpr.Expressions.Add(GetExpressionFromSpan(grammar, sub));
                }
                else if (sub.Node == grammar.node_orexpr_5_modifier)
                {
                    switch (GetModifier(grammar, sub))
                    {
                        case '*': loopable = true; skippable = true; break;
                        case '?': skippable = true; break;
                        case '+': loopable = true; break;
                    }
                }
            }

            orexpr.IsSkippable = skippable;
            orexpr.IsRepeatable = loopable;

            return orexpr;
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
