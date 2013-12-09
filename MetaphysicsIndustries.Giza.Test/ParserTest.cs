using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
            var dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            var grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            var exprDef = grammar.FindDefinitionByName("expr");
            var operandDef = grammar.FindDefinitionByName("operand");
            var operatorDef = grammar.FindDefinitionByName("$implicit literal +");

            var parser = new Parser(exprDef);
            errors = new List<Error>();



            Span[] s = parser.Parse("a + b".ToCharacterSource(), errors);



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
            var dis = (new SupergrammarSpanner()).GetExpressions(grammarText, errors);
            Assert.IsEmpty(errors);

            var grammar = (new TokenizedGrammarBuilder()).BuildTokenizedGrammar(dis);

            var exprDef = grammar.FindDefinitionByName("expr");
            var subexprDef = grammar.FindDefinitionByName("subexpr");
            var operandDef = grammar.FindDefinitionByName("operand");
            var plusDef = grammar.FindDefinitionByName("$implicit literal +");
            var plusPlusDef = grammar.FindDefinitionByName("$implicit literal ++");

            var parser = new Parser(exprDef);
            errors = new List<Error>();



            Span[] s = parser.Parse("a+++b".ToCharacterSource(), errors);



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

        [Test]
        public void TestTokenIgnoreCase1()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token, ignore case> item = 'item'; \r\n";
            string testInputText = "item ITEM iTeM";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestTokenIgnoreCase2()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token, ignore case> item = [\\l]+; \r\n";
            string testInputText = "item ITEM iTeM";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenIgnoreCase1()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<token> item = 'abc' middle 'xyz'; \r\n" +
                "<subtoken, ignore case> middle = 'qwer'; \r\n";
            string testInputText = "abcqwerxyz abcQWERxyz abcQwErxyz";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abcqwerxyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("abcQWERxyz", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("abcQwErxyz", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenIgnoreCase2()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token> item = 'abc' middle 'xyz'; \r\n" +
                    "<subtoken, ignore case> middle = [\\l]+; \r\n";
            string testInputText = "abcqwerxyz abcQWERxyz abcQwErxyz";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abcqwerxyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("abcQWERxyz", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("abcQwErxyz", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestSubtokenAtomic()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token> item = 'abc-' sub+ '-xyz'; \r\n" +
                    "<subtoken, atomic> sub = [\\l]+; \r\n";
            string testInputText = "abc-qwer-xyz";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abc-qwer-xyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestSubtokenNonAtomic()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token> item = 'abc-' sub+ '-xyz'; \r\n" +
                    "<subtoken> sub = [\\l]+; \r\n";
            string testInputText = "abc-qw-xyz";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);



            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans.Count);
            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("abc-qw-xyz", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestCommentIgnoreCase1()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<token> item = 'item'; \r\n" +
                "<comment, ignore case> middle = '[comment]'; \r\n";
            string testInputText = "item [comment] item [COMMENT] item [CoMmEnT]";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestCommentIgnoreCase2()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "<token> item = 'item'; \r\n" +
                    "<comment, ignore case> middle = '[' [\\l]+ ']'; \r\n";
            string testInputText = "item [comment] item [COMMENT] item [CoMmEnT]";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[1].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[2].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans.Count);
        }

        [Test]
        public void TestCaseInsensitiveImplicitTokens1()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<ignore case> item = 'item'; \r\n";
            string testInputText = "item ITEM iTeM";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var implicitDef = testGrammar.FindDefinitionByName("$implicit ignore case literal item");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            Assert.IsNotNull(implicitDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("item", spans[0].Subspans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[1].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[1].Subspans[0].DefRef);
            Assert.AreEqual("ITEM", spans[0].Subspans[1].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[2].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("iTeM", spans[0].Subspans[2].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans[0].Subspans.Count);
        }

        [Test]
        public void TestCaseInsensitiveImplicitTokens2()
        {
            // setup
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = item+; \r\n" +
                "<ignore case, mind whitespace> item = [\\dabcdef]; \r\n";
            string testInputText = "0 a A";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var implicitDef = testGrammar.FindDefinitionByName("$implicit ignore case char class \\dabcdef");
            Assert.IsNotNull(itemDef);
            Assert.IsNotNull(sequenceDef);
            Assert.IsNotNull(implicitDef);
            var parser = new Parser(sequenceDef);


            // action
            var spans = parser.Parse(testInputText.ToCharacterSource(), errors);


            // assertions
            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[0].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[0].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[0].Subspans[0].DefRef);
            Assert.AreEqual("0", spans[0].Subspans[0].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[0].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[1].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[1].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[1].Subspans[0].DefRef);
            Assert.AreEqual("a", spans[0].Subspans[1].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[1].Subspans[0].Subspans.Count);

            Assert.AreSame(itemDef, spans[0].Subspans[2].DefRef);
            Assert.AreEqual(1, spans[0].Subspans[2].Subspans.Count);
            Assert.AreSame(implicitDef, spans[0].Subspans[2].Subspans[0].DefRef);
            Assert.AreEqual("A", spans[0].Subspans[2].Subspans[0].Value);
            Assert.AreEqual(0, spans[0].Subspans[2].Subspans[0].Subspans.Count);
        }

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

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

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

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var itemDef = testGrammar.FindDefinitionByName("item");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

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

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            Grammar testGrammar = tgb.BuildTokenizedGrammar(dis);
            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(13, err.Column);
            Assert.IsInstanceOf<CharNode>(err.LastValidMatchingNode);
            Assert.AreEqual("m", (err.LastValidMatchingNode as CharNode).CharClass.ToUndelimitedString());
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
//            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
//            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidToken1()
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

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.InvalidToken, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
            Assert.AreEqual(8, err.OffendingToken.StartPosition.Index);
            Assert.AreEqual(")", err.OffendingToken.Value);
            Assert.AreSame(cparenDef, err.OffendingToken.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidToken2()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren | one-two ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "one-two = 'one' 'two'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( two ";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var twoDef = testGrammar.FindDefinitionByName("$implicit literal two");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.InvalidToken, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
            Assert.AreEqual(8, err.OffendingToken.StartPosition.Index);
            Assert.AreEqual("two", err.OffendingToken.Value);
            Assert.AreSame(twoDef, err.OffendingToken.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNotNull(err.ExpectedNodes);
            Assert.AreEqual(1, err.ExpectedNodes.Count());
            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestInvalidCharacter()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                    "sequence = item+; \r\n" +
                    "item = ( id-item1 | id-item2 | paren ); \r\n" +
                    "<token> id-item1 = 'item1'; \r\n" +
                    "<token> id-item2 = 'item2'; \r\n" +
                    "paren = '(' sequence ')'; \r\n";

            string testInput = "item1 ( $";

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var oparenDef = testGrammar.FindDefinitionByName("$implicit literal (");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(testGrammar.FindDefinitionByName("sequence"));

            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var err = ((SpannerError)errors[0]);
            Assert.AreEqual(SpannerError.InvalidCharacter, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(9, err.Column);
            Assert.AreEqual(8, err.Index);
//            Assert.AreEqual(8, err.OffendingToken.StartPosition.Index);
            Assert.AreEqual('$', err.OffendingCharacter);
//            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
//            Assert.AreSame(oparenDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
//            Assert.IsNotNull(err.ExpectedNodes);
//            Assert.AreEqual(1, err.ExpectedNodes.Count());
//            Assert.IsInstanceOf<DefRefNode>(err.ExpectedNodes.First());
//            Assert.AreSame(sequenceDef, (err.ExpectedNodes.First() as DefRefNode).DefRef);
        }

        [Test]
        public void TestExcessRemainingInput()
        {
            string testGrammarText =
                " // test grammar \r\n" +
                "sequence = 'one' 'two' 'three'; \r\n" +
                "number = ( 'one' | 'two' | 'three' | 'four' ); \r\n";

            string testInput = "one two three four";
                              //123456789012345678
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var threeDef = testGrammar.FindDefinitionByName("$implicit literal three");
            var fourDef = testGrammar.FindDefinitionByName("$implicit literal four");
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(sequenceDef);


            Span[] spans = parser.Parse(testInput.ToCharacterSource(), errors);


            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ParserError<Token>>(errors[0]);
            var err = ((ParserError<Token>)errors[0]);
            Assert.AreEqual(ParserError.ExcessRemainingInput, err.ErrorType);
            Assert.AreEqual(1, err.Line);
            Assert.AreEqual(15, err.Column);
            Assert.AreEqual(14, err.Index);
            Assert.AreEqual(14, err.OffendingToken.StartPosition.Index);
            Assert.AreEqual("four", err.OffendingToken.Value);
            Assert.AreSame(fourDef, err.OffendingToken.Definition);
            Assert.IsInstanceOf<DefRefNode>(err.LastValidMatchingNode);
            Assert.AreSame(threeDef, (err.LastValidMatchingNode as DefRefNode).DefRef);
            Assert.IsNull(err.ExpectedNodes);
        }

        [Test]
        [Explicit("A manual test for debugging the internal order of source node matches in the parser.")]
        public void TestNodeMatchTokenOrder()
        {
            var testGrammarText =
                "sequence = ( a abc c | aa bb+ cc );\n" +
                "<token> a = 'a';\n" +
                "<token> abc = 'abbbbc';\n" +
                "<token> aa = 'aa';\n" +
                "<token> bb = 'bb';\n" +
                "<token> cc = 'cc';\n" +
                "<token> c = 'c';";
            var testInput = "aabbbbcc".ToCharacterSource();
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();
            var testGrammar = tgb.BuildTokenizedGrammar(dis);
            var sequenceDef = testGrammar.FindDefinitionByName("sequence");
            var parser = new Parser(sequenceDef);


            var spans = parser.Parse(testInput, errors);

            Assert.IsNotNull(spans);
            Assert.AreEqual(2, spans.Length);

        }

        public class MockTokenSource : IInputSource<Token>
        {
            public readonly Dictionary<int, InputElementSet<Token>> InputElementSetsByIndex = new Dictionary<int, InputElementSet<Token>>();

            public InputElementSet<Token> GetInputAtLocation(int index)
            {
                return InputElementSetsByIndex[index];
            }

            public InputPosition CurrentPosition
            {
                get { throw new NotImplementedException(); }
            }

            public InputPosition GetPosition(int index)
            {
                throw new NotImplementedException();
            }

            public void SetCurrentIndex(int index)
            {
                throw new NotImplementedException();
            }

            public InputChar Peek()
            {
                throw new NotImplementedException();
            }

            public InputChar GetNextValue()
            {
                throw new NotImplementedException();
            }

            public int Length
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsAtEnd
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class MockError : Error
        {
            public static readonly ErrorType MockErrorType = new ErrorType(name: "MockErrorType");
            public static readonly ErrorType MockErrorType2 = new ErrorType(name: "MockErrorType2");
        }

        [Test]
        public void TestErrorsFromTokenSource()
        {
            // setup
            var tokenSource = new MockTokenSource();
            var def = new Definition("A");
            def.Directives.Add(DefinitionDirective.Token);
            CharNode anode = new CharNode('a');
            def.StartNodes.Add(anode);
            def.EndNodes.Add(anode);
            def.Nodes.Add(anode);
            tokenSource.InputElementSetsByIndex[0] = new InputElementSet<Token>() {
                InputElements = new Token[] {
                    new Token(def, new InputPosition(0), "a", 1)
                },
            };
            tokenSource.InputElementSetsByIndex[1] = new InputElementSet<Token>() {
                Errors = new List<Error> {
                    new MockError() {
                        ErrorType = MockError.MockErrorType
                    },
                    new MockError() {
                        ErrorType = MockError.MockErrorType2
                    },
                }
            };

            var def2 = new Definition("sequence");
            var node = new DefRefNode(def);
            node.NextNodes.Add(node);
            def2.StartNodes.Add(node);
            def2.EndNodes.Add(node);
            def2.Nodes.Add(node);

            var parser = new Parser(def2);
            List<Error> errors = new List<Error>();

            // action
            var spans = parser.Parse(tokenSource, errors);

            // assertions
            Assert.IsNotNull(spans);
            Assert.AreEqual(0, spans.Length);
            Assert.AreEqual(2, errors.Count);
            Assert.IsInstanceOf<MockError>(errors[0]);
            Assert.AreEqual(MockError.MockErrorType, errors[0].ErrorType);
            Assert.IsInstanceOf<MockError>(errors[1]);
            Assert.AreEqual(MockError.MockErrorType2, errors[1].ErrorType);
        }
    }
}

