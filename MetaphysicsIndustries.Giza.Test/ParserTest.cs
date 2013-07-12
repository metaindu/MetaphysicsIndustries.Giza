using System;
using NUnit.Framework;

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

            string error;
            DefinitionInfo[] dis = (new SupergrammarSpanner()).GetExpressions(grammarText, out error);
            if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException();

            Grammar grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            Definition exprDef = grammar.FindDefinitionByName("expr");
            Definition operandDef = grammar.FindDefinitionByName("operand");
            Definition operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            Parser parser = new Parser(exprDef);



            Span[] s = parser.Parse("a + b");



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
    }
}

