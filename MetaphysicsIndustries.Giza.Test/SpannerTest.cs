using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class SpannerTest
    {
        [Test]
        public void TestOrExpr()
        {
            // setup
            //def1 = ( 'qwer' | 'asdf' );
            var testGrammar = new Grammar {
                Definitions = {
                    new Definition(
                        name: "def1",
                        nodes: new [] {
                            new CharNode('q', "qwer"),
                            new CharNode('w', "qwer"),
                            new CharNode('e', "qwer"),
                            new CharNode('r', "qwer"),
                            new CharNode('a', "asdf"),
                            new CharNode('s', "asdf"),
                            new CharNode('d', "asdf"),
                            new CharNode('f', "asdf"),
                        },
                        nexts: new [] {
                            0, 1,
                            1, 2,
                            2, 3,
                            4, 5,
                            5, 6,
                            6, 7,
                        },
                        startNodes: new [] { 0, 4 },
                        endNodes: new [] { 3, 7 }
                    )
                }
            };
            var errors = new List<Error>();
            Spanner s = new Spanner(testGrammar.FindDefinitionByName("def1"));

            // action
            Span[] spans = s.Process("qwer".ToCharacterSource(), errors);

            // assertions
            Assert.AreEqual(1, spans.Length);
            Assert.IsEmpty(errors);

            // action
            spans = s.Process("asdf".ToCharacterSource(), errors);
            // assertions
            Assert.AreEqual(1, spans.Length);
            Assert.IsEmpty(errors);
        }

        [Test()]
        public void TestErrorInvalidCharacterAfterDefRef()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 ); \r\n" +
                "<whitespace, atomic> id-item1 = 'item1'; \r\n" +
                "<whitespace, atomic> id-item2 = 'item2'; \r\n";

            Supergrammar sg = new Supergrammar();
            Spanner s = new Spanner(sg.def_grammar);
            var errors = new List<Error>();
            Span[] spans = s.Process(testGrammarText.ToCharacterSource(), errors);

            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('w', err.OffendingCharacter);
            Assert.AreEqual(4, err.Position.Line);
            Assert.AreEqual(2, err.Position.Column);
            Assert.AreEqual(74, err.Position.Index);
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            var charnode = (err.LastValidMatchingNode as CharNode);
            Assert.AreEqual("<", charnode.CharClass.ToUndelimitedString());
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.AreEqual("directive-item", (err.ExpectedNodes.First() as DefRefNode).DefRef.Name);
        }

        [Test()]
        public void TestInvalidCharacterErrorDescription()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 ); \r\n" +
                "<whitespace, atomic> id-item1 = 'item1'; \r\n" +
                "<whitespace, atomic> id-item2 = 'item2'; \r\n";

            Supergrammar sg = new Supergrammar();
            Spanner s = new Spanner(sg.def_grammar);
            var errors = new List<Error>();
            Span[] spans = s.Process(testGrammarText.ToCharacterSource(), errors);

            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('w', err.OffendingCharacter);
            Assert.AreEqual(4, err.Position.Line);
            Assert.AreEqual(2, err.Position.Column);
            Assert.AreEqual(74, err.Position.Index);
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            var charnode = (err.LastValidMatchingNode as CharNode);
            Assert.AreEqual("<", charnode.CharClass.ToUndelimitedString());
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.AreEqual("directive-item", (err.ExpectedNodes.First() as DefRefNode).DefRef.Name);

            Assert.AreEqual(
                "Invalid character 'w' at (4,2), after a '<': expected directive-item",
                err.Description);
        }

        [Test()]
        public void TestErrorInvalidCharacterAtStart()
        {
            // setup
            //sequence = item+;
            //item = ( id-item1 | id-item2 );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
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
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var itemDef =
                new Definition(
                    name: "item",
                    nodes: new [] {
                        new DefRefNode(item1Def),
                        new DefRefNode(item2Def),
                    },
                    startNodes: new [] { 0, 1 },
                    endNodes: new [] { 0, 1 }
                );
            var sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new Grammar {
                Definitions = { sequenceDef, itemDef, item1Def, item2Def }
            };

            string testInput = "$ item1 item2 ";

            var errors = new List<Error>();

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('$', err.OffendingCharacter);
            Assert.AreEqual(1, err.Position.Line);
            Assert.AreEqual(1, err.Position.Column);
            Assert.AreEqual(0, err.Position.Index);
            Assert.IsNull(err.LastValidMatchingNode);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.AreEqual("item", (err.ExpectedNodes.First() as DefRefNode).DefRef.Name);
        }

        static Grammar CreateGrammarForTestUnexpectedEndOfInput()
        {
            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
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
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace,
                    }
                );
            var sequenceDef = new Definition("sequence");
            var parenDef =
                new Definition(
                    name: "paren",
                    nodes: new Node[] {
                        new CharNode('('),
                        new DefRefNode(sequenceDef),
                        new CharNode(')'),
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

            sequenceDef.Nodes.Add(new DefRefNode(itemDef));
            sequenceDef.Nodes[0].NextNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.StartNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.EndNodes.Add(sequenceDef.Nodes[0]);

            return new Grammar(new [] { sequenceDef, itemDef, item1Def, item2Def, parenDef });
        }

        [Test]
        public void TestUnexpectedEndOfInput1()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInput();

            var errors = new List<Error>();
            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item2 ";

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Position.Line);
            Assert.AreEqual(15, err.Position.Column);
            Assert.AreEqual(14, err.Position.Index);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("sequence", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.AreEqual(")", (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString());
        }

        [Test]
        public void TestUnexpectedEndOfInput2()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInput();

            var errors = new List<Error>();
            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item2";

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Position.Line);
            Assert.AreEqual(14, err.Position.Column);
            Assert.AreEqual(13, err.Position.Index);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("sequence", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.AreEqual(")", (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString());
        }

        [Test]
        public void TestUnexpectedEndOfInput3()
        {
            // setup

            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var testGrammar = CreateGrammarForTestUnexpectedEndOfInput();

            var errors = new List<Error>();
            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            string testInput = "item1 ( item";

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Position.Line);
            Assert.AreEqual(13, err.Position.Column);
            Assert.AreEqual(12, err.Position.Index);
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            Assert.AreEqual("m", (err.LastValidMatchingNode as CharNode).CharClass.ToUndelimitedString());
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            var expectedChar = (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString();
            Assert.True(expectedChar == "1" || expectedChar == "2");
        }

        [Test]
        public void TestExcessRemainingInput()
        {
            // setup
            //sequence = id-one id-two id-three;
            //<mind whitespace, atomic> id-one = 'one';
            //<mind whitespace, atomic> id-two = 'two';
            //<mind whitespace, atomic> id-three = 'three';

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
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.Atomic,
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
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.Atomic,
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
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.Atomic,
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
            var testGrammar = new Grammar(sequenceDef, oneDef, twoDef, threeDef);

            string testInput = "one two three four";
                              //123456789012345678
            var errors = new List<Error>();

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.ExcessRemainingInput, err.ErrorType);
            Assert.AreEqual(1, err.Position.Line);
            Assert.AreEqual(15, err.Position.Column);
            Assert.AreEqual(14, err.Position.Index);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("sequence", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNull(err.ExpectedNodes);
        }

        static Grammar CreateGrammarForTestEndOfInputParameter()
        {
            //expr = operand '+' operand;
            //<atomic> operand = [\l_] [\l\d_]*;

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
                    directives: new [] { DefinitionDirective.Atomic }
                );
            var grammar = new Grammar {
                Definitions = {
                    new Definition(
                        name: "expr",
                        nodes: new Node[] {
                            new DefRefNode(operandDef),
                            new CharNode('+'),
                            new DefRefNode(operandDef),
                        },
                        nexts: new [] { 0, 1, 1, 2 },
                        startNodes: new [] { 0 },
                        endNodes: new [] { 2 }
                    ),
                    operandDef
                }
            };
            return grammar;
        }

        [Test]
        public void TestEndOfInputParameter1()
        {
            // setup

            //expr = operand '+' operand;
            //<atomic> operand = [\l_] [\l\d_]*;

            var grammar = CreateGrammarForTestEndOfInputParameter();

            var errors = new List<Error>();

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b";
            bool endOfInput;
            InputPosition endOfInputPosition;

            // action
            var tokens = spanner.Match(input.ToCharacterSource(), errors,
                                       out endOfInput, out endOfInputPosition,
                                       mustUseAllInput: false, startIndex: 5);

            // assertions
            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(5, endOfInputPosition.Index);
        }

        [Test]
        public void TestEndOfInputParameter2()
        {
            // setup

            //expr = operand '+' operand;
            //<atomic> operand = [\l_] [\l\d_]*;

            var grammar = CreateGrammarForTestEndOfInputParameter();

            var errors = new List<Error>();

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;
            InputPosition endOfInputPosition;


            var tokens = spanner.Match(input.ToCharacterSource(), errors,
                                       out endOfInput, out endOfInputPosition,
                                       mustUseAllInput: false, startIndex: 5);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(6, endOfInputPosition.Index);
        }

        [Test]
        public void TestEndOfInputParameter3()
        {
            // setup

            //expr = operand '+' operand;
            //<atomic> operand = [\l_] [\l\d_]*;

            var grammar = CreateGrammarForTestEndOfInputParameter();

            var errors = new List<Error>();

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;
            InputPosition endOfInputPosition;


            var tokens = spanner.Match(input.ToCharacterSource(), errors,
                                       out endOfInput, out endOfInputPosition,
                                       mustUseAllInput: false, startIndex: 6);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(6, endOfInputPosition.Index);
        }

        [Test]
        public void TestEndDefAtLastCharacter()
        {
            // setup
            //<mind whitespace> format = ( text | param )+;
            //<atomic, mind whitespace> text = [^{}]+ ;
            //<mind whitespace> param = '{' [\s]* name [\s]* '}' ;
            //<mind whitespace> name = [\l_] [\l\d]* ;

            var nameDef =
                new Definition(
                    name: "name",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l_")),
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d")),
                    },
                    nexts: new [] { 0, 1, 1, 1 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0, 1 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace
                    }
                );
            var paramDef =
                new Definition(
                    name: "param",
                    nodes: new Node[] {
                        new CharNode('{'),
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\s")),
                        new DefRefNode(nameDef),
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\s")),
                        new CharNode('}'),
                    },
                    nexts: new [] {
                        0, 1,
                        1, 1,
                        1, 2,
                        2, 3,
                        3, 3,
                        3, 4,
                        0, 2,
                        2, 4,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 4 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace
                    }
                );
            var textDef =
                new Definition(
                    name: "text",
                    nodes: new [] { new CharNode(CharClass.FromUndelimitedCharClassText("^{}")) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.Atomic,
                        DefinitionDirective.MindWhitespace
                    }
                );
            var formatDef =
                new Definition(
                    name: "format",
                    nodes: new [] {
                        new DefRefNode(textDef),
                        new DefRefNode(paramDef),
                    },
                    nexts: new [] { 0, 0, 0, 1, 1, 1, 1, 0 },
                    startNodes: new [] { 0, 1 },
                    endNodes: new [] { 0, 1 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace
                    }
                );
            var grammar = new Grammar(formatDef, textDef, paramDef, nameDef);

            string testInput = "leading {delimited}x";
            var errors = new List<Error>();
            var spanner = new Spanner(grammar.FindDefinitionByName("format"));

            // action
            var spans = spanner.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            var span = spans[0];
            Assert.AreSame(formatDef, span.DefRef);
            Assert.AreEqual(3, span.Subspans.Count);
            var s0 = span.Subspans[0];
            var s1 = span.Subspans[1];
            var s2 = span.Subspans[2];
            Assert.AreSame(textDef, s0.DefRef);
            Assert.AreEqual("leading ", s0.CollectValue());

            Assert.AreSame(paramDef, s1.DefRef);
            Assert.AreEqual(3, s1.Subspans.Count);
            var s10 = s1.Subspans[0];
            var s11 = s1.Subspans[1];
            var s12 = s1.Subspans[2];
            Assert.AreSame(paramDef.Nodes[0], s10.Node);
            Assert.IsNull(s10.DefRef);
            Assert.AreEqual(0, s10.Subspans.Count);
            Assert.AreEqual("{", s10.Value);
            Assert.AreSame(nameDef, s11.DefRef);
            Assert.AreEqual("delimited", s11.CollectValue());
            Assert.AreSame(paramDef.Nodes[4], s12.Node);
            Assert.IsNull(s12.DefRef);
            Assert.AreEqual(0, s12.Subspans.Count);
            Assert.AreEqual("}", s12.Value);

            Assert.AreSame(textDef, s2.DefRef);
            Assert.AreEqual("x", s2.CollectValue());
        }

        [Ignore("Will not fix")]
        public void TestAmbiguityAtEndOfAtomicDef()
        {
            // setup
            var input =
                "line1,fielda,fieldb\n"+
                "line2,fielda,fieldb,fieldc\n"+
                "line3,fielda,fieldb\n";

            //<mind whitespace, atomic> file = record (('\r'? '\n')+ record?)* ;
            //<mind whitespace> record = field (',' field?)* ;
            //<mind whitespace> field = [\d\l]+;

            var fieldDef =
                new Definition(
                    name: "field",
                    nodes: new [] {
                        new CharNode(CharClass.FromUndelimitedCharClassText("\\d\\l"))
                    },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace
                    }
                );
            var recordDef =
                new Definition(
                    name: "record",
                    nodes: new Node[] {
                        new DefRefNode(fieldDef),
                        new CharNode(','),
                        new DefRefNode(fieldDef),
                    },
                    nexts: new [] { 0, 1, 1, 1, 1, 2, 2, 1 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0, 1, 2 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace
                    }
                );
            var fileDef =
                new Definition(
                    name: "file",
                    nodes: new Node[] {
                        new DefRefNode(recordDef),
                        new CharNode('\r', "\r"),
                        new CharNode('\n', "\n"),
                        new DefRefNode(recordDef),
                    },
                    nexts: new [] {
                        0, 1,
                        0, 2,
                        1, 2,
                        2, 1,
                        2, 2,
                        2, 3,
                        3, 1,
                        3, 2,
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0, 2, 3 },
                    directives: new [] {
                        DefinitionDirective.MindWhitespace,
                        DefinitionDirective.Atomic
                    }
                );
            var grammar = new Grammar(fileDef, recordDef, fieldDef);
            var errors = new List<Error>();
            var spanner = new Spanner(grammar.FindDefinitionByName("file"));

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
        }

        [Test]
        public void TestIgnoreCase1()
        {
            // setup
            var item =
                new DefinitionExpression(
                    name: "item",
                    directives: new [] { DefinitionDirective.IgnoreCase },
                    items: new [] { new LiteralSubExpression("item") }
                );
            var defs = (new DefinitionBuilder()).BuildDefinitions(new [] { item });
            var grammar = new Grammar(defs);
            var itemDef = grammar.FindDefinitionByName("item");
            var spanner = new Spanner(itemDef);
            var input = "iTeM";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            var s = spans[0];
            Assert.AreSame(itemDef, s.DefRef);
            Assert.AreEqual(4, s.Subspans.Count);
            Assert.AreSame(itemDef.Nodes[0], s.Subspans[0].Node);
            Assert.AreEqual("i", s.Subspans[0].Value);
            Assert.IsEmpty(s.Subspans[0].Subspans);
            Assert.AreSame(itemDef.Nodes[1], s.Subspans[1].Node);
            Assert.AreEqual("T", s.Subspans[1].Value);
            Assert.IsEmpty(s.Subspans[1].Subspans);
            Assert.AreSame(itemDef.Nodes[2], s.Subspans[2].Node);
            Assert.AreEqual("e", s.Subspans[2].Value);
            Assert.IsEmpty(s.Subspans[2].Subspans);
            Assert.AreSame(itemDef.Nodes[3], s.Subspans[3].Node);
            Assert.AreEqual("M", s.Subspans[3].Value);
            Assert.IsEmpty(s.Subspans[3].Subspans);
        }

        [Test]
        public void TestIgnoreCase2()
        {
            // setup
            var item =
                new DefinitionExpression(
                    name: "item",
                    items: new [] { new LiteralSubExpression("item") }
                );
            var defs = (new DefinitionBuilder()).BuildDefinitions(new [] { item });
            var grammar = new Grammar(defs);
            var itemDef = grammar.FindDefinitionByName("item");
            var spanner = new Spanner(itemDef);
            var input = "iTeM";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var e = (SpannerError)(errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, e.ErrorType);
            Assert.AreEqual('T', e.OffendingCharacter);
            Assert.AreEqual(new InputPosition(1, 1, 2), e.Position);
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
        }

        [Test]
        public void TestAtomic1()
        {
            // setup
            var sequence =
                new DefinitionExpression(
                    name: "sequence",
                    items: new [] {
                        new DefRefSubExpression("letters", isRepeatable: true)
                    }
                );
            var letters =
                new DefinitionExpression(
                    name: "letters",
                    directives: new [] { DefinitionDirective.Atomic },
                    items: new [] {
                        new CharClassSubExpression(
                            charClass: CharClass.FromUndelimitedCharClassText("\\l"),
                            isRepeatable: true
                        )
                    }
                );
            var defs = (new DefinitionBuilder()).BuildDefinitions(new [] { letters, sequence });
            var grammar = new Grammar(defs);
            var sequenceDef = grammar.FindDefinitionByName("sequence");
            var lettersDef = grammar.FindDefinitionByName("letters");
            var spanner = new Spanner(sequenceDef);
            var input = "abc";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            var s = spans[0];
            Assert.AreSame(sequenceDef, s.DefRef);
            Assert.AreEqual(1, s.Subspans.Count);
            var s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(3, s2.Subspans.Count);

            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[2].Node);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.AreEqual("c", s2.Subspans[2].Value);
        }

        [Test]
        public void TestAtomic2()
        {
            // setup
            var sequence =
                new DefinitionExpression(
                    name: "sequence",
                    items: new [] {
                        new DefRefSubExpression("letters", isRepeatable: true)
                    }
                );
            var letters =
                new DefinitionExpression(
                    name: "letters",
                    items: new [] {
                        new CharClassSubExpression(
                            charClass: CharClass.FromUndelimitedCharClassText("\\l"),
                            isRepeatable: true
                        )
                    }
                );
            var defs = (new DefinitionBuilder()).BuildDefinitions(new [] { letters, sequence });
            var grammar = new Grammar(defs);
            var sequenceDef = grammar.FindDefinitionByName("sequence");
            var lettersDef = grammar.FindDefinitionByName("letters");
            var spanner = new Spanner(sequenceDef);
            var input = "abc";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(4, spans.Length);

            // sequence
            //    |
            // letters('abc')

            //       sequence
            //       |      |
            // letters('a') letters('bc')

            //         sequence
            //         |      |
            // letters('ab') letters('c')

            //                sequence
            //              /     |    \
            //             /      |     \
            // letters('a') letters('b') letters('c')

            Assert.AreEqual(1, spans.Count(x => x.Subspans.Count == 1));    // abc
            Assert.AreEqual(1, spans.Count(x => x.Subspans.Count == 3));    // a b c
            Assert.AreEqual(2, spans.Count(x => x.Subspans.Count == 2));
            Assert.AreEqual(1, spans.Count(
                x =>
                    x.Subspans.Count == 2 &&
                    x.Subspans[0].Subspans.Count == 1 &&
                    x.Subspans[1].Subspans.Count == 2));                    // a bc
            Assert.AreEqual(1, spans.Count(
                x =>
                    x.Subspans.Count == 2 &&
                    x.Subspans[0].Subspans.Count == 2 &&
                    x.Subspans[1].Subspans.Count == 1));                    // ab c

            Span s;
            Span s2;

            s = spans.First(x => x.Subspans.Count == 1);    //abc
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(3, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[2].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.AreEqual("c", s2.Subspans[2].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 1); // a bc
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(1, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            s2 = s.Subspans[1];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(2, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreEqual("b", s2.Subspans[0].Value);
            Assert.AreEqual("c", s2.Subspans[1].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 2); // ab c
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(2, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            s2 = s.Subspans[1];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(1, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreEqual("c", s2.Subspans[0].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);

            s = spans.First(x => x.Subspans.Count == 3);    // a b c
            Assert.AreSame(sequenceDef, s.DefRef);
            Assert.AreSame(lettersDef, s.Subspans[0].DefRef);
            Assert.AreSame(lettersDef, s.Subspans[1].DefRef);
            Assert.AreSame(lettersDef, s.Subspans[2].DefRef);
            Assert.AreEqual(1, s.Subspans[0].Subspans.Count);
            Assert.AreEqual(1, s.Subspans[1].Subspans.Count);
            Assert.AreEqual(1, s.Subspans[2].Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[0].Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[1].Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[2].Subspans[0].Node);
            Assert.AreEqual("a", s.Subspans[0].Subspans[0].Value);
            Assert.AreEqual("b", s.Subspans[1].Subspans[0].Value);
            Assert.AreEqual("c", s.Subspans[2].Subspans[0].Value);
            Assert.IsEmpty(s.Subspans[0].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[1].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[2].Subspans[0].Subspans);
        }
    }
}

