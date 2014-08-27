using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class DefinitionRendererTest
    {
        [Test]
        public void TestRenderDefinitionExprsAsGrammarText()
        {
            SupergrammarSpanner ss = new SupergrammarSpanner();
            string input =
                "expr = ( binop | subexpr );\n\n" +
                "binop = subexpr ( [%*+-/] subexpr )+;\n\n" +
                "subexpr = ( number | var | unop | paren );\n\n" +
                "number = [\\d]+;\n\n" +
                "var = [\\l]+;\n\n" +
                "unop = [+-] subexpr;\n\n" +
                "paren = '(' expr ')';\n\n";
            var errors = new List<Error>();
            var defs = ss.GetExpressions(input, errors);
            Assert.IsEmpty(errors);

            var dr = new DefinitionRenderer();
            var result = dr.RenderDefinitionExprsAsGrammarText(defs);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void TestRenderDefinitionExprsAsGrammarTextModifiers()
        {
            SupergrammarSpanner ss = new SupergrammarSpanner();
            string input =
                "question = 'item'?;\n\n" +
                "plus = 'item'+;\n\n" +
                "star = 'item'*;\n\n";
            var errors = new List<Error>();
            var defs = ss.GetExpressions(input, errors);
            Assert.IsEmpty(errors);

            var dr = new DefinitionRenderer();
            var result = dr.RenderDefinitionExprsAsGrammarText(defs);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void TestRenderDefinitionExprsAsGrammarTextTags()
        {
            SupergrammarSpanner ss = new SupergrammarSpanner();
            string input =
                "literal = 'item':something;\n\n" +
                "charclass = [abc]:another;\n\n" +
                "defref = literal:stillmore;\n\n" +
                "tag-and-modifiers = 'item'+:thetag;\n\n";
            var errors = new List<Error>();
            var defs = ss.GetExpressions(input, errors);
            Assert.IsEmpty(errors);

            var dr = new DefinitionRenderer();
            var result = dr.RenderDefinitionExprsAsGrammarText(defs);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void TestRenderDefinitionExprsAsGrammarTextDirectives()
        {
            SupergrammarSpanner ss = new SupergrammarSpanner();
            string input =
                "none = 'item';\n\n" +
                "<atomic> atomic = 'item';\n\n" +
                "<ignore case> ignore-case = 'item';\n\n" +
                "<mind whitespace> mind-whitespace = 'item';\n\n" +
                "<token> token = 'item';\n\n" +
                "<subtoken> subtoken = 'item';\n\n" +
                "<comment> comment = 'item';\n\n" +
                "<atomic, ignore case> multiple = 'item';\n\n";
            var errors = new List<Error>();
            var defs = ss.GetExpressions(input, errors);
            Assert.IsEmpty(errors);

            var dr = new DefinitionRenderer();
            var result = dr.RenderDefinitionExprsAsGrammarText(defs);

            Assert.AreEqual(input, result);
        }
    }
}

