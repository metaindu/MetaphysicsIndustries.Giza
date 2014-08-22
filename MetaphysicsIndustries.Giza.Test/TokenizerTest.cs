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
            var item1Def = new Definition {
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

            var item2Def = new Definition {
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

            var itemDef = new Definition {
                Name = "item",
                Nodes = {
                    new DefRefNode(item1Def),
                    new DefRefNode(item2Def),
                },
            };
            itemDef.StartNodes.AddRange(itemDef.Nodes);
            itemDef.EndNodes.AddRange(itemDef.Nodes);

            var sequenceDef = new Definition {
                Name = "sequence",
                Nodes = {
                    new DefRefNode(itemDef),
                },
            };
            sequenceDef.Nodes[0].NextNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.StartNodes.AddRange(sequenceDef.Nodes);
            sequenceDef.EndNodes.AddRange(sequenceDef.Nodes);

            var testGrammar = new Grammar();
            testGrammar.Definitions.AddRange(sequenceDef, itemDef, item1Def, item2Def);

            Tokenizer t = new Tokenizer(testGrammar, "item1 item2".ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            // assertions
            Assert.IsEmpty(tinfo.Errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreEqual(item1Def, first.Definition);
            Assert.AreEqual(0, first.StartPosition.Index);
            Assert.AreEqual("item1", first.Value);

            tinfo = t.GetInputAtLocation(5);

            Assert.IsEmpty(tinfo.Errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            first = tinfo.InputElements.First();
            Assert.AreEqual(item2Def, first.Definition);
            Assert.AreEqual(6, first.StartPosition.Index);
            Assert.AreEqual("item2", first.Value);
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
            var varrefDef = new Definition {
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

            var plusplusDef = new Definition {
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

            var operDef = new Definition {
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

            var postfixDef = new Definition {
                Name = "postfix",
                Nodes = {
                    new DefRefNode(varrefDef),
                    new DefRefNode(plusplusDef),
                },
            };
            postfixDef.Nodes[0].NextNodes.Add(postfixDef.Nodes[1]);
            postfixDef.StartNodes.Add(postfixDef.Nodes.First());
            postfixDef.EndNodes.Add(postfixDef.Nodes.Last());

            var prefixDef = new Definition {
                Name = "postfix",
                Nodes = {
                    new DefRefNode(plusplusDef),
                    new DefRefNode(varrefDef),
                },
            };
            prefixDef.Nodes[0].NextNodes.Add(prefixDef.Nodes[1]);
            prefixDef.StartNodes.Add(prefixDef.Nodes.First());
            prefixDef.EndNodes.Add(prefixDef.Nodes.Last());

            var itemDef = new Definition {
                Name = "item",
                Nodes = {
                    new DefRefNode(varrefDef),
                    new DefRefNode(prefixDef),
                    new DefRefNode(postfixDef),
                },
            };
            itemDef.StartNodes.AddRange(itemDef.Nodes);
            itemDef.EndNodes.AddRange(itemDef.Nodes);

            var exprDef = new Definition {
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

            Grammar testGrammar = new Grammar();
            testGrammar.Definitions.AddRange(exprDef, itemDef, prefixDef, postfixDef, operDef, plusplusDef, varrefDef);

            Tokenizer t = new Tokenizer(testGrammar, testInput.ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            //assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreEqual(varrefDef, first.Definition);
            Assert.AreEqual(0, first.StartPosition.Index);
            Assert.AreEqual("a", first.Value);

            // action
            tinfo = t.GetInputAtLocation(1);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(2, tinfo.InputElements.Count());
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

            Assert.AreEqual(1, plusplusToken.StartPosition.Index);
            Assert.AreEqual("++", plusplusToken.Value);
            Assert.AreEqual(1, operToken.StartPosition.Index);
            Assert.AreEqual("+", operToken.Value);
        }

        [Test()]
        public void TestAmbiguousCombinedToken()
        {
            // setup

            // // test grammar
            //expr = item ( oper item )+;
            //<token> item = [\\l]+ ;
            //<token> oper = ( '<' | '<<' );

            var itemDef = new Definition {
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

            var operDef = new Definition {
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

            var exprDef = new Definition {
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
            var testGrammar = new Grammar();
            testGrammar.Definitions.AddRange(exprDef, itemDef, operDef);

            Tokenizer t = new Tokenizer(testGrammar, testInput.ToCharacterSource());

            // action
            var tinfo = t.GetInputAtLocation(0);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreEqual(itemDef, first.Definition);
            Assert.AreEqual(0, first.StartPosition.Index);
            Assert.AreEqual("a", first.Value);

            // action
            tinfo = t.GetInputAtLocation(1);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(2, tinfo.InputElements.Count());
            first = tinfo.InputElements.First();
            var second = tinfo.InputElements.ElementAt(1);
            Assert.AreEqual(2, first.StartPosition.Index);
            Assert.AreEqual(2, second.StartPosition.Index);
            Assert.AreEqual(operDef, first.Definition);
            Assert.AreEqual(operDef, second.Definition);
            Assert.IsTrue(first.Value == "<" || first.Value == "<<");
            Assert.IsTrue(second.Value == "<" || second.Value == "<<");
            Assert.IsTrue(first.Value != second.Value);
        }

        static Grammar CreateGrammarForTestTokensAtIndex()
        {
            var operandDef = new Definition {
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

            var implicitPlusDef = new Definition {
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

            var exprDef = new Definition {
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


            var grammar = new Grammar();
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
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(operandDef, first.Definition);
            Assert.AreEqual(0, first.StartPosition.Index);
            Assert.AreEqual("a", first.Value);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
        }

        [Test]
        public void TestTokensAtIndex1()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(1);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(operatorDef, first.Definition);
            Assert.AreEqual(2, first.StartPosition.Index);
            Assert.AreEqual("+", first.Value);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
        }

        [Test]
        public void TestTokensAtIndex2()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(2);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(operatorDef, first.Definition);
            Assert.AreEqual(2, first.StartPosition.Index);
            Assert.AreEqual("+", first.Value);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
        }

        [Test]
        public void TestTokensAtIndex3()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            Definition operandDef = grammar.FindDefinitionByName("operand");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(3);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(operandDef, first.Definition);
            Assert.AreEqual(4, first.StartPosition.Index);
            Assert.AreEqual("b", first.Value);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
        }

        [Test]
        public void TestTokensAtIndex4()
        {
            // setup

            //expr = operand '+' operand;
            //<token> operand = [\\l_] [\\l\\d_]*;

            var grammar = CreateGrammarForTestTokensAtIndex();

            Definition operandDef = grammar.FindDefinitionByName("operand");

            string input = "a + b";
            Tokenizer tokenizer = new Tokenizer(grammar, input.ToCharacterSource());

            // action
            var tinfo = tokenizer.GetInputAtLocation(4);

            // assertions
            Assert.IsFalse(tinfo.EndOfInput);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(operandDef, first.Definition);
            Assert.AreEqual(4, first.StartPosition.Index);
            Assert.AreEqual("b", first.Value);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(0, tinfo.InputElements.Count());
            Assert.AreEqual(5, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(5, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(6, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(6, tinfo.EndOfInputPosition.Index);
        }

        static Grammar CreateGrammarForTestEndOfInputParameterWithComment()
        {
            var grammar = CreateGrammarForTestTokensAtIndex();

            var commentDef = new Definition {
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
            Assert.AreEqual(16, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(17, tinfo.EndOfInputPosition.Index);
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
            Assert.AreEqual(17, tinfo.EndOfInputPosition.Index);
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
            var strangeDef = new Definition {
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
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(strangeDef, first.Definition);
            Assert.AreEqual(5, first.StartPosition.Index);
            Assert.AreEqual("/*comment", first.Value);
            Assert.AreEqual(16, tinfo.EndOfInputPosition.Index);
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

            var middleDef = new Definition {
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

            var itemDef = new Definition {
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

            var sequenceDef = new Definition {
                Name = "sequence",
                Nodes = {
                    new DefRefNode(itemDef),
                }
            };
            sequenceDef.Nodes[0].NextNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.StartNodes.Add(sequenceDef.Nodes[0]);
            sequenceDef.EndNodes.Add(sequenceDef.Nodes[0]);

            var grammar = new Grammar();
            grammar.Definitions.AddRange(sequenceDef, itemDef, middleDef);

            Tokenizer tokenizer = new Tokenizer(grammar, testInputText.ToCharacterSource());


            var tinfo = tokenizer.GetInputAtLocation(0);


            Assert.IsFalse(tinfo.EndOfInput);
            Assert.AreEqual(-1, tinfo.EndOfInputPosition.Index);
            Assert.IsNotNull(tinfo.InputElements);
            Assert.AreEqual(1, tinfo.InputElements.Count());
            var first = tinfo.InputElements.First();
            Assert.AreSame(itemDef, first.Definition);
            Assert.AreEqual(0, first.StartPosition.Index);
            Assert.AreEqual("start-ABCD-end", first.Value);
        }

        [Test]
        public void TestReadInOrder()
        {
            // setup
            var grammarText = "sequence = ('a' | 'ab' | 'abc' | 'bca' | 'bc' | 'cab' | 'c')+;";
            var errors = new List<Error>();
            var defs = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);
            var grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(defs);
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
            Assert.AreEqual(3, ies.InputElements.Count());
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitA));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAb));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAbc));
            token = ies.InputElements.First(x => x.Definition == implicitA);
            Assert.AreEqual(1, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(0, 1, 1), token.StartPosition);
            Assert.AreEqual("a", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitAb);
            Assert.AreEqual(2, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(0, 1, 1), token.StartPosition);
            Assert.AreEqual("ab", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitAbc);
            Assert.AreEqual(3, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(0, 1, 1), token.StartPosition);
            Assert.AreEqual("abc", token.Value);
            Assert.AreEqual(1, tokenizer.CurrentPosition.Index);
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(2, ies.InputElements.Count());
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitBc));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitBca));
            token = ies.InputElements.First(x => x.Definition == implicitBc);
            Assert.AreEqual(3, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(1, 1, 2), token.StartPosition);
            Assert.AreEqual("bc", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitBca);
            Assert.AreEqual(4, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(1, 1, 2), token.StartPosition);
            Assert.AreEqual("bca", token.Value);
            Assert.AreEqual(2, tokenizer.CurrentPosition.Index);
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(2, ies.InputElements.Count());
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitC));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitCab));
            token = ies.InputElements.First(x => x.Definition == implicitC);
            Assert.AreEqual(3, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(2, 1, 3), token.StartPosition);
            Assert.AreEqual("c", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitCab);
            Assert.AreEqual(5, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(2, 1, 3), token.StartPosition);
            Assert.AreEqual("cab", token.Value);
            Assert.AreEqual(3, tokenizer.CurrentPosition.Index);
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(3, ies.InputElements.Count());
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitA));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAb));
            Assert.IsTrue(ies.InputElements.Any(x => x.Definition == implicitAbc));
            token = ies.InputElements.First(x => x.Definition == implicitA);
            Assert.AreEqual(4, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(3, 1, 4), token.StartPosition);
            Assert.AreEqual("a", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitAb);
            Assert.AreEqual(5, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(3, 1, 4), token.StartPosition);
            Assert.AreEqual("ab", token.Value);
            token = ies.InputElements.First(x => x.Definition == implicitAbc);
            Assert.AreEqual(6, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(3, 1, 4), token.StartPosition);
            Assert.AreEqual("abc", token.Value);
            Assert.AreEqual(4, tokenizer.CurrentPosition.Index);
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            token = ies.InputElements.First();
            Assert.AreSame(implicitBc, token.Definition);
            Assert.AreEqual(6, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(4, 1, 5), token.StartPosition);
            Assert.AreEqual("bc", token.Value);
            Assert.AreEqual(5, tokenizer.CurrentPosition.Index);
            Assert.IsFalse(tokenizer.IsAtEnd);

            // action
            ies = tokenizer.GetNextValue();

            // assertions
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            token = ies.InputElements.First();
            Assert.AreSame(implicitC, token.Definition);
            Assert.AreEqual(6, token.IndexOfNextTokenization);
            Assert.AreEqual(new InputPosition(5, 1, 6), token.StartPosition);
            Assert.AreEqual("c", token.Value);
            Assert.AreEqual(6, tokenizer.CurrentPosition.Index);
            Assert.IsTrue(tokenizer.IsAtEnd);


        }
    }
}

