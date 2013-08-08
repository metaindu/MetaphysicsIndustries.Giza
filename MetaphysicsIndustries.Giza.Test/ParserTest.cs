using System;
using NUnit.Framework;
using System.Collections.Generic;

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
    }
}

