
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
using System.Collections.Generic;
using NUnit.Framework;

// token/token         n
// token/subtoken      y
// token/comment       n
// token/nontoken      n
// subtoken/token      n
// subtoken/subtoken   y
// subtoken/comment    n
// subtoken/nontoken   n
// comment/token       n
// comment/subtoken    y
// comment/comment     n
// comment/nontoken    n
// nontoken/token      y
// nontoken/subtoken   n
// nontoken/comment    n
// nontoken/nontoken   y

namespace MetaphysicsIndustries.Giza.Test
{
    public partial class ExpressionCheckerTest
    {
        [Test]
        public void TestTokenReferencesToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestTokenReferencesSubtoken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestTokenReferencesComment()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesComment));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestTokenReferencesNonToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesNonToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestSubtokenReferencesToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestSubtokenReferencesSubtoken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestSubtokenReferencesComment()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesComment));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test()]
        public void TestSubtokenReferencesNonToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesNonToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestCommentReferencesToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestCommentReferencesSubtoken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestCommentReferencesComment()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesComment));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestCommentReferencesNonToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.TokenizedReferencesNonToken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestNonTokenReferencesToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestNonTokenReferencesSubtoken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NonTokenReferencesSubtoken));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test()]
        public void TestNonTokenReferencesComment()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NonTokenReferencesComment));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.EqualTo(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defs[1].Expr.Items[0]));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
        }

        [Test]
        public void TestNonTokenReferencesNonToken()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(
                new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(
                new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(0));
        }
    }
}

