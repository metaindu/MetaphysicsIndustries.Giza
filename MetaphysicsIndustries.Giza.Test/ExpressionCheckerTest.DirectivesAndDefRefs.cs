
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(0, errors.Count);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(0, errors.Count);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(0, errors.Count);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(0, errors.Count);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NonTokenReferencesSubtoken, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NonTokenReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Expr.Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
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
            Assert.AreEqual(0, errors.Count);
        }
    }
}

