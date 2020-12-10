
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

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

