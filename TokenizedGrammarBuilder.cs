
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
    public class TokenizedGrammarBuilder : IPreGrammarTransform
    {
        // tokenized grammars differ from non-tokenized by virtue of
        // 'implicit token' definitions. That is, any occurence of
        // LiteralSubExpression or CharClassSubExpression within a
        // non-tokenized definition will be converted into a defref
        // pointing to a new tokenized definition created to take the
        // place of that subexpr. In this way, non-tokenized definitions
        // are composed entirely of defrefs.

        public Grammar BuildTokenizedGrammar(PreGrammar pg)
        {
            var pg2 = Tokenize(pg);
            var db = new DefinitionBuilder();
            var defs2 = db.BuildDefinitions(pg2.Definitions);
            var grammar = new Grammar();
            grammar.Definitions.AddRange(defs2);

            return grammar;
        }

        public PreGrammar Transform(PreGrammar pg) => Tokenize(pg);

        public PreGrammar Tokenize(PreGrammar pg)
        {
            var defs = pg.Definitions;
            var ec = new ExpressionChecker();
            var errors = ec.CheckDefinitionForParsing(defs);
            if (errors.GetNonWarningsCount() > 0)
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            // get the implicit tokens
            var implicitTokenDefs =
                new Dictionary<string, DefinitionExpression>();
            var tokenizedDefs = new List<DefinitionExpression>();
            var nonTokenizedDefs = new List<DefinitionExpression>();

            foreach (var def in defs)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Subtoken) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    tokenizedDefs.Add(def);
                }
                else
                {
                    var ignoreCase = def.Directives.Contains(
                        DefinitionDirective.IgnoreCase);
                    var defsByLiteral =
                        new Dictionary<LiteralSubExpression,
                            DefinitionExpression>();
                    var defsByCharClass =
                        new Dictionary<CharClassSubExpression,
                            DefinitionExpression>();

                    foreach (var literal in def.EnumerateLiterals())
                    {
                        var defname = GetImplicitDefinitionName(literal, ignoreCase);
                        if (!implicitTokenDefs.ContainsKey(defname))
                        {
                            var di = new DefinitionExpression
                            {
                                Name = defname,
                                IsImported = def.IsImported,
                            };
                            di.Items.Add(new LiteralSubExpression {Value = literal.Value});
                            di.Directives.Add(DefinitionDirective.Token);
                            if (ignoreCase)
                            {
                                di.Directives.Add(DefinitionDirective.IgnoreCase);
                            }

                            implicitTokenDefs[defname] = di;
                        }
                        defsByLiteral[literal] = implicitTokenDefs[defname];
                    }

                    foreach (var cc in def.EnumerateCharClasses())
                    {
                        var defname = GetImplicitDefinitionName(cc, ignoreCase);
                        if (!implicitTokenDefs.ContainsKey(defname))
                        {
                            var di = new DefinitionExpression
                            {
                                Name = defname,
                                IsImported = def.IsImported,
                            };
                            di.Items.Add(new CharClassSubExpression() {CharClass = cc.CharClass});
                            di.Directives.Add(DefinitionDirective.Token);
                            if (ignoreCase)
                            {
                                di.Directives.Add(DefinitionDirective.IgnoreCase);
                            }

                            implicitTokenDefs[defname] = di;
                        }
                        defsByCharClass[cc] = implicitTokenDefs[defname];
                    }

                    nonTokenizedDefs.Add(
                        ReplaceInDefintionExpression(def, defsByLiteral,
                            defsByCharClass));
                }
            }

            tokenizedDefs.AddRange(implicitTokenDefs.Values);

            foreach (var def in tokenizedDefs)
            {
                if (!def.Directives.Contains(DefinitionDirective.Subtoken))
                    def.Directives.Add(DefinitionDirective.Atomic);
                def.Directives.Add(DefinitionDirective.MindWhitespace);
            }

            foreach (var def in nonTokenizedDefs)
            {
                def.Directives.Remove(DefinitionDirective.Atomic);
                def.Directives.Remove(DefinitionDirective.IgnoreCase);
            }

            var outdefs = new List<DefinitionExpression>();
            outdefs.AddRange(nonTokenizedDefs);
            outdefs.AddRange(tokenizedDefs);
            return new PreGrammar() {Definitions = outdefs};
        }

        string GetImplicitDefinitionName(LiteralSubExpression literal, bool ignoreCase)
        {
            return string.Format(
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

        public DefinitionExpression ReplaceInDefintionExpression(
            DefinitionExpression def,
            Dictionary<LiteralSubExpression, DefinitionExpression> defsByLiteral,
            Dictionary<CharClassSubExpression, DefinitionExpression> defsByCharClass)
        {
            return new DefinitionExpression(def.Name, def.Directives,
                def.Items.Select(item => ReplaceInExpressionItem(
                    item, defsByLiteral, defsByCharClass)));
        }
        public Expression ReplaceInExpression(Expression expr,
            Dictionary<LiteralSubExpression, DefinitionExpression> defsByLiteral,
            Dictionary<CharClassSubExpression, DefinitionExpression> defsByCharClass)
        {
            return new Expression(
                expr.Items.Select(item => ReplaceInExpressionItem(
                    item, defsByLiteral, defsByCharClass)));
        }

        public ExpressionItem ReplaceInExpressionItem(ExpressionItem item,
            Dictionary<LiteralSubExpression, DefinitionExpression> defsByLiteral,
            Dictionary<CharClassSubExpression, DefinitionExpression> defsByCharClass)
        {
            if (item is OrExpression orexpr)
                return new OrExpression(
                    orexpr.Expressions.Select(
                        expr => ReplaceInExpression(expr, defsByLiteral,
                            defsByCharClass)),
                    orexpr.IsSkippable, orexpr.IsRepeatable);
            if (item is DefRefSubExpression defref)
                return new DefRefSubExpression(defref.DefinitionName,
                    defref.Tag, defref.IsSkippable, defref.IsRepeatable);
            if (item is LiteralSubExpression literal)
            {
                var def = defsByLiteral[literal];
                return new DefRefSubExpression(def.Name, literal.Tag,
                    literal.IsSkippable, literal.IsRepeatable);
            }

            if (item is CharClassSubExpression cc)
            {
                var def = defsByCharClass[cc];
                return new DefRefSubExpression(def.Name, cc.Tag,
                    cc.IsSkippable, cc.IsRepeatable);
            }

            throw new ArgumentOutOfRangeException(
                $"Unknown type of 'item': {item.GetType().FullName}");
        }
    }
}

