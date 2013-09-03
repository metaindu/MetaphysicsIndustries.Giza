using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ParserError = MetaphysicsIndustries.Giza.Parser.ParserError;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void TestSimple()
        {
            string grammarText =
                "expr = operand '+' operand;\n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition exprDef = grammar.FindDefinitionByName("expr");
            Definition operandDef = grammar.FindDefinitionByName("operand");
            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            Parser parser = new Parser(exprDef);
            errors = new List<Error>();



            Span[] s = parser.Parse("a + b", errors);



            Assert.IsNotNull(s);
            Assert.AreEqual(1, s.Length);
            Assert.AreEqual(3, s[0].Subspans.Count);

            Assert.AreSame(exprDef.Nodes[0], s[0].Subspans[0].Node);
            Assert.AreSame(operandDef, s[0].Subspans[0].DefRef);
            Assert.AreEqual("a", s[0].Subspans[0].Value);

            Assert.AreSame(exprDef.Nodes[1], s[0].Subspans[1].Node);
            Assert.AreSame(operatorDef, s[0].Subspans[1].DefRef);
            Assert.AreEqual("+", s[0].Subspans[1].Value);

            Assert.AreSame(exprDef.Nodes[2], s[0].Subspans[2].Node);
            Assert.AreSame(operandDef, s[0].Subspans[2].DefRef);
            Assert.AreEqual("b", s[0].Subspans[2].Value);
        }

        [Test]
        public void TestAmbiguousImplicitTokens()
        {
            string grammarText =
                "expr = subexpr '+' subexpr;\n" +
                "subexpr = ( operand | operand '++' | '++' operand ); \n" +
                "<token> operand = [\\l_] [\\l\\d_]*;";

            var errors = new List<Error>();
            DefinitionExpression[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition exprDef = grammar.FindDefinitionByName("expr");
            Definition subexprDef = grammar.FindDefinitionByName("subexpr");
            Definition operandDef = grammar.FindDefinitionByName("operand");
            Definition plusDef = grammar.FindDefinitionByName("$implicit literal +");
            Definition plusPlusDef = grammar.FindDefinitionByName("$implicit literal ++");

            Parser parser = new Parser(exprDef);
            errors = new List<Error>();



            Span[] s = parser.Parse("a+++b", errors);



            Assert.IsNotNull(s);
            Assert.AreEqual(2, s.Length);

            Assert.AreEqual(3, s[0].Subspans.Count);
            Assert.AreSame(subexprDef, s[0].Subspans[0].DefRef);
            Assert.AreSame(plusDef, s[0].Subspans[1].DefRef);
            Assert.AreEqual(0, s[0].Subspans[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[0].Subspans[2].DefRef);

            Assert.AreEqual(3, s[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[1].Subspans[0].DefRef);
            Assert.AreSame(plusDef, s[1].Subspans[1].DefRef);
            Assert.AreEqual(0, s[1].Subspans[1].Subspans.Count);
            Assert.AreSame(subexprDef, s[1].Subspans[2].DefRef);

            Assert.True(s[0].Subspans[0].Subspans.Count == 1 ||
                        s[0].Subspans[0].Subspans.Count == 2);
            Assert.True(s[1].Subspans[0].Subspans.Count == 1 ||
                        s[1].Subspans[0].Subspans.Count == 2);
            Assert.True(s[0].Subspans[0].Subspans.Count != s[1].Subspans[0].Subspans.Count);

            Span oneTwo;
            Span twoOne;
            if (s[0].Subspans[0].Subspans.Count == 1)
            {
                oneTwo = s[0];
                twoOne = s[1];
            }
            else
            {
                oneTwo = s[1];
                twoOne = s[0];
            }

            Assert.AreEqual(1, oneTwo.Subspans[0].Subspans.Count);
            Assert.AreEqual(0, oneTwo.Subspans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, oneTwo.Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("a", oneTwo.Subspans[0].Subspans[0].Value);
            Assert.AreEqual(2, oneTwo.Subspans[2].Subspans.Count);
            Assert.AreEqual(0, oneTwo.Subspans[2].Subspans[0].Subspans.Count);
            Assert.AreSame(plusPlusDef, oneTwo.Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual(0, oneTwo.Subspans[2].Subspans[1].Subspans.Count);
            Assert.AreSame(operandDef, oneTwo.Subspans[2].Subspans[1].DefRef);
            Assert.AreEqual("b", oneTwo.Subspans[2].Subspans[1].Value);

            Assert.AreEqual(2, twoOne.Subspans[0].Subspans.Count);
            Assert.AreEqual(0, twoOne.Subspans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, twoOne.Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("a", twoOne.Subspans[0].Subspans[0].Value);
            Assert.AreSame(plusPlusDef, twoOne.Subspans[0].Subspans[1].DefRef);
            Assert.AreEqual(0, twoOne.Subspans[0].Subspans[1].Subspans.Count);
            Assert.AreEqual(1, twoOne.Subspans[2].Subspans.Count);
            Assert.AreEqual(0, twoOne.Subspans[2].Subspans[0].Subspans.Count);
            Assert.AreSame(operandDef, twoOne.Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("b", twoOne.Subspans[2].Subspans[0].Value);
        }

        //[Test]
        //public void TestCaseInsensitiveImplicitTokens()
        //{
        //}

        //[Test]
        //public void TestMindWhitespaceTokens()
        //{
        //}

        [Test]
        public void TestUnexpectedEndOfInput1()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item2 ";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            Parser parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("id-item2", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(itemDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestUnexpectedEndOfInput2()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item2";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            Parser parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(14, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreEqual("id-item2", (err.LastValidMatchingNode as DefRefNode).DefRef.Name);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(itemDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestUnexpectedEndOfInputInToken()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( item";

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(13, err.Column);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidToken()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( )";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var cparenDef = testGrammar.FindDefinitionByName("$implicit literal )");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError>(errors[0]);
            var err = ((ParserError)errors[0]);
            Assert.AreEqual(ParserError.InvalidToken, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.OffendingToken.StartIndex);
            Assert.AreEqual(1, err.OffendingToken.Length);
            Assert.AreSame(cparenDef, err.OffendingToken.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }
    }
}

