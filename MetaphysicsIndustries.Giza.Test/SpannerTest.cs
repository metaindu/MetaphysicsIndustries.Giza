using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class SpannerTest
    {
        [Test]
        public void TestOrExpr()
        {
            string testGrammarText = 
                "def1 = ( 'qwer' | 'asdf' );";
            var errors = new List<Error>();
            Grammar testGrammar = (new SupergrammarSpanner()).GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner();

            Span[] spans = s.Process(testGrammar, "def1", "qwer", errors);
            Assert.AreEqual(1, spans.Length);
            Assert.IsEmpty(errors);

            spans = s.Process(testGrammar, "def1", "asdf", errors);
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

            Spanner s = new Spanner();
            Supergrammar sg = new Supergrammar();
            var errors = new List<Error>();
            Span[] spans = s.Process(sg.def_0_grammar, testGrammarText, errors);

            Assert.IsEmpty(spans);
            throw new NotImplementedException();
//            Assert.AreEqual("Invalid character 'w' at (4,2), after a '<': expected id-mind, id-ignore, id-atomic, id-token, or id-subtoken", error);
        }

        [Test()]
        public void TestErrorInvalidCharacterAtStart()
        {
            string testGrammarText = 
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 ); \r\n" +
                "<mind whitespace, atomic> id-item1 = 'item1'; \r\n" +
                "<mind whitespace, atomic> id-item2 = 'item2'; \r\n";

            string testInput = "$ item1 item2 ";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            Grammar testGrammar = sgs.GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner();
            Span[] spans = s.Process(testGrammar, "sequence", testInput, errors);

            Assert.IsEmpty(spans);
            throw new NotImplementedException();
//            Assert.AreEqual("Invalid character '$' at (1,1): a sequence must start with item", error);
        }

        [Test]
        public void TestNodeMatchClone()
        {
            Node node = new CharNode('c', "asdf");
            NodeMatch nm = new NodeMatch(node, NodeMatch.TransitionType.Follow, null);
            nm.Index = 123;
            nm.Token = new Token { StartIndex = 3, Length = 3 };

            NodeMatch clone = nm.CloneWithNewToken(new Token { StartIndex = 5, Length = 2 });

            Assert.AreSame(node, clone.Node);
            Assert.AreEqual(NodeMatch.TransitionType.Follow, clone.Transition);
            Assert.AreEqual(123, clone.Index);
            Assert.AreEqual(5, clone.Token.StartIndex);
            Assert.AreEqual(2, clone.Token.Length);
        }
    }
}

