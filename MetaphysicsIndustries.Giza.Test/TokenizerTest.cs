using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class TokenizerTest
    {
        [Test()]
        public void TestNormal()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 ); \r\n" +
                "<mind whitespace, atomic, token> id-item1 = 'item1'; \r\n" +
                "<mind whitespace, atomic, token> id-item2 = 'item2'; \r\n";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Grammar testGrammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Definition item1Def = testGrammar.FindDefinitionByName("id-item1");
            Definition item2Def = testGrammar.FindDefinitionByName("id-item2");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;
            bool endOfInput;
            int lastIndex;
            tokens = t.GetTokensAtLocation("item1 item2", 0, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(item1Def, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(5, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation("item1 item2", 5, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(item2Def, tokens[0].Definition);
                Assert.AreEqual(6, tokens[0].StartIndex);
                Assert.AreEqual(5, tokens[0].Length);
            }
        }

        [Test()]
        public void TestAmbiguousSeparateTokens()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "expr = item ( oper item )+; \r\n" +
                "item = ( varref | prefix | postfix ); \r\n" +
                "prefix = plusplus varref; \r\n" +
                "postfix = varref plusplus; \r\n" +
                "<mind whitespace, atomic, token> varref = [\\l]+ ; \r\n" +
                "<mind whitespace, token> plusplus = '++'; \r\n" +
                "<token> oper = '+'; \r\n";

            string testInput = "a+++b";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Grammar testGrammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Definition varrefDef = testGrammar.FindDefinitionByName("varref");
            Definition plusplusDef = testGrammar.FindDefinitionByName("plusplus");
            Definition operDef = testGrammar.FindDefinitionByName("oper");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;
            bool endOfInput;
            int lastIndex;

            tokens = t.GetTokensAtLocation(testInput, 0, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(varrefDef, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(1, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation(testInput, 1, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(2, tokens.Length);
            Assert.IsTrue(tokens[0].Definition == plusplusDef || tokens[0].Definition == operDef);
            Assert.IsTrue(tokens[1].Definition == plusplusDef || tokens[1].Definition == operDef);
            Assert.IsFalse(tokens[0].Definition == tokens[1].Definition);

            Token plusplusToken;
            Token operToken;
            if (tokens[0].Definition == plusplusDef)
            {
                plusplusToken = tokens[0];
                operToken = tokens[1];
            }
            else
            {
                plusplusToken = tokens[1];
                operToken = tokens[0];
            }

            Assert.AreEqual(1, plusplusToken.StartIndex);
            Assert.AreEqual(2, plusplusToken.Length);
            Assert.AreEqual(1, operToken.StartIndex);
            Assert.AreEqual(1, operToken.Length);
        }

        [Test()]
        public void TestAmbiguousCombinedToken()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "expr = item ( oper item )+; \r\n" +
                "<mind whitespace, atomic, token> item = [\\l]+ ; \r\n" +
                "<mind whitespace, token> oper = ( '<' | '<<' ); \r\n";

            string testInput = "a << b";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(testGrammarText, errors); 
            Assert.IsEmpty(errors);
            TokenizedGrammarBuilder tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);


            Definition itemDef = testGrammar.FindDefinitionByName("item");
            Definition operDef = testGrammar.FindDefinitionByName("oper");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;
            bool endOfInput;
            int lastIndex;

            tokens = t.GetTokensAtLocation(testInput, 0, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(itemDef, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(1, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation(testInput, 1, errors,
                                           out endOfInput, out lastIndex);
            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.AreEqual(2, tokens.Length);
            Assert.AreEqual(2, tokens[0].StartIndex);
            Assert.AreEqual(2, tokens[1].StartIndex);
            Assert.AreEqual(operDef, tokens[0].Definition);
            Assert.AreEqual(operDef, tokens[1].Definition);
            Assert.IsTrue(tokens[0].Length == 1 || tokens[0].Length == 2);
            Assert.IsTrue(tokens[1].Length == 1 || tokens[1].Length == 2);
            Assert.IsTrue(tokens[0].Length != tokens[1].Length);
        }

        [Test]
        public void TestTokensAtIndex0()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition operandDef = grammar.FindDefinitionByName("operand");

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 0, errors,
                                                       out endOfInput,
                                                       out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(operandDef, tokens[0].Definition);
            Assert.AreEqual(0, tokens[0].StartIndex);
            Assert.AreEqual(1, tokens[0].Length);
            Assert.AreEqual(-1, endOfInputIndex);
        }

        [Test]
        public void TestTokensAtIndex1()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 1, errors,
                                                       out endOfInput,
                                                       out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(operatorDef, tokens[0].Definition);
            Assert.AreEqual(2, tokens[0].StartIndex);
            Assert.AreEqual(1, tokens[0].Length);
            Assert.AreEqual(-1, endOfInputIndex);
        }

        [Test]
        public void TestTokensAtIndex2()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 2, errors,
                                                       out endOfInput,
                                                       out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(operatorDef, tokens[0].Definition);
            Assert.AreEqual(2, tokens[0].StartIndex);
            Assert.AreEqual(1, tokens[0].Length);
            Assert.AreEqual(-1, endOfInputIndex);
        }

        [Test]
        public void TestTokensAtIndex3()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition operandDef = grammar.FindDefinitionByName("operand");

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 3, errors,
                                                   out endOfInput,
                                                   out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(operandDef, tokens[0].Definition);
            Assert.AreEqual(4, tokens[0].StartIndex);
            Assert.AreEqual(1, tokens[0].Length);
            Assert.AreEqual(-1, endOfInputIndex);
        }

        [Test]
        public void TestTokensAtIndex4()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition operandDef = grammar.FindDefinitionByName("operand");

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 4, errors,
                                                   out endOfInput,
                                                   out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsFalse(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(operandDef, tokens[0].Definition);
            Assert.AreEqual(4, tokens[0].StartIndex);
            Assert.AreEqual(1, tokens[0].Length);
            Assert.AreEqual(-1, endOfInputIndex);
        }

        [Test]
        public void TestTokensAtIndex5()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int endOfInputIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                   out endOfInput,
                                                   out endOfInputIndex);

            Assert.IsEmpty(errors);
            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(0, tokens.Length);
            Assert.AreEqual(5, endOfInputIndex);
        }

        [Test]
        public void TestEndOfInputParameter1()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(5, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameter2()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b ";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(6, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameter3()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b ";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 6, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(6, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameterWithComment1()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;\n" +
                "<comment> comment = '/*' [^*]* '*/';";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b/*comment*/";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(16, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameterWithComment2()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;\n" +
                    "<comment> comment = '/*' [^*]* '*/';";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b /*comment*/";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(17, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameterWithComment3()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;\n" +
                    "<comment> comment = '/*' [^*]* '*/';";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b/*comment*/ ";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(17, lastIndex);
        }

        [Test]
        public void TestEndOfInputParameterWithAmbiguousComment()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                    "<token> operand = [\\l_] [\\l\\d_]*;\n" +
                    "<comment> comment = '/*' [^*]* '*/';\n" +
                    "<token> strange = '/*' [\\l]+;\n";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);
            Definition strangeDef = grammar.FindDefinitionByName("strange");
            Tokenizer tokenizer = new Tokenizer(grammar);
            string input = "a + b/*comment*/";
            bool endOfInput;
            int lastIndex;


            var tokens = tokenizer.GetTokensAtLocation(input, 5, errors,
                                                       out endOfInput,
                                                       out lastIndex);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(1, tokens.Length);
            Assert.AreSame(strangeDef, tokens[0].Definition);
            Assert.AreEqual(5, tokens[0].StartIndex);
            Assert.AreEqual(9, tokens[0].Length);
            Assert.AreEqual(16, lastIndex);
        }
    }
}

