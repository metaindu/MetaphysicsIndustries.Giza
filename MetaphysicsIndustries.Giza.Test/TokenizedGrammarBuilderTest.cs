using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class TokenizedGrammarBuilderTest
    {
        [Test]
        public void TestImplicitLiteral()
        {
            // setup
            //def = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    items: new [] {
                        new LiteralSubExpression(value: "value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

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
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(5, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }

        [Test]
        public void TestImplicitCharClass()
        {
            // setup
            //"def = [\\d];
            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    items: new [] {
                        new CharClassSubExpression(charClass: CharClass.FromUndelimitedCharClassText("\\d"))
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

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
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(1, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }

        [Test]
        public void TestImplicitIgnoreCaseLiteral()
        {
            // setup
            //<ignore case> def = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    items: new [] {
                        new LiteralSubExpression(value: "value")
                    },
                    directives: new [] {
                        DefinitionDirective.IgnoreCase
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

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
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(5, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }

        [Test]
        public void TestImplicitIgnoreCaseCharClass()
        {
            // setup
            //<ignore case> def = [\\d];

            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    items: new [] {
                        new CharClassSubExpression(charClass: CharClass.FromUndelimitedCharClassText("\\d"))
                    },
                    directives: new [] {
                        DefinitionDirective.IgnoreCase
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

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
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(1, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }

        [Test]
        public void TestNonTokenWithoutDirectives()
        {
            // setup
            //def = token;
            //<token> token = 'token';
            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    items: new [] {
                        new DefRefSubExpression("token")
                    }
                ),
                new DefinitionExpression(
                    name: "token",
                    items: new [] {
                        new LiteralSubExpression("token")
                    },
                    directives: new [] {
                        DefinitionDirective.Token
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(1, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestTokenDirective()
        {
            // setup
            //<token> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Token
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestSubtokenDirective()
        {
            // setup
            //<subtoken> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Subtoken
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestCommentDirective()
        {
            // setup
            //<comment> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Comment
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestAtomicDirectiveInNonToken()
        {
            // setup
            //<atomic> def = token;
            //<token> token = 'token';
            var dis = new [] {
                new DefinitionExpression(
                    name: "def",
                    directives: new [] {
                        DefinitionDirective.Atomic,
                    },
                    items: new [] {
                        new DefRefSubExpression("token")
                    }
                ),
                new DefinitionExpression(
                    name: "token",
                    directives: new [] {
                        DefinitionDirective.Token,
                    },
                    items: new [] {
                        new LiteralSubExpression("token")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(1, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestAtomicDirectiveInToken()
        {
            // setup
            //<token, atomic> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestAtomicDirectiveInSubtoken()
        {
            // setup
            //<subtoken, atomic> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.Atomic
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestAtomicDirectiveInComment()
        {
            // setup
            //<comment, atomic> something = 'value';
            var dis = new [] {
                new DefinitionExpression(
                    name: "something",
                    directives: new [] {
                        DefinitionDirective.Comment,
                        DefinitionDirective.Atomic
                    },
                    items: new [] {
                        new LiteralSubExpression("value")
                    }
                )
            };
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
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
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }
    }
}

