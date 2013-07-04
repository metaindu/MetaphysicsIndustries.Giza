using System;
using NUnit.Framework;
using System.Collections.Generic;

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

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedExpressionOrItem, e.Error);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[1], e.DefinitionInfo);
        }

        [Test()]
        public void TestExpressionItemReuse()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };
            Expression exprA = new Expression();
            exprA.Items.Add(literal);
            Expression exprB = new Expression();
            exprB.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = exprA,
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = exprB,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedExpressionOrItem, e.Error);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[1], e.DefinitionInfo);
        }

        [Test()]
        public void TestExpressionItemReuseinSameDef()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };
            Expression expr = new Expression();
            expr.Items.Add(literal);
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedExpressionOrItem, e.Error);
            Assert.AreSame(literal, e.ExpressionItem);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestReferenceCycle()
        {
            OrExpression orexpr = new OrExpression();
            Expression expr = new Expression();

            orexpr.Expressions.Add(expr);
            expr.Items.Add(orexpr);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedExpressionOrItem, e.Error);
            Assert.AreSame(expr, e.Expression);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestReferenceCycleItem()
        {
            OrExpression orexpr = new OrExpression();
            Expression expr = new Expression();
            Expression expr2 = new Expression();

            expr.Items.Add(orexpr);
            orexpr.Expressions.Add(expr2);
            expr2.Items.Add(orexpr);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedExpressionOrItem, e.Error);
            Assert.AreSame(null, e.Expression);
            Assert.AreSame(orexpr, e.ExpressionItem);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestNullDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };
            Expression expr = new Expression();
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = null,
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidDefinitionException e = 
                Assert.Throws<ExpressionChecker.InvalidDefinitionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyDefinitionName, e.Error);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestEmptyDefinitionName()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "literal" };
            Expression expr = new Expression();
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidDefinitionException e = 
                Assert.Throws<ExpressionChecker.InvalidDefinitionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyDefinitionName, e.Error);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestNullDefinitionExpression()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = null,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidExpressionException e = 
                Assert.Throws<ExpressionChecker.InvalidExpressionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.NullDefinitionExpression, e.Error);
            Assert.AreSame(null, e.Expression);
            Assert.AreSame(null, e.ExpressionItem);
            Assert.AreSame(defs[0], e.DefinitionInfo);
        }

        [Test()]
        public void TestEmptyExpressionItems()
        {
            Expression expr = new Expression();

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.EmptyExpressionItems, errors[0].Error);
            Assert.AreSame(expr, errors[0].Expression);
            Assert.AreSame(null, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestEmptyOrExpressionExpressions()
        {
            Expression expr = new Expression();
            OrExpression orexpr = new OrExpression();
            expr.Items.Add(orexpr);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.EmptyOrexprExpressionList, errors[0].Error);
            Assert.AreSame(orexpr, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestNullSubExpressionTag()
        {
            LiteralSubExpression literal = new LiteralSubExpression {
                Value = "literal",
                Tag = null,
            };
            Expression expr = new Expression();
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullSubexprTag, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(literal, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestNullDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = null };
            Expression expr = new Expression();
            expr.Items.Add(defref);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyDefrefName, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(defref, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestEmptyDefRefDefinitionName()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "" };
            Expression expr = new Expression();
            expr.Items.Add(defref);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyDefrefName, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(defref, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestNullLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = null };
            Expression expr = new Expression();
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyLiteralValue, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(literal, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestEmptyLiteralValue()
        {
            LiteralSubExpression literal = new LiteralSubExpression { Value = "" };
            Expression expr = new Expression();
            expr.Items.Add(literal);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyLiteralValue, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(literal, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestNullCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = null };
            Expression expr = new Expression();
            expr.Items.Add(cc);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyCharClass, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(cc, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestEmptyCharClass()
        {
            CharClassSubExpression cc = new CharClassSubExpression { CharClass = new CharClass(new char[0]) };
            Expression expr = new Expression();
            expr.Items.Add(cc);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NullOrEmptyCharClass, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(cc, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestDuplicateNames()
        {
            Expression exprA = new Expression();
            exprA.Items.Add(new LiteralSubExpression { Value = "literal" });


            Expression exprB = new Expression();
            exprB.Items.Add(new LiteralSubExpression { Value = "literal" });

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = exprA,
                },
                new DefinitionInfo {
                    Name = "A",
                    Expression = exprB,
                }
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.DuplicateDefinitionName, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(null, errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
            Assert.AreEqual(1, errors[0].Index);
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

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.AllItemsSkippable, errors[0].Error);
            Assert.AreSame(expr, errors[0].Expression);
            Assert.AreSame(null, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
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
//            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);
//
//            Assert.AreEqual(1, errors.Count);
//            Assert.AreEqual(ExpressionChecker.Error.AllItemsSkippable, errors[0].Error);
//            Assert.AreSame(expr, errors[0].Expression);
//            Assert.AreSame(null, errors[0].ExpressionItem);
//            Assert.AreSame(defs[0], errors[0].DefinitionInfo);


        [Test()]
        public void TestDefRefNotFound()
        {
            DefRefSubExpression defref = new DefRefSubExpression { DefinitionName = "qwerty" };
            Expression expr = new Expression();
            expr.Items.Add(defref);

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
            };

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfos(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.DefRefNameNotFound, errors[0].Error);
            Assert.AreSame(null, errors[0].Expression);
            Assert.AreSame(defref, errors[0].ExpressionItem);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestDefinitionReuse()
        {
            Expression expr = new Expression();
            expr.Items.Add(new LiteralSubExpression { Value = "literal" });
            DefinitionInfo def = new DefinitionInfo {
                Name = "A",
                Expression = expr,
            };

            DefinitionInfo[] defs = {
                def,
                def,
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidDefinitionException e = 
                Assert.Throws<ExpressionChecker.InvalidDefinitionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.ReusedDefintion, e.Error);
            Assert.AreSame(def, e.DefinitionInfo);
            Assert.AreEqual(1, e.Index);
        }

        [Test()]
        public void TestNullDefinition()
        {
            Expression expr = new Expression();
            expr.Items.Add(new LiteralSubExpression { Value = "literal" });

            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = expr,
                },
                null,
            };

            ExpressionChecker ec = new ExpressionChecker();
            ExpressionChecker.InvalidDefinitionException e = 
                Assert.Throws<ExpressionChecker.InvalidDefinitionException>(
                    () => ec.CheckDefinitionInfos(defs)
                );

            Assert.AreEqual(ExpressionChecker.Error.NullDefinition, e.Error);
            Assert.AreSame(null, e.DefinitionInfo);
            Assert.AreEqual(1, e.Index);
        }

        [Test()]
        public void TestTokenInNonTokenized1()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.TokenizedDirectiveInNonTokenizedGrammar, errors[0].Error);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestTokenInNonTokenized2()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.TokenizedDirectiveInNonTokenizedGrammar, errors[0].Error);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestTokenInNonTokenized3()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForSpanning(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.TokenizedDirectiveInNonTokenizedGrammar, errors[0].Error);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestMixedTokenizedDirectives1()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.MixedTokenizedDirectives, errors[0].Error);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestMixedTokenizedDirectives2()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.MixedTokenizedDirectives, errors[0].Error);
            Assert.AreSame(defs[0], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestMixedTokenizedDirectives3()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(0, errors.Count);
        }

        [Test()]
        public void TestReferencedComment()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expression.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.ReferencedComment, errors[0].Error);
            Assert.AreEqual(null, errors[0].Expression);
            Assert.AreSame(defs[1].Expression.Items[0], errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestNonTokenReferencesSubtoken()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Expression.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.NonTokenReferencesSubtoken, errors[0].Error);
            Assert.AreEqual(null, errors[0].Expression);
            Assert.AreSame(defs[1].Expression.Items[0], errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestSubtokenReferencesNonToken()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = new Expression(),
                },
            };
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expression.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.SubtokenReferencesNonToken, errors[0].Error);
            Assert.AreEqual(null, errors[0].Expression);
            Assert.AreSame(defs[1].Expression.Items[0], errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestTokenReferencesNonToken()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = new Expression(),
                },
            };
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Expression.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.TokenReferencesNonToken, errors[0].Error);
            Assert.AreEqual(null, errors[0].Expression);
            Assert.AreSame(defs[1].Expression.Items[0], errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
        }

        [Test()]
        public void TestSubtokenReferencesToken()
        {
            DefinitionInfo[] defs = {
                new DefinitionInfo {
                    Name = "A",
                    Expression = new Expression(),
                },
                new DefinitionInfo {
                    Name = "B",
                    Expression = new Expression(),
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Expression.Items.Add(new LiteralSubExpression { Value = "literal" });
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Expression.Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(ExpressionChecker.Error.SubtokenReferencesToken, errors[0].Error);
            Assert.AreEqual(null, errors[0].Expression);
            Assert.AreSame(defs[1].Expression.Items[0], errors[0].ExpressionItem);
            Assert.AreSame(defs[1], errors[0].DefinitionInfo);
        }
    }
}

