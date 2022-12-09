
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class TokenizerTest
    {
        [Test()]
        public void TestNormal()
        {
            // setup

            // // test grammar
            //sequence = item+;
            //item = ( id-item1 | id-item2 );
            //<token> id-item1 = 'item1';
            //<token> id-item2 = 'item2';
            var item1Def = new NDefinition {
                Name = "id-item1",
                Nodes = {
                    new CharNode('i', "item1"),
                    new CharNode('t', "item1"),
                    new CharNode('e', "item1"),
                    new CharNode('m', "item1"),
                    new CharNode('1', "item1"),
                },
            };
            item1Def.Nodes[0].NextNodes.Add(item1Def.Nodes[1]);
            item1Def.Nodes[1].NextNodes.Add(item1Def.Nodes[2]);
            item1Def.Nodes[2].NextNodes.Add(item1Def.Nodes[3]);
            item1Def.Nodes[3].NextNodes.Add(item1Def.Nodes[4]);
            item1Def.Directives.Add(DefinitionDirective.Token);
            item1Def.Directives.Add(DefinitionDirective.Atomic);
            item1Def.Directives.Add(DefinitionDirective.MindWhitespace);
            item1Def.StartNodes.Add(item1Def.Nodes.First());
            item1Def.EndNodes.Add(item1Def.Nodes.Last());

            var item2Def = new NDefinition {
                Name = "id-item2",
                Nodes = {
                    new CharNode('i', "item2"),
                    new CharNode('t', "item2"),
                    new CharNode('e', "item2"),
                    new CharNode('m', "item2"),
                    new CharNode('2', "item2"),
                },
            };
            item2Def.Nodes[0].NextNodes.Add(item2Def.Nodes[1]);
            item2Def.Nodes[1].NextNodes.Add(item2Def.Nodes[2]);
            item2Def.Nodes[2].NextNodes.Add(item2Def.Nodes[3]);
            item2Def.Nodes[3].NextNodes.Add(item2Def.Nodes[4]);
            item2Def.Directives.Add(DefinitionDirective.Token);
            item2Def.Directives.Add(DefinitionDirective.Atomic);
            item2Def.Directives.Add(DefinitionDirective.MindWhitespace);
            item2Def.StartNodes.Add(item2Def.Nodes.First());
            item2Def.EndNodes.Add(item2Def.Nodes.Last());

            var itemDef = new NDefinition {
                Name = "item",
                Nodes = {
                    new DefRefNode(item1Def),
                    new DefRefNode(item2Def),
                },
            };
            itemDef.StartNodes.AddRange(itemDef.Nodes);
            itemDef.EndNodes.AddRange(itemDef.Nodes);

            var sequenceDef = new NDefinition {
                Name = "sequence",
                Nodes = {
                    new DefRefNode(itemDef),
                },
            };
            sequenceDef.Nodes[0].NextNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.StartNodes.AddRange(sequenceDef.Nodes);
            sequenceDef.EndNodes.AddRange(sequenceDef.Nodes);

            var testGrammar = new NGrammar();
            testGrammar.Definitions.AddRange(sequenceDef, itemDef, item1Def, item2Def);

            Tokenizer t = new Tokenizer(testGrammar, "item1 item2".ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            // assertions
            Assert.IsEmpty(tinfo.Errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.EqualTo(item1Def));
            Assert.That(first.StartPosition.Index, Is.EqualTo(0));
            Assert.That(first.Value, Is.EqualTo("item1"));

            tinfo = t.GetInputAtLocation(5);

            Assert.IsEmpty(tinfo.Errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.EqualTo(item2Def));
            Assert.That(first.StartPosition.Index, Is.EqualTo(6));
            Assert.That(first.Value, Is.EqualTo("item2"));
        }

        [Test()]
        public void TestAmbiguousSeparateTokens()
        {
            // setup

            // // test grammar
            //expr = item ( oper item )+;
            //item = ( varref | prefix | postfix );
            //prefix = plusplus varref;
            //postfix = varref plusplus;
            //<token> varref = [\l]+ ;
            //<token> plusplus = '++';
            //<token> oper = '+';
            var varrefDef = new NDefinition {
                Name = "varref",
                Nodes = {
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l"))
                },
            };
            varrefDef.Nodes[0].NextNodes.Add(varrefDef.Nodes[0]);
            varrefDef.StartNodes.Add(varrefDef.Nodes[0]);
            varrefDef.EndNodes.Add(varrefDef.Nodes[0]);
            varrefDef.Directives.Add(DefinitionDirective.Token);
            varrefDef.Directives.Add(DefinitionDirective.Atomic);
            varrefDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var plusplusDef = new NDefinition {
                Name = "plusplus",
                Nodes = {
                    new CharNode('+', "++"),
                    new CharNode('+', "++"),
                },
            };
            plusplusDef.Nodes[0].NextNodes.Add(plusplusDef.Nodes[1]);
            plusplusDef.StartNodes.Add(plusplusDef.Nodes.First());
            plusplusDef.EndNodes.Add(plusplusDef.Nodes.Last());
            plusplusDef.Directives.Add(DefinitionDirective.Token);
            plusplusDef.Directives.Add(DefinitionDirective.Atomic);
            plusplusDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var operDef = new NDefinition {
                Name = "open",
                Nodes = {
                    new CharNode('+'),
                },
            };
            operDef.StartNodes.Add(operDef.Nodes[0]);
            operDef.EndNodes.Add(operDef.Nodes[0]);
            operDef.Directives.Add(DefinitionDirective.Token);
            operDef.Directives.Add(DefinitionDirective.Atomic);
            operDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var postfixDef = new NDefinition {
                Name = "postfix",
                Nodes = {
                    new DefRefNode(varrefDef),
                    new DefRefNode(plusplusDef),
                },
            };
            postfixDef.Nodes[0].NextNodes.Add(postfixDef.Nodes[1]);
            postfixDef.StartNodes.Add(postfixDef.Nodes.First());
            postfixDef.EndNodes.Add(postfixDef.Nodes.Last());

            var prefixDef = new NDefinition {
                Name = "postfix",
                Nodes = {
                    new DefRefNode(plusplusDef),
                    new DefRefNode(varrefDef),
                },
            };
            prefixDef.Nodes[0].NextNodes.Add(prefixDef.Nodes[1]);
            prefixDef.StartNodes.Add(prefixDef.Nodes.First());
            prefixDef.EndNodes.Add(prefixDef.Nodes.Last());

            var itemDef = new NDefinition {
                Name = "item",
                Nodes = {
                    new DefRefNode(varrefDef),
                    new DefRefNode(prefixDef),
                    new DefRefNode(postfixDef),
                },
            };
            itemDef.StartNodes.AddRange(itemDef.Nodes);
            itemDef.EndNodes.AddRange(itemDef.Nodes);

            var exprDef = new NDefinition {
                Name = "expr",
                Nodes = {
                    new DefRefNode(itemDef),
                    new DefRefNode(operDef),
                    new DefRefNode(itemDef),
                },
            };
            exprDef.Nodes[0].NextNodes.Add(exprDef.Nodes[1]);
            exprDef.Nodes[1].NextNodes.Add(exprDef.Nodes[2]);
            exprDef.Nodes[2].NextNodes.Add(exprDef.Nodes[1]);
            exprDef.StartNodes.Add(exprDef.Nodes[0]);
            exprDef.EndNodes.Add(exprDef.Nodes[2]);

            string testInput = "a+++b";

            var errors = new List<Error>();

            var testGrammar = new NGrammar();
            testGrammar.Definitions.AddRange(exprDef, itemDef, prefixDef, postfixDef, operDef, plusplusDef, varrefDef);

            Tokenizer t = new Tokenizer(testGrammar, testInput.ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            //assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.EqualTo(varrefDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(0));
            Assert.That(first.Value, Is.EqualTo("a"));

            // action
            tinfo = t.GetInputAtLocation(1);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(2));
            first = tinfo.InputElements.First();
            var second = tinfo.InputElements.ElementAt(1);
            Assert.IsTrue(first.Definition == plusplusDef || first.Definition == operDef);
            Assert.IsTrue(second.Definition == plusplusDef || second.Definition == operDef);
            Assert.IsFalse(first.Definition == second.Definition);

            Token plusplusToken;
            Token operToken;
            if (first.Definition == plusplusDef)
            {
                plusplusToken = first;
                operToken = second;
            }
            else
            {
                plusplusToken = second;
                operToken = first;
            }

            Assert.That(plusplusToken.StartPosition.Index, Is.EqualTo(1));
            Assert.That(plusplusToken.Value, Is.EqualTo("++"));
            Assert.That(operToken.StartPosition.Index, Is.EqualTo(1));
            Assert.That(operToken.Value, Is.EqualTo("+"));
        }

        [Test()]
        public void TestAmbiguousCombinedToken()
        {
            // setup

            // // test grammar
            //expr = item ( oper item )+;
            //<token> item = [\\l]+ ;
            //<token> oper = ( '<' | '<<' );

            var itemDef = new NDefinition {
                Name = "item",
                Nodes = {
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l"))
                },
            };
            itemDef.Nodes[0].NextNodes.Add(itemDef.Nodes[0]);
            itemDef.StartNodes.Add(itemDef.Nodes[0]);
            itemDef.EndNodes.Add(itemDef.Nodes[0]);
            itemDef.Directives.Add(DefinitionDirective.Token);
            itemDef.Directives.Add(DefinitionDirective.Atomic);
            itemDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var operDef = new NDefinition {
                Name = "oper",
                Nodes = {
                    new CharNode('<', "<"),
                    new CharNode('<', "<<"),
                    new CharNode('<', "<<"),
                },
            };
            operDef.Nodes[1].NextNodes.Add(operDef.Nodes[2]);
            operDef.StartNodes.Add(operDef.Nodes[0]);
            operDef.StartNodes.Add(operDef.Nodes[1]);
            operDef.EndNodes.Add(operDef.Nodes[0]);
            operDef.EndNodes.Add(operDef.Nodes[2]);
            operDef.Directives.Add(DefinitionDirective.Token);
            operDef.Directives.Add(DefinitionDirective.Atomic);
            operDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var exprDef = new NDefinition {
                Name = "expr",
                Nodes = {
                    new DefRefNode(itemDef),
                    new DefRefNode(operDef),
                    new DefRefNode(itemDef),
                },
            };
            exprDef.Nodes[0].NextNodes.Add(exprDef.Nodes[1]);
            exprDef.Nodes[1].NextNodes.Add(exprDef.Nodes[2]);
            exprDef.Nodes[2].NextNodes.Add(exprDef.Nodes[1]);
            exprDef.StartNodes.Add(exprDef.Nodes[0]);
            exprDef.EndNodes.Add(exprDef.Nodes[2]);

            string testInput = "a << b";

            var errors = new List<Error>();
            var testGrammar = new NGrammar();
            testGrammar.Definitions.AddRange(exprDef, itemDef, operDef);

            Tokenizer t = new Tokenizer(testGrammar, testInput.ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.EqualTo(itemDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(0));
            Assert.That(first.Value, Is.EqualTo("a"));

            // action
            tinfo = t.GetInputAtLocation(1);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(2));
            first = tinfo.InputElements.First();
            var second = tinfo.InputElements.ElementAt(1);
            Assert.That(first.StartPosition.Index, Is.EqualTo(2));
            Assert.That(second.StartPosition.Index, Is.EqualTo(2));
            Assert.That(first.Definition, Is.EqualTo(operDef));
            Assert.That(second.Definition, Is.EqualTo(operDef));
            Assert.IsTrue(first.Value == "<" || first.Value == "<<");
            Assert.IsTrue(second.Value == "<" || second.Value == "<<");
            Assert.IsTrue(first.Value != second.Value);
        }

        static NGrammar CreateGrammarForTestTokensAtIndex()
        {
            var operandDef = new NDefinition {
                Name = "operand",
                Nodes = {
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l_")),
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d_")),
                },
            };
            operandDef.Nodes[0].NextNodes.Add(operandDef.Nodes[1]);
            operandDef.Nodes[1].NextNodes.Add(operandDef.Nodes[1]);
            operandDef.StartNodes.Add(operandDef.Nodes[0]);
            operandDef.EndNodes.AddRange(operandDef.Nodes);
            operandDef.Directives.Add(DefinitionDirective.Token);
            operandDef.Directives.Add(DefinitionDirective.Atomic);
            operandDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var implicitPlusDef = new NDefinition {
                Name = "$implicit literal +",
                Nodes = {
                    new CharNode('+'),
                },
            };
            implicitPlusDef.StartNodes.Add(implicitPlusDef.Nodes[0]);
            implicitPlusDef.EndNodes.Add(implicitPlusDef.Nodes[0]);
            implicitPlusDef.Directives.Add(DefinitionDirective.Token);
            implicitPlusDef.Directives.Add(DefinitionDirective.Atomic);
            implicitPlusDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var exprDef = new NDefinition {
                Name = "expr",
                Nodes = {
                    new DefRefNode(operandDef),
                    new DefRefNode(implicitPlusDef),
                    new DefRefNode(operandDef),
                },
            };
            exprDef.Nodes[0].NextNodes.Add(exprDef.Nodes[1]);
            exprDef.Nodes[1].NextNodes.Add(exprDef.Nodes[2]);
            exprDef.StartNodes.Add(exprDef.Nodes.First());
            exprDef.EndNodes.Add(exprDef.Nodes.Last());


            var grammar = new NGrammar();
            grammar.Definitions.AddRange(exprDef, implicitPlusDef, operandDef);

            return grammar;
        }

        [Test]
        public void TestTokensAtIndex0()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            var operandDef = grammar.FindDefinitionByName("operand");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(0);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(operandDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(0));
            Assert.That(first.Value, Is.EqualTo("a"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
        }

        [Test]
        public void TestTokensAtIndex1()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            var operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(1);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(operatorDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(2));
            Assert.That(first.Value, Is.EqualTo("+"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
        }

        [Test]
        public void TestTokensAtIndex2()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            var operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(2);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(operatorDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(2));
            Assert.That(first.Value, Is.EqualTo("+"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
        }

        [Test]
        public void TestTokensAtIndex3()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            var operandDef = grammar.FindDefinitionByName("operand");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(3);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(operandDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(4));
            Assert.That(first.Value, Is.EqualTo("b"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
        }

        [Test]
        public void TestTokensAtIndex4()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            var operandDef = grammar.FindDefinitionByName("operand");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(4);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(operandDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(4));
            Assert.That(first.Value, Is.EqualTo("b"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
        }

        [Test]
        public void TestTokensAtIndex5()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(0));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(5));
        }

        [Test]
        public void TestEndOfInputParameter1()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();
            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(5));
        }

        [Test]
        public void TestEndOfInputParameter2()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();
            string input = "a + b ";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(6));
        }

        [Test]
        public void TestEndOfInputParameter3()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();
            string input = "a + b ";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(6);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(6));
        }

        static NGrammar CreateGrammarForTestEndOfInputParameterWithComment()
        {
            var grammar = CreateGrammarForTestTokensAtIndex();

            var commentDef = new NDefinition {
                Name = "comment",
                Nodes = {
                    new CharNode('/', "/*"),
                    new CharNode('*', "/*"),
                    new CharNode(CharClass.FromUndelimitedCharClassText("^*")),
                    new CharNode('*', "*/"),
                    new CharNode('/', "*/"),
                },
            };
            commentDef.Nodes[0].NextNodes.Add(commentDef.Nodes[1]);
            commentDef.Nodes[1].NextNodes.Add(commentDef.Nodes[2]);
            commentDef.Nodes[2].NextNodes.Add(commentDef.Nodes[2]);
            commentDef.Nodes[2].NextNodes.Add(commentDef.Nodes[3]);
            commentDef.Nodes[1].NextNodes.Add(commentDef.Nodes[3]);
            commentDef.Nodes[3].NextNodes.Add(commentDef.Nodes[4]);
            commentDef.StartNodes.Add(commentDef.Nodes.First());
            commentDef.EndNodes.Add(commentDef.Nodes.Last());
            commentDef.Directives.Add(DefinitionDirective.Comment);
            commentDef.Directives.Add(DefinitionDirective.Atomic);
            commentDef.Directives.Add(DefinitionDirective.MindWhitespace);

            grammar.Definitions.Add(commentDef);

            return grammar;
        }

        [Test]
        public void TestEndOfInputParameterWithComment1()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;
            //<comment> comment = '/*' [^*]* '*/';

            var grammar = CreateGrammarForTestEndOfInputParameterWithComment();

            string input = "a + b/*comment*/";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(16));
        }

        [Test]
        public void TestEndOfInputParameterWithComment2()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;
            //<comment> comment = '/*' [^*]* '*/';

            var grammar = CreateGrammarForTestEndOfInputParameterWithComment();
            string input = "a + b /*comment*/";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(17));
        }

        [Test]
        public void TestEndOfInputParameterWithComment3()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;
            //<comment> comment = '/*' [^*]* '*/';

            var grammar = CreateGrammarForTestEndOfInputParameterWithComment();
            string input = "a + b/*comment*/ ";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.IsEmpty(tinfo.InputElements);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(17));
        }

        [Test]
        public void TestEndOfInputParameterWithAmbiguousComment()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;
            //<comment> comment = '/*' [^*]* '*/';
            //<token> strange = '/*' [\\l]+;

            var grammar = CreateGrammarForTestEndOfInputParameterWithComment();
            var strangeDef = new NDefinition {
                Name = "strange",
                Nodes = {
                    new CharNode('/', "/*"),
                    new CharNode('*', "/*"),
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l")),
                }
            };
            strangeDef.Nodes[0].NextNodes.Add(strangeDef.Nodes[1]);
            strangeDef.Nodes[1].NextNodes.Add(strangeDef.Nodes[2]);
            strangeDef.Nodes[2].NextNodes.Add(strangeDef.Nodes[2]);
            strangeDef.StartNodes.Add(strangeDef.Nodes.First());
            strangeDef.EndNodes.Add(strangeDef.Nodes.Last());
            strangeDef.Directives.Add(DefinitionDirective.Token);
            strangeDef.Directives.Add(DefinitionDirective.Atomic);
            strangeDef.Directives.Add(DefinitionDirective.MindWhitespace);

            grammar.Definitions.Add(strangeDef);

            string input = "a + b/*comment*/";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(5);

            // assertions
            Assert.IsTrue(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(strangeDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(5));
            Assert.That(first.Value, Is.EqualTo("/*comment"));
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(16));
        }

        [Test]
        public void TestAmbiguousSubtokens()
        {
            // item is a token and middle is a subtoken. middle is not atomic,
            // therefore it should run into the 2^N explosion and produce
            // multiple spans. however, becase the spans all start and end at
            // the same indexes, it should only result in one token.

            // setup

            //sequence = item+ ;
            //<token> item = 'start-' middle+ '-end' ;
            //<subtoken> middle = [\\l]+ ;

            string testInputText = "start-ABCD-end";

            var middleDef = new NDefinition {
                Name = "middle",
                Nodes = {
                    new CharNode(CharClass.FromUndelimitedCharClassText("\\l"))
                }
            };
            middleDef.Nodes[0].NextNodes.Add(middleDef.Nodes[0]);
            middleDef.StartNodes.Add(middleDef.Nodes[0]);
            middleDef.EndNodes.Add(middleDef.Nodes[0]);
            middleDef.Directives.Add(DefinitionDirective.Subtoken);
            middleDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var itemDef = new NDefinition {
                Name = "item",
                Nodes = {
                    new CharNode('s', "start-"),
                    new CharNode('t', "start-"),
                    new CharNode('a', "start-"),
                    new CharNode('r', "start-"),
                    new CharNode('t', "start-"),
                    new CharNode('-', "start-"),
                    new DefRefNode(middleDef),
                    new CharNode('-', "-end"),
                    new CharNode('e', "-end"),
                    new CharNode('n', "-end"),
                    new CharNode('d', "-end"),
                }
            };
            itemDef.Nodes[0].NextNodes.Add(itemDef.Nodes[1]);
            itemDef.Nodes[1].NextNodes.Add(itemDef.Nodes[2]);
            itemDef.Nodes[2].NextNodes.Add(itemDef.Nodes[3]);
            itemDef.Nodes[3].NextNodes.Add(itemDef.Nodes[4]);
            itemDef.Nodes[4].NextNodes.Add(itemDef.Nodes[5]);
            itemDef.Nodes[5].NextNodes.Add(itemDef.Nodes[6]);
            itemDef.Nodes[6].NextNodes.Add(itemDef.Nodes[6]);
            itemDef.Nodes[6].NextNodes.Add(itemDef.Nodes[7]);
            itemDef.Nodes[7].NextNodes.Add(itemDef.Nodes[8]);
            itemDef.Nodes[8].NextNodes.Add(itemDef.Nodes[9]);
            itemDef.Nodes[9].NextNodes.Add(itemDef.Nodes[10]);
            itemDef.StartNodes.Add(itemDef.Nodes.First());
            itemDef.EndNodes.Add(itemDef.Nodes.Last());
            itemDef.Directives.Add(DefinitionDirective.Token);
            itemDef.Directives.Add(DefinitionDirective.Atomic);
            itemDef.Directives.Add(DefinitionDirective.MindWhitespace);

            var sequenceDef = new NDefinition {
                Name = "sequence",
                Nodes = {
                    new DefRefNode(itemDef),
                }
            };
            sequenceDef.Nodes[0].NextNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.StartNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.EndNodes.Add(sequenceDef.Nodes[0]);

            var grammar = new NGrammar();
            grammar.Definitions.AddRange(sequenceDef, itemDef, middleDef);

            Tokenizer tokenizer = new Tokenizer(grammar, testInputText.ToCharacterSource());


            var tinfo = tokenizer.GetInputAtLocation(0);


            Assert.IsFalse(tinfo.EndOfInput);
            Assert.That(tinfo.EndOfInputPosition.Index, Is.EqualTo(-1));
            Assert.IsNotNull(tinfo.InputElements);
            Assert.That(tinfo.InputElements.Count(), Is.EqualTo(1));
            var first = tinfo.InputElements.First();
            Assert.That(first.Definition, Is.SameAs(itemDef));
            Assert.That(first.StartPosition.Index, Is.EqualTo(0));
            Assert.That(first.Value, Is.EqualTo("start-ABCD-end"));
        }

        [Test]
        public void TestReadInOrder()
        {
            // setup
            var grammarText = "sequence = ('a' | 'ab' | 'abc' | 'bca' | 'bc' | 'cab' | 'c')+;";
            var errors = new List<Error>();
            var g = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);
            var g2 = (new TokenizeTransform()).Tokenize(g);
            var gc = new GrammarCompiler();
            var grammar = gc.Compile(g2);
            var implicitA = grammar.FindDefinitionByName("$implicit literal a");
            var implicitAb = grammar.FindDefinitionByName("$implicit literal ab");
            var implicitAbc = grammar.FindDefinitionByName("$implicit literal abc");
            var implicitBca = grammar.FindDefinitionByName("$implicit literal bca");
            var implicitCab = grammar.FindDefinitionByName("$implicit literal cab");
            var implicitBc = grammar.FindDefinitionByName("$implicit literal bc");
            var implicitC = grammar.FindDefinitionByName("$implicit literal c");
            var input = "abcabc";
            var tokenizer = new Tokenizer(grammar, input.ToCharacterSource());
            InputElementSet<Token> ies = null;
            Token token;

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(3));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitA));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAb));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAbc));
            token = ies.InputElements.First(x => x.Definition == implicitA);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(1));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(0, 1, 1)));
            Assert.That(token.Value, Is.EqualTo("a"));
            token = ies.InputElements.First(x => x.Definition == implicitAb);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(2));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(0, 1, 1)));
            Assert.That(token.Value, Is.EqualTo("ab"));
            token = ies.InputElements.First(x => x.Definition == implicitAbc);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(3));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(0, 1, 1)));
            Assert.That(token.Value, Is.EqualTo("abc"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(1));
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(2));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitBc));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitBca));
            token = ies.InputElements.First(x => x.Definition == implicitBc);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(3));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(1, 1, 2)));
            Assert.That(token.Value, Is.EqualTo("bc"));
            token = ies.InputElements.First(x => x.Definition == implicitBca);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(4));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(1, 1, 2)));
            Assert.That(token.Value, Is.EqualTo("bca"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(2));
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(2));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitC));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitCab));
            token = ies.InputElements.First(x => x.Definition == implicitC);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(3));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(2, 1, 3)));
            Assert.That(token.Value, Is.EqualTo("c"));
            token = ies.InputElements.First(x => x.Definition == implicitCab);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(5));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(2, 1, 3)));
            Assert.That(token.Value, Is.EqualTo("cab"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(3));
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(3));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitA));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAb));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAbc));
            token = ies.InputElements.First(x => x.Definition == implicitA);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(4));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(3, 1, 4)));
            Assert.That(token.Value, Is.EqualTo("a"));
            token = ies.InputElements.First(x => x.Definition == implicitAb);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(5));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(3, 1, 4)));
            Assert.That(token.Value, Is.EqualTo("ab"));
            token = ies.InputElements.First(x => x.Definition == implicitAbc);
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(6));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(3, 1, 4)));
            Assert.That(token.Value, Is.EqualTo("abc"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(4));
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            token = ies.InputElements.First();
            Assert.That(token.Definition, Is.SameAs(implicitBc));
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(6));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(4, 1, 5)));
            Assert.That(token.Value, Is.EqualTo("bc"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(5));
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            token = ies.InputElements.First();
            Assert.That(token.Definition, Is.SameAs(implicitC));
            Assert.That(token.IndexOfNextTokenization, Is.EqualTo(6));
            Assert.That(token.StartPosition,
                Is.EqualTo(new InputPosition(5, 1, 6)));
            Assert.That(token.Value, Is.EqualTo("c"));
            Assert.That(tokenizer.CurrentPosition.Index, Is.EqualTo(6));
            Assert.IsTrue(tokenizer.IsAtEnd);


        }
    }
}

