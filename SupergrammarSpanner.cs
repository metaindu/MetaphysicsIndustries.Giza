
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
using System.IO;
using System.Linq;


namespace MetaphysicsIndustries.Giza
{
    public class SupergrammarSpanner
    {
        public SupergrammarSpanner()
            : this(new FileSource())
        {
        }

        public SupergrammarSpanner(IFileSource fileSource)
        {
            _fileSource = fileSource;
        }

        private readonly IFileSource _fileSource;

        public DefinitionExpression[] GetExpressions(string input, List<Error> errors)
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

            DefinitionExpression[] dis = BuildExpressions(supergrammar, s2[0], errors);
            return dis;
        }

        public Grammar GetGrammar(string input, List<Error> errors)
        {
            DefinitionExpression[] dis = GetExpressions(input, errors);

            if (errors.Count > 0 || dis == null)
            {
                return null;
            }

            DefinitionBuilder db = new DefinitionBuilder();
            Definition[] defs = db.BuildDefinitions(dis);

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs);

            return grammar;
        }
        
        public DefinitionExpression[] BuildExpressions(Supergrammar grammar, Span span, List<Error> errors)
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

            List<DefinitionExpression> defs = new List<DefinitionExpression>();

            foreach (Span defspan in span.Subspans)
            {
                if (defspan.Node == grammar.node_grammar_1_comment)
                {
                    continue;
                }
                if (defspan.Node == grammar.node_grammar_2_import_002D_stmt)
                {
                    var importedDefs = ImportDefinitionsFromFile(grammar, defspan, errors);
                    var importedDefNames = new HashSet<string>(
                        importedDefs.Select(d => d.Name));
                    foreach (var def1 in defs.ToArray())
                    {
                        if (importedDefNames.Contains(def1.Name))
                            defs.Remove(def1);
                    }
                    defs.AddRange(importedDefs);
                    continue;
                }

                if (defspan.Node != grammar.node_grammar_0_definition)
                    // TODO: add an error? throw an exception?
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

                def.Directives.UnionWith(directives);
            }

            return defs.ToArray();
        }

        private Dictionary<string, Dictionary<string, DefinitionExpression>> _importCache =
            new Dictionary<string, Dictionary<string, DefinitionExpression>>();
        DefinitionExpression[] ImportDefinitionsFromFile(
            Supergrammar importingGrammar, Span defspan, List<Error> errors)
        {
            if (defspan.Node != importingGrammar.node_grammar_2_import_002D_stmt)
                throw new ArgumentException("The span must describe an import-stmt.");
            var stmtType = defspan.Subspans[0];
            Span source;
            // TODO: relative imports should be relative to the importing file.
            List<Tuple<string, string>> defNamesToImport = null;
            IEnumerable<DefinitionExpression> defsToImport;
            if (stmtType.Node == importingGrammar.node_import_002D_stmt_0_import)
            {
                source = defspan.Subspans[6];
            }
            else if (stmtType.Node == importingGrammar.node_import_002D_stmt_7_from)
            {
                source = defspan.Subspans[4];
                int i;
                defNamesToImport = new List<Tuple<string, string>>();
                for (i = 11; i < defspan.Subspans.Count - 1; i += 2)
                {
                    var importRef = defspan.Subspans[i];
                    var sourceName = importRef.Subspans[0].CollectValue();
                    string destName = sourceName;
                    if (importRef.Subspans.Count > 1)
                        destName = importRef.Subspans[3].CollectValue();
                    defNamesToImport.Add(new Tuple<string, string>(sourceName, destName));
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
            if (!_importCache.ContainsKey(fileToImport))
            {
                var content = _fileSource.GetFileContents(fileToImport);
                var errors2 = new List<Error>();
                var importedDefs = GetExpressions(content, errors2);
                if (errors2.Count > 0)
                {
                    errors.AddRange(errors2);
                    return null;
                }

                var importedGrammar = GetGrammar(content, errors2);
                if (errors2.Count > 0)
                {
                    errors.AddRange(errors2);
                    return null;
                }

                var importedDefsByName1 =
                    importedDefs.ToDictionary(
                        d => d.Name,
                        d => d);
                _importCache[fileToImport] = importedDefsByName1;
            }

            var importedDefsByName = _importCache[fileToImport];

            if (defNamesToImport != null)
            {
                var defsToImport1 = new List<DefinitionExpression>();
                defsToImport = defsToImport1;
                foreach (var namePair in defNamesToImport)
                {
                    var sourceName = namePair.Item1;
                    var destName = namePair.Item2;
                    if (!importedDefsByName.ContainsKey(sourceName))
                        errors.Add(new ImportError
                        {
                            ErrorType = ImportError.DefinitionNotFound,
                            DefinitionName = sourceName
                        });
                    else
                    {
                        var sourceDef = importedDefsByName[sourceName];
                        var destDef = new DefinitionExpression(destName,
                            sourceDef.Directives, sourceDef.Items);
                        defsToImport1.Add(destDef);
                    }
                }
            }
            else
            {
                defsToImport = importedDefsByName.Values;
            }

            return defsToImport.ToArray();
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


