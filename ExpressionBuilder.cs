using System;
using System.Collections.Generic;
using System.Globalization;

namespace MetaphysicsIndustries.Giza
{
    public class ExpressionBuilder
    {
        public DefinitionExpression[] BuildExpressions(Supergrammar grammar, Span span)
        {
            if (!(span.Node is DefRefNode) ||
                (span.Node as DefRefNode).DefRef != grammar.def_0_grammar)
            {
                throw new InvalidOperationException();
            }

            SpanChecker sc = new SpanChecker();
            List<Error> errors = sc.CheckSpan(span, grammar);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException();
            }

            List<DefinitionExpression> defs = new List<DefinitionExpression>();

            foreach (Span defspan in span.Subspans)
            {
                if (defspan.Node != grammar.node_grammar_0_definition)
                    continue;

                DefinitionExpression def = new DefinitionExpression();
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
                        var expr = GetExpressionFromSpan(grammar, sub);
                        def.Items.AddRange(expr.Items);
                        expr.Items.Clear();
                    }
                }

                def.Directives.AddRange(directives);
            }

            return defs.ToArray();
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
                    case '?':
                        skippable = true;
                        loop = false;
                        break;
                    case '+':
                        skippable = false;
                        loop = true;
                        break;
                    case '*':
                        skippable = true;
                        loop = true;
                        break;
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

        DefRefSubExpression GetDefRefSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            string ident = GetIdentifier(grammar, span);
            return new DefRefSubExpression {
                DefinitionName = ident,
                Tag = ident
            };
        }

        LiteralSubExpression GetLiteralSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            List<char> chs = new List<char>();

            int i;
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];
                if (sub.Node == grammar.node_literal_0__0027_)
                    continue;
                if (sub.Node == grammar.node_literal_5__0027_)
                    continue;

                char ch = ' ';
                if (sub.Node == grammar.node_literal_4_unicodechar)
                {
                    ch = GetUnicodeChar(grammar, sub);
                }
                else if (sub.Node == grammar.node_literal_1__005E__0027__005C__005C_) // [^\\']
                {
                    ch = sub.Value[0];
                }
                else if (sub.Node == grammar.node_literal_2__005C_) // '\\'
                {
                    Span sub2 = span.Subspans[i+1];

                    if (sub2.Value == "r")
                        ch = '\r';
                    else if (sub2.Value == "n")
                        ch = '\n';
                    else if (sub2.Value == "t")
                        ch = '\t';
                    else if (sub2.Value == "\\")
                        ch = '\\';
                    else if (sub2.Value == "'")
                        ch = '\'';

                    i++;
                }
                chs.Add(ch);
            }

            string value = new string(chs.ToArray());

            return new LiteralSubExpression {
                Value = value,
                Tag = value,
            };
        }

        CharClassSubExpression GetCharClassSubExpressionFromSpan(Supergrammar grammar, Span span)
        {
            int i;
            List<char> items = new List<char>();
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];

                if (sub.Node == grammar.node_charclass_1__005E__005C__005B__005C__005C__005C__005D_) // [^\\\[\]]
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_2__005C_) // '\\'
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_3__005C__005B__005C__005C__005C__005D_dlnrstw) // [wldsrnt\\\[\]]
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
                    case '*':
                        loopable = true;
                        skippable = true;
                        break;
                    case '?':
                        skippable = true;
                        break;
                    case '+':
                        loopable = true;
                        break;
                    }
                }
            }

            orexpr.IsSkippable = skippable;
            orexpr.IsRepeatable = loopable;

            return orexpr;
        }

        char GetUnicodeChar(Supergrammar grammar, Span span)
        {
            string s =
                span.Subspans[2].Value +
                span.Subspans[3].Value +
                span.Subspans[4].Value +
                span.Subspans[5].Value;

            int i;
            if (!int.TryParse(s, NumberStyles.HexNumber, null, out i))
                throw new InvalidOperationException();

            return (char)i;
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
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_0_id_002D_mind &&
                span.Subspans[1].Node == grammar.node_directive_002D_item_1_id_002D_whitespace)
            {
                return DefinitionDirective.MindWhitespace;
            }
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_2_id_002D_ignore &&
                span.Subspans[1].Node == grammar.node_directive_002D_item_3_id_002D_case)
            {
                return DefinitionDirective.IgnoreCase;
            }
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_4_id_002D_atomic)
            {
                return DefinitionDirective.Atomic;
            }
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_5_id_002D_token)
            {
                return DefinitionDirective.Token;
            }
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_6_id_002D_subtoken)
            {
                return DefinitionDirective.Subtoken;
            }
            if (span.Subspans[0].Node == grammar.node_directive_002D_item_7_id_002D_comment)
            {
                return DefinitionDirective.Comment;
            }

            throw new InvalidOperationException();
        }
    }
}

