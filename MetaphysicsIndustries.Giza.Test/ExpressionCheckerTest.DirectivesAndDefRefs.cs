using System;
using System.Collections.Generic;
using NUnit.Framework;
using EcError = MetaphysicsIndustries.Giza.ExpressionChecker.EcError;

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
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestTokenReferencesSubtoken()
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
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void TestTokenReferencesComment()
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
            defs[1].Directives.Add(DefinitionDirective.Token);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestSubtokenReferencesSubtoken()
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
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void TestSubtokenReferencesComment()
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
            defs[1].Directives.Add(DefinitionDirective.Subtoken);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesComment, errors[0].ErrorType);
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestCommentReferencesToken()
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
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestCommentReferencesSubtoken()
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
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void TestCommentReferencesComment()
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
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestCommentReferencesNonToken()
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
            defs[1].Directives.Add(DefinitionDirective.Comment);
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.TokenizedReferencesNonToken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestNonTokenReferencesToken()
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
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NonTokenReferencesSubtoken, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test()]
        public void TestNonTokenReferencesComment()
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.NonTokenReferencesComment, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreEqual(null, err.Expression);
            Assert.AreSame(defs[1].Items[0], err.ExpressionItem);
            Assert.AreSame(defs[1], err.DefinitionInfo);
        }

        [Test]
        public void TestNonTokenReferencesNonToken()
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
            defs[1].Items.Add(new DefRefSubExpression { DefinitionName = "A" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }
    }
}

