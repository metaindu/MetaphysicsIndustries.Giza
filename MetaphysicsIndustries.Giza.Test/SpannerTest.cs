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
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('w', err.OffendingCharacter);
            Assert.AreEqual(4, err.Line);
            Assert.AreEqual(2, err.Column);
            Assert.IsInstanceOf<CharNode>(err.PreviousNode);
            var charnode = (err.PreviousNode as CharNode);
            Assert.AreEqual("<", charnode.CharClass.ToUndelimitedString());
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.AreEqual("directive-item", (err.ExpectedNodes.First() as DefRefNode).DefRef.Name);
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
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('$', err.OffendingCharacter);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(1, err.Column);
            Assert.IsNull(err.PreviousNode);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.AreEqual("item", (err.ExpectedNodes.First() as DefRefNode).DefRef.Name);
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

