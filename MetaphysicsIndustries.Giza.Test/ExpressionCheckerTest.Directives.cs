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
            Assert.AreEqual(ExpressionError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as ExpressionError).DefinitionInfo);
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
            Assert.AreEqual(ExpressionError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as ExpressionError).DefinitionInfo);
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
            Assert.AreEqual(ExpressionError.TokenizedDirectiveInNonTokenizedGrammar, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            Assert.AreSame(defs[0], (errors[0] as ExpressionError).DefinitionInfo);
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
            Assert.AreEqual(ExpressionError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MixedTokenizedDirectives, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.AtomicInNonTokenDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MindWhitespaceInNonTokenDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInTokenDefinition()
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
            Assert.AreEqual(ExpressionError.AtomicInTokenDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }

        [Test]
        public void TestAtomicInSubtokenDefinition()
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
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void TestAtomicInCommentDefinition()
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
            Assert.AreEqual(ExpressionError.AtomicInCommentDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
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
            Assert.AreEqual(ExpressionError.MindWhitespaceInTokenizedDefinition, errors[0].ErrorType);
            Assert.IsInstanceOf<ExpressionError>(errors[0]);
            var err = (errors[0] as ExpressionError);
            Assert.AreSame(defs[0], err.DefinitionInfo);
        }
    }
}

