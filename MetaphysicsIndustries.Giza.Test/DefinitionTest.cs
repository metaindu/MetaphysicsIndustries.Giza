
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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

using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class DefinitionTest
    {
        [Test]
        public void CreateYieldsNewObjectWithDefaultValues()
        {
            // when
            var result = new Definition();
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.Name);
            Assert.IsEmpty(result.Directives);
            Assert.IsNotNull(result.Expr);
            Assert.IsEmpty(result.Expr.Items);
            Assert.IsFalse(result.IsImported);
        }

        [Test]
        public void CreateWithParametersYieldsThoseValues()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            // when
            var result = new Definition("def1", directives, expr, true,
                "source1");
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsTrue(result.IsImported);
            Assert.AreEqual("source1", result.Source);
        }

        [Test]
        public void CloneWithNoArgsYieldsCopy()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            var def = new Definition("def1", directives, expr, true);
            // when
            var result = def.Clone();
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsTrue(result.IsImported);
        }

        [Test]
        public void CloneWithNewNameYieldsCopyWithThatValue()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            var def = new Definition("def1", directives, expr, true);
            // when
            var result = def.Clone(newName: "def2");
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def2", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsTrue(result.IsImported);
        }

        [Test]
        public void CloneWithNewDirectiveYieldsCopyWithThatValue()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            var def = new Definition("def1", directives, expr, true);
            // when
            var result = def.Clone(
                newDirectives: new []{DefinitionDirective.MindWhitespace});
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsFalse(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsFalse(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsTrue(
                result.Directives.Contains(
                    DefinitionDirective.MindWhitespace));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsTrue(result.IsImported);
        }

        [Test]
        public void CloneWithNewExprYieldsCopyWithThatValue()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr1 = new Expression(new LiteralSubExpression("value"));
            var expr2 = new Expression(new CharClassSubExpression(
                CharClass.FromUndelimitedCharClassText("\\l")));
            var def = new Definition("def1", directives, expr1, true);
            // when
            var result = def.Clone(newExpr: expr2);
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreNotSame(expr1, result.Expr);
            Assert.AreSame(expr2, result.Expr);
            Assert.IsTrue(result.IsImported);
        }

        [Test]
        public void CloneWithNewIsImportedYieldsCopyWithThatValue()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            var def = new Definition("def1", directives, expr, true);
            // when
            var result = def.Clone(newIsImported: false);
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsFalse(result.IsImported);
        }

        [Test]
        public void CloneWithNewIsImportedYieldsCopyWithThatValue2()
        {
            // given
            var directives = new[]
            {
                DefinitionDirective.Atomic,
                DefinitionDirective.IgnoreCase
            };
            var expr = new Expression(new LiteralSubExpression("value"));
            var def = new Definition("def1", directives, expr, false);
            // when
            var result = def.Clone(newIsImported: true);
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual("def1", result.Name);
            Assert.IsNotEmpty(result.Directives);
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.Atomic));
            Assert.IsTrue(
                result.Directives.Contains(DefinitionDirective.IgnoreCase));
            Assert.IsNotNull(result.Expr);
            Assert.AreSame(expr, result.Expr);
            Assert.IsTrue(result.IsImported);
        }

        [Test]
        public void ToStringIncludesName()
        {
            // given
            var def = new Definition("def1");
            // when
            var result = def.ToString();
            // then
            Assert.AreEqual("Definition def1", result);
        }
    }
}
