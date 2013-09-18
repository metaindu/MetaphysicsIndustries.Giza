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

            Span[] spans = s.Process("qwer", errors);
            Assert.AreEqual(1, spans.Length);
            Assert.IsEmpty(errors);

            spans = s.Process("asdf", errors);
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
            Span[] spans = s.Process(testGrammarText, errors);

            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('w', err.OffendingCharacter);
            Assert.AreEqual(4, err.Line);
            Assert.AreEqual(2, err.Column);
            Assert.AreEqual(74, err.Index);
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
            Span[] spans = s.Process(testInput, errors);

            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual('$', err.OffendingCharacter);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(1, err.Column);
            Assert.AreEqual(0, err.Index);
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
            Span[] spans = s.Process(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.AreEqual(14, err.Index);
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
            Span[] spans = s.Process(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(14, err.Column);
            Assert.AreEqual(13, err.Index);
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
            Span[] spans = s.Process(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(13, err.Column);
            Assert.AreEqual(12, err.Index);
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
            Span[] spans = s.Process(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<Spanner.SpannerError>(errors[0]);
            var err = ((Spanner.SpannerError)errors[0]);
            Assert.AreEqual(Spanner.SpannerError.ExcessRemainingInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.AreEqual(14, err.Index);
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
            int lastIndex;


            var tokens = spanner.Match(input, errors,
                                       out endOfInput, out lastIndex,
                                       mustUseAllInput: false, startIndex: 5);


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
                "<atomic> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            Grammar grammar = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;
            int lastIndex;


            var tokens = spanner.Match(input, errors,
                                       out endOfInput, out lastIndex,
                                       mustUseAllInput: false, startIndex: 5);


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
                "<atomic> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            Grammar grammar = (new SupergrammarSpanner()).GetGrammar(grammarText, errors);
            Assert.IsEmpty(errors);

            var exprDef = grammar.FindDefinitionByName("expr");
            Spanner spanner = new Spanner(exprDef);
            string input = "a + b ";
            bool endOfInput;
            int lastIndex;


            var tokens = spanner.Match(input, errors,
                                       out endOfInput, out lastIndex,
                                       mustUseAllInput: false, startIndex: 5);


            Assert.IsTrue(endOfInput);
            Assert.IsNotNull(tokens);
            Assert.IsEmpty(tokens);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            Assert.AreEqual(6, lastIndex);
        }

        [Test]
        public void TestEndDefAtLastCharacter()
        {
            // setup
            string testGrammarText =
                "<mind whitespace> format = ( text | param )+; \r\n" +
                "<atomic, mind whitespace> text = [^{}]+ ; \r\n" +
                "<mind whitespace> param = '{' [\\s]* name [\\s]* '}' ; \r\n" +
                "<mind whitespace> name = [\\l_] [\\l\\d]* ; \r\n";
            string testInput = "leading {delimited}x";
            var errors = new List<Error>();
            var grammar = (new SupergrammarSpanner()).GetGrammar(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var formatDef = grammar.FindDefinitionByName("format");
            var textDef = grammar.FindDefinitionByName("text");
            var paramDef = grammar.FindDefinitionByName("param");
            var nameDef = grammar.FindDefinitionByName("name");
            var spanner = new Spanner(formatDef);

            var spans = spanner.Process(testInput, errors);

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
    }
}

