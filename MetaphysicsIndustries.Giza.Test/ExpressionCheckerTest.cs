
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
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public partial class ExpressionCheckerTest
    {
        [Test()]
        public void TestNullDefinitionArray()
        {
            ExpressionChecker ec = new ExpressionChecker();

            Assert.Throws<ArgumentNullException>(() => ec.CheckDefinitions(null));
        }

        [Test()]
        public void TestExpressionReuse()
        {
            Expression expr = new Expression();
            expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            OrExpression orexprA = new OrExpression();
            orexprA.Expressions.Add(expr);
            OrExpression orexprB = new OrExpression();
            orexprB.Expressions.Add(expr);

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(orexprA);
            defs[1].Expr.Items.Add(orexprB);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedExpression));
            Assert.That(e.Expression, Is.SameAs(expr));
            Assert.That(e.Definition, Is.SameAs(defs[1]));
        }

        [Test()]
        public void TestExpressionItemReuse()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(literal);
            defs[1].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedExpressionItem));
            Assert.That(e.ExpressionItem, Is.SameAs(literal));
            Assert.That(e.Definition, Is.SameAs(defs[1]));
        }

        [Test()]
        public void TestExpressionItemReuseinSameDef()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedExpressionItem));
            Assert.That(e.ExpressionItem, Is.SameAs(literal));
            Assert.That(e.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestReferenceCycle()
        {
            OrExpression orexpr = new OrExpression();
            OrExpression orexpr2 = new OrExpression();
            Expression expr = new Expression();

            orexpr.Expressions.Add(expr);
            expr.Items.Add(orexpr);
            orexpr2.Expressions.Add(expr);

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr2);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedExpression));
            Assert.That(e.Expression, Is.SameAs(expr));
            Assert.That(e.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestReferenceCycleItem()
        {
            OrExpression orexpr = new OrExpression();
            Expression expr2 = new Expression();

            orexpr.Expressions.Add(expr2);
            expr2.Items.Add(orexpr);

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedExpressionItem));
            Assert.That(e.Expression, Is.SameAs(null));
            Assert.That(e.ExpressionItem, Is.SameAs(orexpr));
            Assert.That(e.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestNullDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            Definition[] defs = {
                new Definition {
                    Name = null,
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
             Is.EqualTo(ExpressionError.NullOrEmptyDefinitionName));
            Assert.That(e.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            Definition[] defs = {
                new Definition {
                    Name = "",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyDefinitionName));
            Assert.That(e.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyDefinitionItems()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.EmptyExpressionItems));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(defs[0].Expr));
            Assert.That(err.ExpressionItem, Is.SameAs(null));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyExpressionItems()
        {
            var orexpr = new OrExpression();
            var expr = new Expression();


            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);
            orexpr.Expressions.Add(expr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.EmptyExpressionItems));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(expr));
            Assert.That(err.ExpressionItem, Is.SameAs(null));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyOrExpressionExpressions()
        {
            OrExpression orexpr = new OrExpression();

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.EmptyOrexprExpressionList));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.ExpressionItem, Is.SameAs(orexpr));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestNullSubExpressionTag()
        {
            LiteralSubExpression literal = new LiteralSubExpression {
                Value = "literal",
                Tag = null,
            };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullSubexprTag));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(literal));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestNullDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = null };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyDefrefName));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defref));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "" };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyDefrefName));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defref));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestNullLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = null };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyLiteralValue));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(literal));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "" };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyLiteralValue));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(literal));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestNullCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = null };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyCharClass));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(cc));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestEmptyCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = new CharClass(new char[0]) };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.NullOrEmptyCharClass));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(cc));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestDuplicateNames()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                new Definition {
                    Name = "A",
                }
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.DuplicateDefinitionName));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(null));
            Assert.That(err.Definition, Is.SameAs(defs[1]));
            Assert.That(err.Index, Is.EqualTo(1));
        }

        [Test()]
        public void TestAllItemsSkippable()
        {
            Expression expr = new Expression();
            expr.Items.Add(new LiteralSubExpression {
                Value = "literal",
                IsSkippable = true,
            });
            expr.Items.Add(new LiteralSubExpression {
                Value = "literal",
                IsSkippable = true,
            });
            var orexpr = new OrExpression();
            orexpr.Expressions.Add(expr);

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.AllItemsSkippable));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(expr));
            Assert.That(err.ExpressionItem, Is.SameAs(null));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }


//        [Test()]
//        public void TestAnyOrExpressionExpressionSkippable()
//        {
//            OrExpression orexpr = new OrExpression();
//            Expression expr = new Expression();
//            expr.Items.Add(new LiteralSubExpression {
//                Value = "literal",
//                IsSkippable = true,
//            });
//            expr.Items.Add(new LiteralSubExpression {
//                Value = "literal",
//                IsSkippable = true,
//            });
//
//            DefinitionInfo[] defs = {
//                new DefinitionInfo {
//                    Name = "A",
//                    Expression = expr,
//                },
//            };
//
//            ExpressionChecker ec = new ExpressionChecker();
//            List<Error> errors = ec.CheckDefinitionInfos(defs);
//
//            Assert.IsNotNull(errors);
//            Assert.That(errors.Count, Is.EqualTo(1));
//            Assert.That(errors[0].ErrorType,
//                Is.EqualTo(ExpressionChecker.Error.AllItemsSkippable));
//        Assert.IsInstanceOf<EcError>(errors[0]);
//        var err = (errors[0] as EcError);
//            Assert.That(err.Expression, Is.SameAs(expr));
//            Assert.That(err.ExpressionItem, Is.SameAs(null));
//            Assert.That(err.DefinitionInfo, Is.SameAs(defs[0]));


        [Test()]
        public void TestDefRefNotFound()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "qwerty" };

            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType,
                Is.EqualTo(ExpressionError.DefRefNameNotFound));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.That(err.Expression, Is.SameAs(null));
            Assert.That(err.ExpressionItem, Is.SameAs(defref));
            Assert.That(err.Definition, Is.SameAs(defs[0]));
        }

        [Test()]
        public void TestDefinitionReuse()
        {
            Definition def = new Definition {
                Name = "A",
            };
            def.Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            Definition[] defs = {
                def,
                def,
            };

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            ExpressionError e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.ReusedDefintion));
            Assert.That(e.Definition, Is.SameAs(def));
            Assert.That(e.Index, Is.EqualTo(1));
        }

        [Test()]
        public void TestNullDefinition()
        {
            Definition[] defs = {
                new Definition {
                    Name = "A",
                },
                null,
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.That(e.ErrorType,
                Is.EqualTo(ExpressionError.NullDefinition));
            Assert.That(e.Definition, Is.SameAs(null));
            Assert.That(e.Index, Is.EqualTo(1));
        }

    }
}

