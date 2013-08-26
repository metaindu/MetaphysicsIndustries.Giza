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

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("def1"));

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

            Supergrammar sg = new Supergrammar();
            Spanner s = new Spanner(sg.def_0_grammar);
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

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
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
        public void TestUnexpectedEndOfInput1()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "item = ( id-item1 | id-item2 | paren ); \r\n" +
                "<mind whitespace, atomic> id-item1 = 'item1'; \r\n" +
                "<mind whitespace, atomic> id-item2 = 'item2'; \r\n" +
                "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item2 ";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            Grammar testGrammar = sgs.GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            Span[] spans = s.Process(testGrammar, "sequence", testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.PreviousNode);
            Assert.AreEqual("sequence", (err.PreviousNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.AreEqual(")", (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString());
        }

        [Test]
        public void TestUnexpectedEndOfInput2()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<mind whitespace, atomic> id-item1 = 'item1'; \r\n" +
                    "<mind whitespace, atomic> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item2";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            Grammar testGrammar = sgs.GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            Span[] spans = s.Process(testGrammar, "sequence", testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(14, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.PreviousNode);
            Assert.AreEqual("sequence", (err.PreviousNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            Assert.AreEqual(")", (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString());
        }

        [Test]
        public void TestUnexpectedEndOfInput3()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<mind whitespace, atomic> id-item1 = 'item1'; \r\n" +
                    "<mind whitespace, atomic> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            Grammar testGrammar = sgs.GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            Span[] spans = s.Process(testGrammar, "sequence", testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(13, err.Column);
            Assert.IsInstanceOf<CharNode>(err.PreviousNode);
            Assert.AreEqual("m", (err.PreviousNode as CharNode).CharClass.ToUndelimitedString());
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<CharNode>(err.ExpectedNodes.First());
            var expectedChar = (err.ExpectedNodes.First() as CharNode).CharClass.ToUndelimitedString();
            Assert.True(expectedChar == "1" || expectedChar == "2");
        }

        [Test]
        public void TestExcessRemainingInput()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = id-one id-two id-three; \r\n" +
                "<mind whitespace, atomic> id-one = 'one'; \r\n" +
                "<mind whitespace, atomic> id-two = 'two'; \r\n" +
                "<mind whitespace, atomic> id-three = 'three'; \r\n";

            string testInput = "one two three four";
                              //123456789012345678
            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            Grammar testGrammar = sgs.GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);

            Spanner s = new Spanner(testGrammar.FindDefinitionByName("sequence"));
            Span[] spans = s.Process(testGrammar, "sequence", testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.ExcessRemainingInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.PreviousNode);
            Assert.AreEqual("sequence", (err.PreviousNode as DefRefNode).DefRef.Name);
            Assert.IsNull(err.ExpectedNodes);
        }

        [Test]
        public void TestEndOfInputParameter1()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<atomic> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            Grammar grammar = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b";
            bool endOfInput;


            var tokens = spanner.Match(exprDef, input, errors, out endOfInput,
                                       mustUseAllInput: false, startIndex: 5);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void TestEndOfInputParameter2()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<atomic> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            Grammar grammar = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;


            var tokens = spanner.Match(exprDef, input, errors, out endOfInput,
                                       mustUseAllInput: false, startIndex: 5);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void TestEndOfInputParameter3()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<atomic> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            Grammar grammar = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;


            var tokens = spanner.Match(exprDef, input, errors, out endOfInput,
                                       mustUseAllInput: false, startIndex: 5);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
        }
    }
}

