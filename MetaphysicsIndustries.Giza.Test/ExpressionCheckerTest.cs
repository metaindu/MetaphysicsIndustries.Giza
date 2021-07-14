
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

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(orexprA);
            defs[1].Expr.Items.Add(orexprB);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedExpression, e.ErrorType);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[1], e.Definition);
        }

        [Test()]
        public void TestExpressionItemReuse()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Expr.Items.Add(literal);
            defs[1].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedExpressionItem, e.ErrorType);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[1], e.Definition);
        }

        [Test()]
        public void TestExpressionItemReuseinSameDef()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedExpressionItem, e.ErrorType);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[0], e.Definition);
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

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr2);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedExpression, e.ErrorType);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[0], e.Definition);
        }

        [Test()]
        public void TestReferenceCycleItem()
        {
            OrExpression orexpr = new OrExpression();
            Expression expr2 = new Expression();

            orexpr.Expressions.Add(expr2);
            expr2.Items.Add(orexpr);

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedExpressionItem, e.ErrorType);
            Assert.AreSame(null, e.Expression);
            Assert.AreSame(orexpr, e.ExpressionItem);
            Assert.AreSame(defs[0], e.Definition);
        }

        [Test()]
        public void TestNullDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = null,
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.NullOrEmptyDefinitionName, e.ErrorType);
            Assert.AreSame(defs[0], e.Definition);
        }

        [Test()]
        public void TestEmptyDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.NullOrEmptyDefinitionName, e.ErrorType);
            Assert.AreSame(defs[0], e.Definition);
        }

        [Test()]
        public void TestEmptyDefinitionItems()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.EmptyExpressionItems, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(defs[0].Expr, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestEmptyExpressionItems()
        {
            var orexpr = new OrExpression();
            var expr = new Expression();


            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);
            orexpr.Expressions.Add(expr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.EmptyExpressionItems, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(expr, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestEmptyOrExpressionExpressions()
        {
            OrExpression orexpr = new OrExpression();

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.EmptyOrexprExpressionList, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(orexpr, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestNullSubExpressionTag()
        {
            LiteralSubExpression literal = new LiteralSubExpression {
                Value = "literal",
                Tag = null,
            };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullSubexprTag, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestNullDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = null };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyDefrefName, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestEmptyDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyDefrefName, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestNullLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = null };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyLiteralValue, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestEmptyLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyLiteralValue, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestNullCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = null };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyCharClass, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(cc, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestEmptyCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = new CharClass(new char[0]) };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.NullOrEmptyCharClass, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(cc, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestDuplicateNames()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "A",
                }
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.DuplicateDefinitionName, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[1], err.Definition);
            Assert.AreEqual(1, err.Index);
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

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.AllItemsSkippable, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(expr, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
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
//            Assert.AreEqual(1, errors.Count);
//            Assert.AreEqual(ExpressionChecker.Error.AllItemsSkippable, errors[0].ErrorType);
//        Assert.IsInstanceOf<EcError>(errors[0]);
//        var err = (errors[0] as EcError);
//            Assert.AreSame(expr, err.Expression);
//            Assert.AreSame(null, err.ExpressionItem);
//            Assert.AreSame(defs[0], err.DefinitionInfo);


        [Test()]
        public void TestDefRefNotFound()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "qwerty" };

            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Expr.Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitions(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionError.DefRefNameNotFound, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.Definition);
        }

        [Test()]
        public void TestDefinitionReuse()
        {
            DefinitionExpression def = new DefinitionExpression {
                Name = "A",
            };
            def.Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            DefinitionExpression[] defs = {
                def,
                def,
            };

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            ExpressionError e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.ReusedDefintion, e.ErrorType);
            Assert.AreSame(def, e.Definition);
            Assert.AreEqual(1, e.Index);
        }

        [Test()]
        public void TestNullDefinition()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                null,
            };
            defs[0].Expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitions(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var e = (ExpressionError)errors[0];
            Assert.AreEqual(ExpressionError.NullDefinition, e.ErrorType);
            Assert.AreSame(null, e.Definition);
            Assert.AreEqual(1, e.Index);
        }

    }
}

