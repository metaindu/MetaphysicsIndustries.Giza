
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
    public partial class ExpressionCheckerTest
    {
        [Test()]
        public void TestTokenInNonTokenized1()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionsForSpanning(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.
                    TokenizedDirectiveInNonTokenizedGrammar));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.That((errors[0] as ExpressionError).Definition,
                Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestTokenInNonTokenized2()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionsForSpanning(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.TokenizedDirectiveInNonTokenizedGrammar));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.That((errors[0] as ExpressionError).Definition,
                Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestTokenInNonTokenized3()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionsForSpanning(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.TokenizedDirectiveInNonTokenizedGrammar));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.That((errors[0] as ExpressionError).Definition,
                Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestMixedTokenizedDirectives1()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.MixedTokenizedDirectives));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestMixedTokenizedDirectives2()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.MixedTokenizedDirectives));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestMixedTokenizedDirectives3()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.MixedTokenizedDirectives));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestAtomicInNonTokenDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.AtomicInNonTokenDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestMindWhitespaceInNonTokenDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.MindWhitespaceInNonTokenDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestAtomicInTokenDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.AtomicInTokenDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestAtomicInSubtokenDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(
                new LiteralSubExpression {Value = "literal"});

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestAtomicInCommentDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.AtomicInCommentDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition1()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.MindWhitespaceInTokenizedDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition2()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.MindWhitespaceInTokenizedDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition3()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(
                    ExpressionError.MindWhitespaceInTokenizedDefinition));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }
    }
}

