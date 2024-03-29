﻿
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
using NUnit.Framework;
using System.Linq;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class TokenizeTransformTest
    {
        [Test]
        public void TestImplicitLiteral()
        {
            // setup
            //def = 'value';
            var dis = new[]
            {
                new Definition(
                    name: "def",
                    expr: new Expression(
                        new LiteralSubExpression(value: "value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit literal value");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(explicitDef);
            Assert.That(explicitDef.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.That(explicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.That(defref.DefinitionName, Is.EqualTo(implicitDef.Name));

            Assert.IsNotNull(implicitDef);
            Assert.That(implicitDef.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.That(implicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(implicitDef.Expr.Items[0]);
            var literal = (LiteralSubExpression) implicitDef.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestImplicitCharClass()
        {
            // setup
            //"def = [\\d];
            var dis = new [] {
                new Definition(
                    name: "def",
                    expr: new Expression(
                        new CharClassSubExpression(
                            CharClass.FromUndelimitedCharClassText("\\d"))))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit char class \\d");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(explicitDef);
            Assert.That(explicitDef.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.That(explicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.That(defref.DefinitionName, Is.EqualTo(implicitDef.Name));

            Assert.IsNotNull(implicitDef);
            Assert.That(implicitDef.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.That(implicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<CharClassSubExpression>(implicitDef.Expr.Items[0]);
            var cc = (CharClassSubExpression) implicitDef.Expr.Items[0];
            Assert.That(cc.CharClass.ToUndelimitedString(), Is.EqualTo("\\d"));
            Assert.That(cc.Tag, Is.EqualTo(""));
            Assert.IsFalse(cc.IsSkippable);
            Assert.IsFalse(cc.IsRepeatable);
        }

        [Test]
        public void TestImplicitIgnoreCaseLiteral()
        {
            // setup
            //<ignore case> def = 'value';
            var dis = new [] {
                new Definition(
                    name: "def",
                    expr: new Expression(
                        new LiteralSubExpression(value: "value")),
                    directives: new [] {
                        DefinitionDirective.IgnoreCase
                    }
                )
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit ignore case literal value");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(explicitDef);
            Assert.That(explicitDef.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.That(explicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.That(defref.DefinitionName, Is.EqualTo(implicitDef.Name));

            Assert.IsNotNull(implicitDef);
            Assert.That(implicitDef.Directives.Count, Is.EqualTo(4));
            Assert.That(implicitDef.Directives.ToArray(),
                Does.Contain(DefinitionDirective.Token));
            Assert.That(implicitDef.Directives.ToArray(),
                Does.Contain(DefinitionDirective.Atomic));
            Assert.That(implicitDef.Directives.ToArray(),
                Does.Contain(DefinitionDirective.IgnoreCase));
            Assert.That(implicitDef.Directives.ToArray(),
                Does.Contain(DefinitionDirective.MindWhitespace));
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsTrue(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.That(implicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(implicitDef.Expr.Items[0]);
            var literal = (LiteralSubExpression) implicitDef.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestImplicitIgnoreCaseCharClass()
        {
            // setup
            //<ignore case> def = [\\d];

            var dis = new [] {
                new Definition(
                    name: "def",
                    expr: new Expression(
                        new CharClassSubExpression(
                            CharClass.FromUndelimitedCharClassText("\\d"))),
                    directives: new [] {
                        DefinitionDirective.IgnoreCase
                    }
                )
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit ignore case char class \\d");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(explicitDef);
            Assert.That(explicitDef.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);

            Assert.IsNotNull(implicitDef);
            Assert.That(implicitDef.Directives.Count, Is.EqualTo(4));
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.IgnoreCase, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsTrue(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.That(implicitDef.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<CharClassSubExpression>(implicitDef.Expr.Items[0]);
            var cc = (CharClassSubExpression) implicitDef.Expr.Items[0];
            Assert.That(cc.CharClass.ToUndelimitedString(), Is.EqualTo("\\d"));
            Assert.That(cc.Tag, Is.EqualTo(""));
            Assert.IsFalse(cc.IsSkippable);
            Assert.IsFalse(cc.IsRepeatable);
        }

        [Test]
        public void TestNonTokenWithoutDirectives()
        {
            // setup
            //def = token;
            //<token> token = 'token';
            var dis = new [] {
                new Definition(
                    name: "def",
                    expr: new Expression(
                        new DefRefSubExpression("token"))),
                new Definition(
                    name: "token",
                    expr: new Expression(
                        new LiteralSubExpression("token")),
                    directives: new [] {
                        DefinitionDirective.Token
                    }
                )
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("def");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsFalse(def.IsTokenized);
            Assert.IsFalse(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(def.Expr.Items[0]);
            var defref = (DefRefSubExpression) def.Expr.Items[0];
            Assert.That(defref.DefinitionName, Is.EqualTo("token"));
        }

        [Test]
        public void TestTokenDirective()
        {
            // setup
            //<token> something = 'value';
            var dis = new [] {
                new Definition(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Token
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Token, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestSubtokenDirective()
        {
            // setup
            //<subtoken> something = 'value';
            var dis = new[]
            {
                new Definition(
                    name: "something",
                    directives: new[]
                    {
                        DefinitionDirective.Subtoken
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(2));
            Assert.Contains(DefinitionDirective.Subtoken, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestCommentDirective()
        {
            // setup
            //<comment> something = 'value';
            var dis = new [] {
                new Definition(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Comment
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value"))
                )
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Comment, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsTrue(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestAtomicDirectiveInNonToken()
        {
            // setup
            //<atomic> def = token;
            //<token> token = 'token';
            var dis = new [] {
                new Definition(
                    name: "def",
                    directives: new [] {
                        DefinitionDirective.Atomic,
                    },
                    expr: new Expression(
                        new DefRefSubExpression("token"))
                ),
                new Definition(
                    name: "token",
                    directives: new [] {
                        DefinitionDirective.Token,
                    },
                    expr: new Expression(
                        new LiteralSubExpression("token")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("def");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(2));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(0));
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsFalse(def.IsTokenized);
            Assert.IsFalse(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(def.Expr.Items[0]);
            var defref = (DefRefSubExpression) def.Expr.Items[0];
            Assert.That(defref.DefinitionName, Is.EqualTo("token"));
            Assert.That(defref.Tag, Is.EqualTo(""));
            Assert.IsFalse(defref.IsSkippable);
            Assert.IsFalse(defref.IsRepeatable);
        }

        [Test]
        public void TestAtomicDirectiveInToken()
        {
            // setup
            //<token, atomic> something = 'value';
            var dis = new[]
            {
                new Definition(
                    name: "something",
                    directives: new[]
                    {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Token, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestAtomicDirectiveInSubtoken()
        {
            // setup
            //<subtoken, atomic> something = 'value';
            var dis = new[]
            {
                new Definition(
                    name: "something",
                    directives: new[]
                    {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.Atomic
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Subtoken, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestAtomicDirectiveInComment()
        {
            // setup
            //<comment, atomic> something = 'value';
            var dis = new[]
            {
                new Definition(
                    name: "something",
                    directives: new[]
                    {
                        DefinitionDirective.Comment,
                        DefinitionDirective.Atomic
                    },
                    expr: new Expression(
                        new LiteralSubExpression("value")))
            };
            var g = new Grammar(dis);
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.That(grammar.Definitions.Count, Is.EqualTo(1));

            Assert.IsNotNull(def);
            Assert.That(def.Directives.Count, Is.EqualTo(3));
            Assert.Contains(DefinitionDirective.Comment, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsTrue(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.That(def.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("value"));
            Assert.That(literal.Tag, Is.EqualTo(""));
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }

        [Test]
        public void TestImportedStaysImported()
        {
            // given
            var g = new Grammar(
                //def = 'value';
                new[]
                {
                    new Definition(
                        name: "def",
                        expr: new Expression(
                            new LiteralSubExpression(value: "value")),
                        isImported: true)  // here's the important bit
                });
            var tt = new TokenizeTransform();
            // when
            var result = tt.Tokenize(g);
            // then
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(2));
            var explicitDef = result.FindDefinitionByName("def");
            Assert.IsNotNull(explicitDef);
            Assert.IsTrue(explicitDef.IsImported); // still marked as imported
            Assert.That(g.Definitions[0], Is.Not.SameAs(explicitDef));
            var implicitDef =
                result.FindDefinitionByName("$implicit literal value");
            Assert.IsNotNull(implicitDef);
            Assert.IsTrue(implicitDef.IsImported); // should it be, though?
        }

        [Test]
        public void TestSourceStaysTheSame()
        {
            // given
            var g = new Grammar(
                //def = 'value';
                new[]
                {
                    new Definition(
                        name: "def",
                        expr: new Expression(
                            new LiteralSubExpression(value: "value")),
                        source: "src1")  // here's the important bit
                },
                null,
                "src1");
            var tt = new TokenizeTransform();
            // when
            var result = tt.Tokenize(g);
            // then
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(2));
            Assert.That(result.Source, Is.EqualTo("src1"));

            var explicitDef = result.FindDefinitionByName("def");
            Assert.IsNotNull(explicitDef);
            Assert.That(explicitDef.Source, Is.EqualTo("src1")); //
            Assert.That(g.Definitions[0], Is.Not.SameAs(explicitDef));

            var implicitDef =
                result.FindDefinitionByName("$implicit literal value");
            Assert.IsNotNull(implicitDef);

            Assert.That(implicitDef.Source, Is.EqualTo("src1"));
            // should it be, though?
        }
    }
}

