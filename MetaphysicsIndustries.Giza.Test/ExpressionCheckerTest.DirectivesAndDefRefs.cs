using System;
using System.Collections.Generic;
using NUnit.Framework;
using EcError = MetaphysicsIndustries.Giza.ExpressionChecker.EcError;

namespace MetaphysicsIndustries.Giza.Test
{
    public partial class ExpressionCheckerTest
    {

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

