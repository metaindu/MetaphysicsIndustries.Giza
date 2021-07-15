
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

/*****************************************************************************
 *                                                                           *
 *  SupergrammarSpanner.cs                                                   *
 *  6 July 2010                                                              *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2010 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;


namespace MetaphysicsIndustries.Giza
{
    public class SupergrammarSpanner
    {
        public Grammar GetGrammar(string input, List<Error> errors)
        {
            Supergrammar supergrammar = new Supergrammar();
            Spanner spanner = new Spanner(supergrammar.def_grammar);
            Span[] s2 = spanner.Process(input.ToCharacterSource(), errors);

            if (errors.Count > 0)
            {
                return null;
            }

            if (s2.Length < 1)
            {
                errors.Add(new SupergrammarSpannerError {
                    ErrorType = SupergrammarSpannerError.NoValidSpans,
                });
                return null;
            }

            if (s2.Length > 1)
            {
                errors.Add(new SupergrammarSpannerError {
                    ErrorType = SupergrammarSpannerError.MultipleValidSpans,
                    NumSpans = s2.Length,
                });
                return null;
            }

            var g = BuildPreGrammar(supergrammar, s2[0], errors);
            return g;
        }

        public Grammar BuildPreGrammar(Supergrammar grammar, Span span, List<Error> errors)
        {
            if (!(span.Node is DefRefNode) ||
                (span.Node as DefRefNode).DefRef != grammar.def_grammar)
            {
                throw new InvalidOperationException();
            }

            SpanChecker sc = new SpanChecker();
            List<Error> errors2 = sc.CheckSpan(span, grammar);
            if (errors2.Count > 0)
            {
                errors.AddRange(errors2);
                throw new InvalidOperationException("There were errors while checking the spans.");
            }

            List<Definition> defs = new List<Definition>();
            var importStmts = new List<ImportStatement>();

            foreach (Span defspan in span.Subspans)
            {
                if (defspan.Node == grammar.node_grammar_1_comment)
                {
                    continue;
                }
                if (defspan.Node == grammar.node_grammar_2_import_002D_stmt)
                {
                    var importStmt =
                        GetImportStatementFromSpan(grammar, defspan, errors);
                    importStmts.Add(importStmt);
                    continue;
                }

                if (defspan.Node != grammar.node_grammar_0_definition)
                    // TODO: add an error? throw an exception?
                    continue;

                Definition def = new Definition();
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
                        def.Expr = GetExpressionFromSpan(grammar, sub);
                    }
                }

                def.Directives.UnionWith(directives);
            }

            return new Grammar()
            {
                Definitions = defs,
                ImportStatements = importStmts,
            };
        }

        ImportStatement GetImportStatementFromSpan(
            Supergrammar importingGrammar, Span importSpan,
            List<Error> errors)
        {
            if (importSpan.Node != importingGrammar.node_grammar_2_import_002D_stmt)
                throw new ArgumentException("The span must describe an import-stmt.");
            var stmtType = importSpan.Subspans[0];
            Span source;
            // TODO: relative imports should be relative to the importing file.
            List<ImportRef> defNamesToImport = null;
            bool importAll = false;
            if (stmtType.Node == importingGrammar.node_import_002D_stmt_0_import)
            {
                source = importSpan.Subspans[6];
                importAll = true;
            }
            else if (stmtType.Node == importingGrammar.node_import_002D_stmt_7_from)
            {
                source = importSpan.Subspans[4];
                int i;
                defNamesToImport = new List<ImportRef>();
                for (i = 11; i < importSpan.Subspans.Count - 1; i += 2)
                {
                    var importRefSpan = importSpan.Subspans[i];
                    var sourceName = importRefSpan.Subspans[0].CollectValue();
                    string destName = sourceName;
                    if (importRefSpan.Subspans.Count > 1)
                        destName = importRefSpan.Subspans[3].CollectValue();
                    var importRef = new ImportRef()
                    {
                        SourceName = sourceName,
                        DestName = destName
                    };
                    defNamesToImport.Add(importRef);
                }
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        "Unknown import type, node={0}",
                        stmtType.Node));
            }

            var fileToImport = GetLiteralSubExpressionFromSpan(importingGrammar, source).Value;

            var importStmt = new ImportStatement
            {
                Filename = fileToImport,
                ImportRefs = defNamesToImport?.ToArray(),
                ImportAll = importAll,
            };

            return importStmt;
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


