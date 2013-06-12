using System;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class TokenizerTest
    {
        [Test()]
        public void TestErrorInvalidCharacterAfterDefRef()
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
            Definition sequenceDef = testGrammar.FindDefinitionByName("sequence");
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
    }
}

