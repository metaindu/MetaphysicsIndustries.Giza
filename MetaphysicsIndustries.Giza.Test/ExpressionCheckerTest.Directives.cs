using System;
using NUnit.Framework;
using System.Collections.Generic;
using EcError = MetaphysicsIndustries.Giza.ExpressionChecker.EcError;

namespace MetaphysicsIndustries.Giza.Test
{
    public partial class ExpressionCheckerTest
    {
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

            Assert.IsNotNull(errors);
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

            Assert.IsNotNull(errors);
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

            Assert.IsNotNull(errors);
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

            Assert.IsNotNull(errors);
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

            Assert.IsNotNull(errors);
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

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInNonTokenDefinition()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.AtomicInNonTokenDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestMindWhitespaceInNonTokenDefinition()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MindWhitespaceInNonTokenDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInTokenizedDefinition1()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.AtomicInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInTokenizedDefinition2()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.AtomicInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInTokenizedDefinition3()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.Atomic);
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.AtomicInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition1()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Token);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition2()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Subtoken);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestMindWhitespaceInTokenizedDefinition3()
        {
            DefinitionExpression[] defs = {
                new DefinitionExpression {
                    Name = "A",
                },
            };
            defs[0].Directives.Add(DefinitionDirective.MindWhitespace);
            defs[0].Directives.Add(DefinitionDirective.Comment);
            defs[0].Items.Add(new LiteralSubExpression { Value = "literal" });

            ExpressionChecker ec = new ExpressionChecker();
            List<Error> errors = ec.CheckDefinitionInfosForParsing(defs);

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(EcError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<EcError>(errors[0]);
            var err = (errors[0] as EcError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }
    }
}

