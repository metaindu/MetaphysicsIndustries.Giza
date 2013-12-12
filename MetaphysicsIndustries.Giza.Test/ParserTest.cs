using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void TestSimple()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\l_] [\l\d_]*;

            var operandDef =
                new Definition(
                    name: "operand",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l_")),
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d_")),
                    },
                    nexts: new [] { 0, 1, 1, 1 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0, 1 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var operatorDef =
                new Definition(
                    name: "$implicit literal +",
                    nodes: new [] { new CharNode('+') },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var exprDef =
                new Definition(
                    name: "expr",
                    nodes: new Node[] {
                        new DefRefNode(operandDef),
                        new DefRefNode(operatorDef),
                        new DefRefNode(operandDef),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 }
                );
            var grammar = new Grammar(exprDef, operandDef, operatorDef);

            var errors = new List<Error>();
            var parser = new Parser(exprDef);


            // action
            Span[] s = parser.Parse("a + b".ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(s);
            Assert.AreEqual(1, s.Length);
            Assert.AreEqual(3, s[0].Subspans.Count);

            Assert.AreSame(exprDef.Nodes[0], s[0].Subspans[0].Node);
            Assert.AreSame(operandDef, s[0].Subspans[0].DefRef);
            Assert.AreEqual("a", s[0].Subspans[0].Value);

            Assert.AreSame(exprDef.Nodes[1], s[0].Subspans[1].Node);
            Assert.AreSame(operatorDef, s[0].Subspans[1].DefRef);
            Assert.AreEqual("+", s[0].Subspans[1].Value);

            Assert.AreSame(exprDef.Nodes[2], s[0].Subspans[2].Node);
            Assert.AreSame(operandDef, s[0].Subspans[2].DefRef);
            Assert.AreEqual("b", s[0].Subspans[2].Value);
        }

        [Test]
        public void TestAmbiguousImplicitTokens()
        {
            string grammarText =
                "expr = subexpr '+' subexpr;\n" +
                "subexpr = ( operand | operand '++' | '++' operand ); \n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            var dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            var grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            var exprDef = grammar.FindDefinitionByName("expr");
            var subexprDef = grammar.FindDefinitionByName("subexpr");
            var operandDef = grammar.FindDefinitionByName("operand");
            var plusDef = grammar.FindDefinitionByName("$implicit literal +");
            var plusPlusDef = grammar.FindDefinitionByName("$implicit literal ++");

            var parser = new Parser(exprDef);
            errors = new List<Error>();



            Span[] s = parser.Parse("a+++b".ToCharacterSource(), errors);



            Assert.IsNotNull(s);
            Assert.AreEqual(2, s.Length);

            Assert.AreEqual(3, s[0].Subspans.Count);
            Assert.AreSame(subexprDef, s[0].Subspans[0].DefRef);
            Assert.AreSame(plusDef, s[0].Subspans[1].DefRef);
            Assert.AreEqual(0, s[0].Subspans[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[0].Subspans[2].DefRef);

            Assert.AreEqual(3, s[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[1].Subspans[0].DefRef);
            Assert.AreSame(plusDef, s[1].Subspans[1].DefRef);
            Assert.AreEqual(0, s[1].Subspans[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[1].Subspans[2].DefRef);

            Assert.True(s[0].Subspans[0].Subspans.Count == 1 ||
                        s[0].Subspans[0].Subspans.Count == 2);
            Assert.True(s[1].Subspans[0].Subspans.Count == 1 ||
                        s[1].Subspans[0].Subspans.Count == 2);
            Assert.True(s[0].Subspans[0].Subspans.Count != s[1].Subspans[0].Subspans.Count);

            Span oneTwo;
            Span twoOne;
            if (s[0].Subspans[0].Subspans.Count == 1)
            {
                oneTwo = s[0];
                twoOne = s[1];
            }
            else
            {
                oneTwo = s[1];
                twoOne = s[0];
            }

            Assert.AreEqual(1, oneTwo.Subspans[0].Subspans.Count);
            Assert.AreEqual(0, oneTwo.Subspans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, oneTwo.Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("a", oneTwo.Subspans[0].Subspans[0].Value);
            Assert.AreEqual(2, oneTwo.Subspans[2].Subspans.Count);
            Assert.AreEqual(0, oneTwo.Subspans[2].Subspans[0].Subspans.Count);
            Assert.AreSame(plusPlusDef, oneTwo.Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual(0, oneTwo.Subspans[2].Subspans[1].Subspans.Count);
            Assert.AreSame(operandDef, oneTwo.Subspans[2].Subspans[1].DefRef);
            Assert.AreEqual("b", oneTwo.Subspans[2].Subspans[1].Value);

            Assert.AreEqual(2, twoOne.Subspans[0].Subspans.Count);
            Assert.AreEqual(0, twoOne.Subspans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, twoOne.Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("a", twoOne.Subspans[0].Subspans[0].Value);
            Assert.AreSame(plusPlusDef, twoOne.Subspans[0].Subspans[1].DefRef);
            Assert.AreEqual(0, twoOne.Subspans[0].Subspans[1].Subspans.Count);
            Assert.AreEqual(1, twoOne.Subspans[2].Subspans.Count);
            Assert.AreEqual(0, twoOne.Subspans[2].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, twoOne.Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("b", twoOne.Subspans[2].Subspans[0].Value);
        }

        [Test]
        public void TestTokenIgnoreCase1()
        {
            // setup
            //sequence = item+;
            //<token, ignore case> item = 'item';
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new CharNode('i', "item"),
                        new CharNode('t', "item"),
                        new CharNode('e', "item"),
                        new CharNode('m', "item"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 3 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef);
            string testInputText = "item ITEM iTeM";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestTokenIgnoreCase2()
        {
            // setup

            //sequence = item+;
            //<token, ignore case> item = [\l]+;
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l"))
                    },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef);
            string testInputText = "item ITEM iTeM";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenIgnoreCase1()
        {
            // setup

            //sequence = item+;
            //<token> item = 'abc' middle 'xyz';
            //<subtoken, ignore case> middle = 'qwer';
            var middleDef =
                new Definition(
                    name: "middle",
                    nodes: new [] {
                        new CharNode('q', "qwer"),
                        new CharNode('w', "qwer"),
                        new CharNode('e', "qwer"),
                        new CharNode('r', "qwer"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 3 },
                    directives: new [] {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new Node[] {
                        new CharNode('a', "abc"),
                        new CharNode('b', "abc"),
                        new CharNode('c', "abc"),
                        new DefRefNode(middleDef),
                        new CharNode('x', "xyz"),
                        new CharNode('y', "xyz"),
                        new CharNode('z', "xyz"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 6 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, middleDef);
            string testInputText = "abcqwerxyz abcQWERxyz abcQwErxyz";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abcqwerxyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("abcQWERxyz", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("abcQwErxyz", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenIgnoreCase2()
        {
            // setup

            //sequence = item+;
            //<token> item = 'abc' middle 'xyz';
            //<subtoken, ignore case> middle = [\l]+;
            var middleDef =
                new Definition(
                    name: "middle",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l")),
                    },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new Node[] {
                        new CharNode('a', "abc"),
                        new CharNode('b', "abc"),
                        new CharNode('c', "abc"),
                        new DefRefNode(middleDef),
                        new CharNode('x', "xyz"),
                        new CharNode('y', "xyz"),
                        new CharNode('z', "xyz"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 6 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, middleDef);
            string testInputText = "abcqwerxyz abcQWERxyz abcQwErxyz";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abcqwerxyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("abcQWERxyz", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("abcQwErxyz", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenAtomic()
        {
            // setup

            //sequence = item+;
            //<token> item = 'abc-' sub+ '-xyz';
            //<subtoken, atomic> sub = [\l]+;
            var sub =
                new Definition(
                    name: "sub",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l")),
                    },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new Node[] {
                        new CharNode('a', "abc-"),
                        new CharNode('b', "abc-"),
                        new CharNode('c', "abc-"),
                        new CharNode('-', "abc-"),
                        new DefRefNode(sub),
                        new CharNode('-', "-xyz"),
                        new CharNode('x', "-xyz"),
                        new CharNode('y', "-xyz"),
                        new CharNode('z', "-xyz"),
                    },
                    nexts: new [] {
                        0, 1,
                        1, 2,
                        2, 3,
                        3, 4,
                        4, 4,
                        4, 5,
                        5, 6,
                        6, 7,
                        7, 8,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 8 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, sub);
            string testInputText = "abc-qwer-xyz";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abc-qwer-xyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestSubtokenNonAtomic()
        {
            // setup

            //sequence = item+;
            //<token> item = 'abc-' sub+ '-xyz';
            //<subtoken> sub = [\l]+;
            var sub =
                new Definition(
                    name: "sub",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l")),
                    },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Subtoken,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new Node[] {
                        new CharNode('a', "abc-"),
                        new CharNode('b', "abc-"),
                        new CharNode('c', "abc-"),
                        new CharNode('-', "abc-"),
                        new DefRefNode(sub),
                        new CharNode('-', "-xyz"),
                        new CharNode('x', "-xyz"),
                        new CharNode('y', "-xyz"),
                        new CharNode('z', "-xyz"),
                    },
                    nexts: new [] {
                        0, 1,
                        1, 2,
                        2, 3,
                        3, 4,
                        4, 4,
                        4, 5,
                        5, 6,
                        6, 7,
                        7, 8,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 8 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, sub);
            string testInputText = "abc-qw-xyz";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);



            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans.Count);
            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abc-qw-xyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestCommentIgnoreCase1()
        {
            // setup

            //sequence = item+;
            //<token> item = 'item';
            //<comment, ignore case> comment = '[comment]';
            var commentDef =
                new Definition(
                    name: "comment",
                    nodes: new [] {
                        new CharNode('[', "[comment]"),
                        new CharNode('c', "[comment]"),
                        new CharNode('o', "[comment]"),
                        new CharNode('m', "[comment]"),
                        new CharNode('m', "[comment]"),
                        new CharNode('e', "[comment]"),
                        new CharNode('n', "[comment]"),
                        new CharNode('t', "[comment]"),
                        new CharNode(']', "[comment]"),
                    },
                    nexts: new [] {
                        0, 1,
                        1, 2,
                        2, 3,
                        3, 4,
                        4, 5,
                        5, 6,
                        6, 7,
                        7, 8,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 8 },
                    directives: new [] {
                        DefinitionDirective.Comment,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new CharNode('i', "item"),
                        new CharNode('t', "item"),
                        new CharNode('e', "item"),
                        new CharNode('m', "item"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 3 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, commentDef);
            string testInputText = "item [comment] item [COMMENT] item [CoMmEnT]";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestCommentIgnoreCase2()
        {
            // setup

            //sequence = item+;
            //<token> item = 'item';
            //<comment, ignore case> comment = '[' [\l]+ ']';
            var commentDef =
                new Definition(
                    name: "comment",
                    nodes: new [] {
                        new CharNode('[', "[comment]"),
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l")),
                        new CharNode(']', "[comment]"),
                    },
                    nexts: new [] {
                        0, 1,
                        1, 1,
                        1, 2,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new [] {
                        DefinitionDirective.Comment,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.IgnoreCase
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new CharNode('i', "item"),
                        new CharNode('t', "item"),
                        new CharNode('e', "item"),
                        new CharNode('m', "item"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 3 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar(sequenceDef, itemDef, commentDef);
            string testInputText = "item [comment] item [COMMENT] item [CoMmEnT]";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestCaseInsensitiveImplicitTokens1()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<ignore case> item = 'item'; \r\n";
            string testInputText = "item ITEM iTeM";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var implicitDef = testGrammar.FindDefinitionByName("$implicit ignore case literal item");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            Assert.IsNotNull(implicitDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[1].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[1].Subspans[0].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[2].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestCaseInsensitiveImplicitTokens2()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<ignore case, mind whitespace> item = [\\dabcdef]; \r\n";
            string testInputText = "0 a A";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var implicitDef = testGrammar.FindDefinitionByName("$implicit ignore case char class \\dabcdef");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            Assert.IsNotNull(implicitDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("0", spans[0].Subspans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[1].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[1].Subspans[0].DefRef);
            Assert.AreEqual("a", spans[0].Subspans[1].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[2].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("A", spans[0].Subspans[2].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans[0].Subspans.Count);
        }


        static Grammar CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken()
        {
            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var item1Def =
                new Definition(
                    name: "id-item1",
                    nodes: new [] {
                        new CharNode('i', "item1"),
                        new CharNode('t', "item1"),
                        new CharNode('e', "item1"),
                        new CharNode('m', "item1"),
                        new CharNode('1', "item1"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var item2Def =
                new Definition(
                    name: "id-item2",
                    nodes: new [] {
                        new CharNode('i', "item2"),
                        new CharNode('t', "item2"),
                        new CharNode('e', "item2"),
                        new CharNode('m', "item2"),
                        new CharNode('2', "item2"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var implicitOpenDef =
                new Definition(
                    name: "$implicit literal (",
                    nodes: new [] { new CharNode('(', "") },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var implicitCloseDef =
                new Definition(
                    name: "$implicit literal )",
                    nodes: new [] { new CharNode(')', "") },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef = new Definition("sequence");
            var parenDef =
                new Definition(
                    name: "paren",
                    nodes: new [] {
                        new DefRefNode(implicitOpenDef, "("),
                        new DefRefNode(sequenceDef),
                        new DefRefNode(implicitCloseDef, ")"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new DefRefNode(item1Def),
                        new DefRefNode(item2Def),
                        new DefRefNode(parenDef),
                    },
                    startNodes: new [] { 0, 1, 2 },
                    endNodes: new [] { 0, 1, 2 }
                );
            sequenceDef.Init(
                name: "sequence",
                nodes: new [] { new DefRefNode(itemDef) },
                nexts: new [] { 0, 0 },
                startNodes: new [] { 0 },
                endNodes: new [] { 0 }
            );

            return new Grammar(sequenceDef, itemDef, parenDef, item1Def, item2Def, implicitOpenDef, implicitCloseDef);
        }


        [Test]
        public void TestUnexpectedEndOfInput1()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken();

            var errors = new List<Error>();
            var itemDef = testGrammar.FindDefinitionByName("item");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item2 ";

            // action
            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("id-item2", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(itemDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestUnexpectedEndOfInput2()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken();

            var errors = new List<Error>();
            var itemDef = testGrammar.FindDefinitionByName("item");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item2";

            // action
            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(14, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("id-item2", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(itemDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestUnexpectedEndOfInputInToken()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken();


            var errors = new List<Error>();
            var itemDef = testGrammar.FindDefinitionByName("item");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item";

            // action
            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(13, err.Column);
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            Assert.AreEqual("m", (err.LastValidMatchingNode as CharNode).CharClass.ToUndelimitedString());
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
//            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
//            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidToken1()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken();

            string testInput = "item1 ( )";

            var errors = new List<Error>();

            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var cparenDef = testGrammar.FindDefinitionByName("$implicit literal )");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.InvalidInputElement, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
            Assert.AreEqual(8, err.OffendingInputElement.StartPosition.Index);
            Assert.AreEqual(")", err.OffendingInputElement.Value);
            Assert.AreSame(cparenDef, err.OffendingInputElement.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidToken2()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren | one-two ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "one-two = 'one' 'two'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren | one-two );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //one-two = 'one' 'two';
            //paren = '(' sequence ')';

            var item1Def =
                new Definition(
                    name: "id-item1",
                    nodes: new [] {
                        new CharNode('i', "item1"),
                        new CharNode('t', "item1"),
                        new CharNode('e', "item1"),
                        new CharNode('m', "item1"),
                        new CharNode('1', "item1"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var item2Def =
                new Definition(
                    name: "id-item2",
                    nodes: new [] {
                        new CharNode('i', "item2"),
                        new CharNode('t', "item2"),
                        new CharNode('e', "item2"),
                        new CharNode('m', "item2"),
                        new CharNode('2', "item2"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var implicitOpenDef =
                new Definition(
                    name: "$implicit literal (",
                    nodes: new [] { new CharNode('(', "") },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var implicitCloseDef =
                new Definition(
                    name: "$implicit literal )",
                    nodes: new [] { new CharNode(')', "") },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef = new Definition("sequence");
            var parenDef =
                new Definition(
                    name: "paren",
                    nodes: new [] {
                        new DefRefNode(implicitOpenDef, "("),
                        new DefRefNode(sequenceDef),
                        new DefRefNode(implicitCloseDef, ")"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 }
                );
            var implicitOneDef =
                new Definition(
                    name: "$implicit literal one",
                    nodes: new [] {
                        new CharNode('o', ""),
                        new CharNode('n', ""),
                        new CharNode('e', ""),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var implicitTwoDef =
                new Definition(
                    name: "$implicit literal two",
                    nodes: new [] {
                        new CharNode('t', ""),
                        new CharNode('w', ""),
                        new CharNode('o', ""),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var oneTwoDef =
                new Definition(
                    name: "one-two",
                    nodes: new [] {
                        new DefRefNode(implicitOneDef, "one"),
                        new DefRefNode(implicitTwoDef, "two"),
                    },
                    nexts: new [] { 0, 1 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 1 }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new DefRefNode(item1Def),
                        new DefRefNode(item2Def),
                        new DefRefNode(parenDef),
                        new DefRefNode(oneTwoDef),
                    },
                    startNodes: new [] { 0, 1, 2, 3 },
                    endNodes: new [] { 0, 1, 2, 3 }
                );
            sequenceDef.Init(
                name: "sequence",
                nodes: new [] { new DefRefNode(itemDef) },
                nexts: new [] { 0, 0 },
                startNodes: new [] { 0 },
                endNodes: new [] { 0 }
            );

            var testGrammar = new Grammar(
                sequenceDef,
                itemDef,
                oneTwoDef,
                parenDef,
                item1Def,
                item2Def,
                implicitOneDef,
                implicitTwoDef,
                implicitOpenDef,
                implicitCloseDef
            );

            string testInput = "item1 ( two ";

            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);

            // action
            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.InvalidInputElement, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
            Assert.AreEqual(8, err.OffendingInputElement.StartPosition.Index);
            Assert.AreEqual("two", err.OffendingInputElement.Value);
            Assert.AreSame(implicitTwoDef, err.OffendingInputElement.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(implicitOpenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidCharacter()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInputAndInvalidToken();

            string testInput = "item1 ( $";

            var errors = new List<Error>();
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
//            Assert.AreEqual(8, err.OffendingToken.StartPosition.Index);
            Assert.AreEqual('$', err.OffendingCharacter);
//            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
//            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
//            Assert.IsNotNull(err.ExpectedNodes);
//            Assert.AreEqual(1, err.ExpectedNodes.Count());
//            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
//            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestExcessRemainingInput()
        {
            // setup

            //sequence = id-one id-two id-three;
            //<token> id-one = 'one';
            //<token> id-two = 'two';
            //<token> id-three = 'three';
            //<token> id-four = 'four';

            var oneDef =
                new Definition(
                    name: "id-one",
                    nodes: new [] {
                        new CharNode('o', "one"),
                        new CharNode('n', "one"),
                        new CharNode('e', "one"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var twoDef =
                new Definition(
                    name: "id-two",
                    nodes: new [] {
                        new CharNode('t', "two"),
                        new CharNode('w', "two"),
                        new CharNode('o', "two"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var threeDef =
                new Definition(
                    name: "id-three",
                    nodes: new [] {
                        new CharNode('t', "three"),
                        new CharNode('h', "three"),
                        new CharNode('r', "three"),
                        new CharNode('e', "three"),
                        new CharNode('e', "three"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, 3, 4 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var fourDef =
                new Definition(
                    name: "id-four",
                    nodes: new [] {
                        new CharNode('f', "four"),
                        new CharNode('o', "four"),
                        new CharNode('u', "four"),
                        new CharNode('r', "four"),
                    },
                    nexts: new [] { 0, 1, 1, 2, 2, 3, },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 3 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] {
                        new DefRefNode(oneDef),
                        new DefRefNode(twoDef),
                        new DefRefNode(threeDef),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 }
                );
            var grammar = new Grammar(sequenceDef, oneDef, twoDef, threeDef, fourDef);

            string testInput = "one two three four";
                              //123456789012345678
            var errors = new List<Error>();
            var parser = new Parser(sequenceDef);


            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);


            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.ExcessRemainingInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.AreEqual(14, err.Index);
            Assert.AreEqual(14, err.OffendingInputElement.StartPosition.Index);
            Assert.AreEqual("four", err.OffendingInputElement.Value);
            Assert.AreSame(fourDef, err.OffendingInputElement.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(threeDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNull(err.ExpectedNodes);
        }

        [Test]
        [Explicit("A manual test for debugging the internal order of source node matches in the parser.")]
        public void TestNodeMatchTokenOrder()
        {
            var testGrammarText =
                "sequence = ( a abc c | aa bb+ cc );\n" +
                "<token> a = 'a';\n" +
                "<token> abc = 'abbbbc';\n" +
                "<token> aa = 'aa';\n" +
                "<token> bb = 'bb';\n" +
                "<token> cc = 'cc';\n" +
                "<token> c = 'c';";
            var testInput = "aabbbbcc".ToCharacterSource();
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(sequenceDef);


            var spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(2, spans.Length);

        }

        public class MockTokenSource : IInputSource<Token>
        {
            public readonly Dictionary<int, InputElementSet<Token>> InputElementSetsByIndex = new Dictionary<int, InputElementSet<Token>>();

            public InputElementSet<Token> GetInputAtLocation(int index)
            {
                return InputElementSetsByIndex[index];
            }

            public InputPosition CurrentPosition
            {
                get { throw new NotImplementedException(); }
            }

            public InputPosition GetPosition(int index)
            {
                throw new NotImplementedException();
            }

            public void SetCurrentIndex(int index)
            {
                throw new NotImplementedException();
            }

            public InputChar Peek()
            {
                throw new NotImplementedException();
            }

            public InputChar GetNextValue()
            {
                throw new NotImplementedException();
            }

            public int Length
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsAtEnd
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class MockError : Error
        {
            public static readonly ErrorType MockErrorType = new ErrorType(name: "MockErrorType");
            public static readonly ErrorType MockErrorType2 = new ErrorType(name: "MockErrorType2");
        }

        [Test]
        public void TestErrorsFromTokenSource()
        {
            // setup
            var def1 =
                new Definition(
                    name: "A",
                    nodes: new [] { new CharNode('a') },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Token,
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(def1) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var grammar = new Grammar(sequenceDef, def1);

            var tokenSource = new MockTokenSource();
            tokenSource.InputElementSetsByIndex[0] = new InputElementSet<Token>() {
                InputElements = new Token[] {
                    new Token(def1, new InputPosition(0), "a", 1)
                },
            };
            tokenSource.InputElementSetsByIndex[1] = new InputElementSet<Token>() {
                Errors = new List<Error> {
                    new MockError() {
                        ErrorType = MockError.MockErrorType
                    },
                    new MockError() {
                        ErrorType = MockError.MockErrorType2
                    },
                }
            };

            var parser = new Parser(sequenceDef);
            List<Error> errors = new List<Error>();

            // action
            var spans = parser.Parse(tokenSource, errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.AreEqual(2, errors.Count);
            Assert.IsInstanceOf<MockError>(errors[0]);
            Assert.AreEqual(MockError.MockErrorType, errors[0].ErrorType);
            Assert.IsInstanceOf<MockError>(errors[1]);
            Assert.AreEqual(MockError.MockErrorType2, errors[1].ErrorType);
        }
    }
}

