
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
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza.Test.SpannerTests
{
    [TestFixture()]
    public class BasicTest
    {
        [Test]
        public void TestOrExpr()
        {
            // setup
            //def1 = ( 'qwer' | 'asdf' );
            var testGrammar = new NGrammar {
                Definitions = {
                    new NDefinition(
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
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.IsEmpty(errors);

            // action
            spans = s.Process("asdf".ToCharacterSource(), errors);
            // assertions
            Assert.That(spans.Length, Is.EqualTo(1));
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
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError<InputChar>>(errors[0]);
            var err = ((ParserError<InputChar>)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.InvalidInputElement));
            Assert.That(err.OffendingInputElement.Value, Is.EqualTo('w'));
            Assert.That(err.Position.Line, Is.EqualTo(4));
            Assert.That(err.Position.Column, Is.EqualTo(2));
            Assert.That(err.Position.Index, Is.EqualTo(74));
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            var charnode = (err.LastValidMatchingNode as CharNode);
            Assert.That(charnode.CharClass.ToUndelimitedString(),
                Is.EqualTo("<"));
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
            Assert.That(
                (err.ExpectedNodes.First() as DefRefNode).DefRef.Name,
                Is.EqualTo("directive-item"));
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
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError<InputChar>>(errors[0]);
            var err = ((ParserError<InputChar>)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.InvalidInputElement));
            Assert.That(err.OffendingInputElement.Value, Is.EqualTo('w'));
            Assert.That(err.Position.Line, Is.EqualTo(4));
            Assert.That(err.Position.Column, Is.EqualTo(2));
            Assert.That(err.Position.Index, Is.EqualTo(74));
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            var charnode = (err.LastValidMatchingNode as CharNode);
            Assert.That(charnode.CharClass.ToUndelimitedString(),
                Is.EqualTo("<"));
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
            Assert.That(
                (err.ExpectedNodes.First() as DefRefNode).DefRef.Name,
                Is.EqualTo("directive-item"));

            Assert.That(
                err.Description,
                Is.EqualTo("Invalid token 'w' at position 4,2 (index 74). " +
                           "Expected directive-item."));
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
                    name: "item",
                    nodes: new [] {
                        new DefRefNode(item1Def),
                        new DefRefNode(item2Def),
                    },
                    startNodes: new [] { 0, 1 },
                    endNodes: new [] { 0, 1 }
                );
            var sequenceDef =
                new NDefinition(
                    name: "sequence",
                    nodes: new [] { new DefRefNode(itemDef) },
                    nexts: new [] { 0, 0 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var testGrammar = new NGrammar {
                Definitions = { sequenceDef, itemDef, item1Def, item2Def }
            };

            string testInput = "$ item1 item2 ";

            var errors = new List<Error>();

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError<InputChar>>(errors[0]);
            var err = ((ParserError<InputChar>)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.InvalidInputElement));
            Assert.That(err.OffendingInputElement.Value, Is.EqualTo('$'));
            Assert.That(err.Position.Line, Is.EqualTo(1));
            Assert.That(err.Position.Column, Is.EqualTo(1));
            Assert.That(err.Position.Index, Is.EqualTo(0));
            Assert.IsNull(err.LastValidMatchingNode);
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
            Assert.That(
                (err.ExpectedNodes.First() as DefRefNode).DefRef.Name,
                Is.EqualTo("item"));
        }

        static NGrammar CreateGrammarForTestUnexpectedEndOfInput()
        {
            //sequence = item+;
            //item = ( id-item1 | id-item2 | paren );
            //<mind whitespace, atomic> id-item1 = 'item1';
            //<mind whitespace, atomic> id-item2 = 'item2';
            //paren = '(' sequence ')';

            var item1Def =
                new NDefinition(
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
                new NDefinition(
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
            var sequenceDef = new NDefinition("sequence");
            var parenDef =
                new NDefinition(
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
                new NDefinition(
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

            return new NGrammar(new [] { sequenceDef, itemDef, item1Def, item2Def, parenDef });
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
            Assert.That(spans.Length, Is.EqualTo(0));
            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.UnexpectedEndOfInput));
            Assert.That(err.Position.Line, Is.EqualTo(1));
            Assert.That(err.Position.Column, Is.EqualTo(15));
            Assert.That(err.Position.Index, Is.EqualTo(14));
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.That(
                (err.LastValidMatchingNode as DefRefNode).DefRef.Name,
                Is.EqualTo("sequence"));
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.That(
                (err.ExpectedNodes.First() as CharNode).CharClass
                .ToUndelimitedString(),
                Is.EqualTo(")"));
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
            Assert.That(spans.Length, Is.EqualTo(0));
            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.UnexpectedEndOfInput));
            Assert.That(err.Position.Line, Is.EqualTo(1));
            Assert.That(err.Position.Column, Is.EqualTo(14));
            Assert.That(err.Position.Index, Is.EqualTo(13));
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.That(
                (err.LastValidMatchingNode as DefRefNode).DefRef.Name,
                Is.EqualTo("sequence"));
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.That(
                (err.ExpectedNodes.First() as CharNode).CharClass
                .ToUndelimitedString(),
                Is.EqualTo(")"));
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
            Assert.That(spans.Length, Is.EqualTo(0));
            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.UnexpectedEndOfInput));
            Assert.That(err.Position.Line, Is.EqualTo(1));
            Assert.That(err.Position.Column, Is.EqualTo(13));
            Assert.That(err.Position.Index, Is.EqualTo(12));
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            Assert.That(
                (err.LastValidMatchingNode as CharNode).CharClass
                .ToUndelimitedString(),
                Is.EqualTo("m"));
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.That(err.ExpectedNodes.Count(), Is.EqualTo(1));
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
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
            var testGrammar = new NGrammar(sequenceDef, oneDef, twoDef, threeDef);

            string testInput = "one two three four";
                              //123456789012345678
            var errors = new List<Error>();

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));

            // action
            Span[] spans = s.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.That(spans.Length, Is.EqualTo(0));
            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.That(err.ErrorType,
                Is.EqualTo(ParserError.ExcessRemainingInput));
            Assert.That(err.Position.Line, Is.EqualTo(1));
            Assert.That(err.Position.Column, Is.EqualTo(15));
            Assert.That(err.Position.Index, Is.EqualTo(14));
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.That(
                (err.LastValidMatchingNode as DefRefNode).DefRef.Name,
                Is.EqualTo("sequence"));
            Assert.IsNull(err.ExpectedNodes);
        }

        static NGrammar CreateGrammarForTestEndOfInputParameter()
        {
            //expr = operand '+' operand;
            //<atomic> operand = [\l_] [\l\d_]*;

            var operandDef =
                new NDefinition(
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
            var grammar = new NGrammar {
                Definitions = {
                    new NDefinition(
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
            Assert.That(endOfInputPosition.Index, Is.EqualTo(5));
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
            Assert.That(endOfInputPosition.Index, Is.EqualTo(6));
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
            Assert.That(endOfInputPosition.Index, Is.EqualTo(6));
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
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
            var grammar = new NGrammar(formatDef, textDef, paramDef, nameDef);

            string testInput = "leading {delimited}x";
            var errors = new List<Error>();
            var spanner = new Spanner(grammar.FindDefinitionByName("format"));

            // action
            var spans = spanner.Process(testInput.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(spans);
            Assert.That(spans.Length, Is.EqualTo(1));
            var span = spans[0];
            Assert.That(span.DefRef, Is.SameAs(formatDef));
            Assert.That(span.Subspans.Count, Is.EqualTo(3));
            var s0 = span.Subspans[0];
            var s1 = span.Subspans[1];
            var s2 = span.Subspans[2];
            Assert.That(s0.DefRef, Is.SameAs(textDef));
            Assert.That(s0.CollectValue(), Is.EqualTo("leading "));

            Assert.That(s1.DefRef, Is.SameAs(paramDef));
            Assert.That(s1.Subspans.Count, Is.EqualTo(3));
            var s10 = s1.Subspans[0];
            var s11 = s1.Subspans[1];
            var s12 = s1.Subspans[2];
            Assert.That(s10.Node, Is.SameAs(paramDef.Nodes[0]));
            Assert.IsNull(s10.DefRef);
            Assert.That(s10.Subspans.Count, Is.EqualTo(0));
            Assert.That(s10.Value, Is.EqualTo("{"));
            Assert.That(s11.DefRef, Is.SameAs(nameDef));
            Assert.That(s11.CollectValue(), Is.EqualTo("delimited"));
            Assert.That(s12.Node, Is.SameAs(paramDef.Nodes[4]));
            Assert.IsNull(s12.DefRef);
            Assert.That(s12.Subspans.Count, Is.EqualTo(0));
            Assert.That(s12.Value, Is.EqualTo("}"));

            Assert.That(s2.DefRef, Is.SameAs(textDef));
            Assert.That(s2.CollectValue(), Is.EqualTo("x"));
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
                new NDefinition(
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
                new NDefinition(
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
                new NDefinition(
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
            var grammar = new NGrammar(fileDef, recordDef, fieldDef);
            var errors = new List<Error>();
            var spanner = new Spanner(grammar.FindDefinitionByName("file"));

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.That(spans.Length, Is.EqualTo(1));
        }

        [Test]
        public void TestIgnoreCase1()
        {
            // setup
            var item =
                new Definition(
                    name: "item",
                    directives: new[] {DefinitionDirective.IgnoreCase},
                    expr: new Expression(new LiteralSubExpression("item")));
            var grammar = (new GrammarCompiler()).Compile(new [] { item });
            var itemDef = grammar.FindDefinitionByName("item");
            var spanner = new Spanner(itemDef);
            var input = "iTeM";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.That(spans.Length, Is.EqualTo(1));
            var s = spans[0];
            Assert.That(s.DefRef, Is.SameAs(itemDef));
            Assert.That(s.Subspans.Count, Is.EqualTo(4));
            Assert.That(s.Subspans[0].Node, Is.SameAs(itemDef.Nodes[0]));
            Assert.That(s.Subspans[0].Value, Is.EqualTo("i"));
            Assert.IsEmpty(s.Subspans[0].Subspans);
            Assert.That(s.Subspans[1].Node, Is.SameAs(itemDef.Nodes[1]));
            Assert.That(s.Subspans[1].Value, Is.EqualTo("T"));
            Assert.IsEmpty(s.Subspans[1].Subspans);
            Assert.That(s.Subspans[2].Node, Is.SameAs(itemDef.Nodes[2]));
            Assert.That(s.Subspans[2].Value, Is.EqualTo("e"));
            Assert.IsEmpty(s.Subspans[2].Subspans);
            Assert.That(s.Subspans[3].Node, Is.SameAs(itemDef.Nodes[3]));
            Assert.That(s.Subspans[3].Value, Is.EqualTo("M"));
            Assert.IsEmpty(s.Subspans[3].Subspans);
        }

        [Test]
        public void TestIgnoreCase2()
        {
            // setup
            var item =
                new Definition(
                    name: "item",
                    expr: new Expression(new LiteralSubExpression("item")));
            var grammar = (new GrammarCompiler()).Compile(new [] { item });
            var itemDef = grammar.FindDefinitionByName("item");
            var spanner = new Spanner(itemDef);
            var input = "iTeM";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ParserError<InputChar>>(errors[0]);
            var e = (ParserError<InputChar>)(errors[0]);
            Assert.That(e.ErrorType,
                Is.EqualTo(ParserError.InvalidInputElement));
            Assert.That(e.OffendingInputElement.Value, Is.EqualTo('T'));
            Assert.That(e.Position,
                Is.EqualTo(new InputPosition(1, 1, 2)));
            Assert.IsNotNull(spans);
            Assert.That(spans.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestSingleCharNode()
        {
            // setup
            var def =
                new NDefinition(
                    name: "def",
                    nodes: new [] {
                        new CharNode('a')
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var grammar = new NGrammar(def);
            var spanner = new Spanner(def);
            var errors = new List<Error>();

            // action
            var spans = spanner.Process("a".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].DefRef, Is.SameAs(def));
            Assert.That(spans[0].Subspans.Count, Is.EqualTo(1));
            Assert.That(spans[0].Subspans[0].Node, Is.SameAs(def.Nodes[0]));
            Assert.IsEmpty(spans[0].Subspans[0].Subspans);
            Assert.That(spans[0].Subspans[0].Value, Is.EqualTo("a"));
        }

        [Test]
        public void TestDefRefNode()
        {
            var chardef =
                new NDefinition(
                    name: "chardef",
                    nodes: new [] {
                        new CharNode('a')
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var defdef =
                new NDefinition(
                    name: "defdef",
                    nodes: new [] {
                        new DefRefNode(chardef)
                    },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 0 }
                );
            var grammar = new NGrammar(defdef, chardef);
            var spanner = new Spanner(defdef);
            var errors = new List<Error>();

            // action
            var spans = spanner.Process("a".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            var s = spans[0];
            Assert.That(s.DefRef, Is.SameAs(defdef));
            Assert.That(s.Subspans.Count, Is.EqualTo(1));
            var s2 = s.Subspans[0];
            Assert.That(s2.DefRef, Is.SameAs(chardef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(1));
            var s3 = s2.Subspans[0];
            Assert.That(s3.Node, Is.SameAs(chardef.Nodes[0]));
            Assert.That(s3.Value, Is.EqualTo("a"));
            Assert.IsEmpty(s3.Subspans);
        }
    }
}

