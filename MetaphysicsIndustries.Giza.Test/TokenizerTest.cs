using System;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class TokenizerTest
    {
//        [Test()]
        public void TestNormal()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 ); \r\n" +
                "<mind whitespace, atomic, token> id-item1 = 'item1'; \r\n" +
                "<mind whitespace, atomic, token> id-item2 = 'item2'; \r\n";

            string error;
            Grammar testGrammar = (new SupergrammarSpanner()).GetGrammar(testGrammarText, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException(error);
            }
            Definition item1Def = testGrammar.FindDefinitionByName("id-item1");
            Definition item2Def = testGrammar.FindDefinitionByName("id-item2");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;
            tokens = t.GetTokensAtLocation("item1 item2", 0, out error);
            Assert.IsNull(error);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(item1Def, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(5, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation("item1 item2", 5, out error);
            Assert.IsNull(error);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(item2Def, tokens[0].Definition);
                Assert.AreEqual(6, tokens[0].StartIndex);
                Assert.AreEqual(5, tokens[0].Length);
            }
        }

//        [Test()]
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

            string error;
            Grammar testGrammar = (new SupergrammarSpanner()).GetGrammar(testGrammarText, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException(error);
            }
            Definition varrefDef = testGrammar.FindDefinitionByName("varref");
            Definition plusplusDef = testGrammar.FindDefinitionByName("plusplus");
            Definition operDef = testGrammar.FindDefinitionByName("oper");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;

            tokens = t.GetTokensAtLocation(testInput, 0, out error);
            Assert.IsNull(error);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(varrefDef, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(1, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation(testInput, 1, out error);
            Assert.IsNull(error);
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

//        [Test()]
        public void TestAmbiguousCombinedToken()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "expr = item ( oper item )+; \r\n" +
                "<mind whitespace, atomic, token> item = [\\l]+ ; \r\n" +
                "<mind whitespace, token> oper = ( '<' | '<<' ); \r\n";

            string testInput = "a << b";

            string error;
            DefinitionInfo[] dis = (new SupergrammarSpanner()).GetExpressions(testGrammarText, out error); 
            TokenizedGrammarBuilder tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);


            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException(error);
            }
            Definition itemDef = testGrammar.FindDefinitionByName("item");
            Definition operDef = testGrammar.FindDefinitionByName("oper");

            Tokenizer t = new Tokenizer(testGrammar);
            Token[] tokens;

            tokens = t.GetTokensAtLocation(testInput, 0, out error);
            Assert.IsNull(error);
            Assert.AreEqual(1, tokens.Length);
            if (tokens.Length > 0)
            {
                Assert.AreEqual(itemDef, tokens[0].Definition);
                Assert.AreEqual(0, tokens[0].StartIndex);
                Assert.AreEqual(1, tokens[0].Length);
            }

            tokens = t.GetTokensAtLocation(testInput, 1, out error);
            Assert.IsNull(error);
            Assert.AreEqual(2, tokens.Length);
            Assert.AreEqual(2, tokens[0].StartIndex);
            Assert.AreEqual(2, tokens[1].StartIndex);
            Assert.AreEqual(operDef, tokens[0].Definition);
            Assert.AreEqual(operDef, tokens[1].Definition);
            Assert.IsTrue(tokens[0].Length == 1 || tokens[0].Length == 2);
            Assert.IsTrue(tokens[1].Length == 1 || tokens[1].Length == 2);
            Assert.IsTrue(tokens[0].Length != tokens[1].Length);
        }
    }
}

