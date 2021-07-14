
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit literal value");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.AreEqual(1, explicitDef.Expr.Items.Count);
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.AreEqual(implicitDef.Name, defref.DefinitionName);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(3, implicitDef.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.AreEqual(1, implicitDef.Expr.Items.Count);
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(implicitDef.Expr.Items[0]);
            var literal = (LiteralSubExpression) implicitDef.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit char class \\d");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.AreEqual(1, explicitDef.Expr.Items.Count);
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.AreEqual(implicitDef.Name, defref.DefinitionName);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(3, implicitDef.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Expr.Items);
            Assert.AreEqual(1, implicitDef.Expr.Items.Count);
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<CharClassSubExpression>(implicitDef.Expr.Items[0]);
            var cc = (CharClassSubExpression) implicitDef.Expr.Items[0];
            Assert.AreEqual("\\d", cc.CharClass.ToUndelimitedString());
            Assert.AreEqual("",cc.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit ignore case literal value");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Expr.Items);
            Assert.AreEqual(1, explicitDef.Expr.Items.Count);
            Assert.IsNotNull(explicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(explicitDef.Expr.Items[0]);
            var defref = (DefRefSubExpression) explicitDef.Expr.Items[0];
            Assert.AreEqual(implicitDef.Name, defref.DefinitionName);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(4, implicitDef.Directives.Count);
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
            Assert.AreEqual(1, implicitDef.Expr.Items.Count);
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(implicitDef.Expr.Items[0]);
            var literal = (LiteralSubExpression) implicitDef.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit ignore case char class \\d");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(4, implicitDef.Directives.Count);
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
            Assert.AreEqual(1, implicitDef.Expr.Items.Count);
            Assert.IsNotNull(implicitDef.Expr.Items[0]);
            Assert.IsInstanceOf<CharClassSubExpression>(implicitDef.Expr.Items[0]);
            var cc = (CharClassSubExpression) implicitDef.Expr.Items[0];
            Assert.AreEqual("\\d", cc.CharClass.ToUndelimitedString());
            Assert.AreEqual("",cc.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("def");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(0, def.Directives.Count);
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsFalse(def.IsTokenized);
            Assert.IsFalse(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(def.Expr.Items[0]);
            var defref = (DefRefSubExpression) def.Expr.Items[0];
            Assert.AreEqual("token", defref.DefinitionName);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(3, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(2, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Subtoken, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(3, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Comment, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsTrue(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("def");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(0, def.Directives.Count);
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsFalse(def.IsTokenized);
            Assert.IsFalse(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<DefRefSubExpression>(def.Expr.Items[0]);
            var defref = (DefRefSubExpression) def.Expr.Items[0];
            Assert.AreEqual("token", defref.DefinitionName);
            Assert.AreEqual("",defref.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(3, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(3, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Subtoken, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
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
            var g = new Grammar() {Definitions = dis.ToList()};
            var tt = new TokenizeTransform();


            // action
            var grammar = tt.Tokenize(g);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(3, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Comment, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.MindWhitespace, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsTrue(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Expr.Items);
            Assert.AreEqual(1, def.Expr.Items.Count);
            Assert.IsNotNull(def.Expr.Items[0]);
            Assert.IsInstanceOf<LiteralSubExpression>(def.Expr.Items[0]);
            var literal = (LiteralSubExpression) def.Expr.Items[0];
            Assert.AreEqual("value", literal.Value);
            Assert.AreEqual("",literal.Tag);
            Assert.IsFalse(literal.IsSkippable);
            Assert.IsFalse(literal.IsRepeatable);
        }
    }
}

