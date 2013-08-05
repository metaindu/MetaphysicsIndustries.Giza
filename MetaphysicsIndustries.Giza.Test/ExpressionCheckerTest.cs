using System;
using NUnit.Framework;
using System.Collections.Generic;
using EcError = MetaphysicsIndustries.Giza.ExpressionChecker.EcError;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class ExpressionCheckerTest
    {
        [Test()]
        public void TestNullDefinitionInfoArray()
        {
            ExpressionChecker ec = new ExpressionChecker();

            Assert.Throws<ArgumentNullException>(() => ec.CheckDefinitionInfos(null));
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
            defs[0].Items.Add(orexprA);
            defs[1].Items.Add(orexprB);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedExpressionOrItem, e.ErrorType);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[1], e.DefinitionInfo);
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
            defs[0].Items.Add(literal);
            defs[1].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedExpressionOrItem, e.ErrorType);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[1], e.DefinitionInfo);
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
            defs[0].Items.Add(literal);
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedExpressionOrItem, e.ErrorType);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[0], e.DefinitionInfo);
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
            defs[0].Items.Add(orexpr2);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedExpressionOrItem, e.ErrorType);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[0], e.DefinitionInfo);
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
            defs[0].Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedExpressionOrItem, e.ErrorType);
            Assert.AreSame(null, e.Expression);
            Assert.AreSame(orexpr, e.ExpressionItem);
            Assert.AreSame(defs[0], e.DefinitionInfo);
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
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.NullOrEmptyDefinitionName, e.ErrorType);
            Assert.AreSame(defs[0], e.DefinitionInfo);
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
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.NullOrEmptyDefinitionName, e.ErrorType);
            Assert.AreSame(defs[0], e.DefinitionInfo);
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
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.EmptyExpressionItems, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(orexpr);
            orexpr.Expressions.Add(expr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.EmptyExpressionItems, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(expr, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.EmptyOrexprExpressionList, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(orexpr, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullSubexprTag, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyDefrefName, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyDefrefName, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyLiteralValue, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(literal);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyLiteralValue, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(literal, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyCharClass, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(cc, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(cc);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NullOrEmptyCharClass, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(cc, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.DuplicateDefinitionName, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
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
            defs[0].Items.Add(orexpr);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.AllItemsSkippable, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(expr, err.Expression);
            Assert.AreSame(null, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
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
            defs[0].Items.Add(defref);

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.DefRefNameNotFound, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(null, err.Expression);
            Assert.AreSame(defref, err.ExpressionItem);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test()]
        public void TestDefinitionReuse()
        {
            DefinitionExpression def = new DefinitionExpression {
                Name = "A",
            };
            def.Items.Add(new LiteralSubExpression { Value = "literal" });

            DefinitionExpression[] defs = {
                def,
                def,
            };

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            EcError e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.ReusedDefintion, e.ErrorType);
            Assert.AreSame(def, e.DefinitionInfo);
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
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();


            var errors = ec.CheckDefinitionInfos(defs);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ExpressionChecker.EcError>(errors[0]);
            var e = (ExpressionChecker.EcError)errors[0];
            Assert.AreEqual(EcError.NullDefinition, e.ErrorType);
            Assert.AreSame(null, e.DefinitionInfo);
            Assert.AreEqual(1, e.Index);
        }

        [Test()]
        public void TestTokenInNonTokenized1()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as EcError).DefinitionInfo);
        }

        [Test()]
        public void TestTokenInNonTokenized2()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as EcError).DefinitionInfo);
        }

        [Test()]
        public void TestTokenInNonTokenized3()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as EcError).DefinitionInfo);
        }


        [Test()]
        public void TestMixedTokenizedDirectives1()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test()]
        public void TestMixedTokenizedDirectives2()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test()]
        public void TestMixedTokenizedDirectives3()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(0, errors.Count);
        }

        [Test()]
        public void TestReferencedComment()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.ReferencedComment, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test()]
        public void TestNonTokenReferencesSubtoken()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NonTokenReferencesSubtoken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test()]
        public void TestSubtokenReferencesNonToken()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.SubtokenReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test()]
        public void TestTokenReferencesNonToken()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test()]
        public void TestSubtokenReferencesToken()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
                new DefinitionExpression {
                    Name = "B",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.SubtokenReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }
    }
}

