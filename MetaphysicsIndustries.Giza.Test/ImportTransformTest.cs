using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ImportTransformTest
    {
        [Test]
        public void ImportingFileAddsDefinitionsToCurrentGrammar()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b';";
            var mfs = new MockFileSource((s) => file1);

            // import 'file1.txt';
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        IsModuleImport = false,
                        ModuleOrFile = "file1.txt",
                    }
                }
            };
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[1].Name == "def1");
            Assert.That(result.Definitions[0].Name == "def2" ||
                        result.Definitions[1].Name == "def2");
            Assert.IsTrue(result.Definitions[0].IsImported);
            Assert.IsTrue(result.Definitions[1].IsImported);
        }

        [Test]
        public void FromStyleImportingOnlyImportsNamedDefinitions2()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b';";
            var mfs = new MockFileSource((s) => file1);
            // from 'file1.txt' import def1;
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        ModuleOrFile = "file1.txt",
                        ImportRefs = new []
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def1",
                            }
                        }
                    }
                }
            };
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1");
            Assert.IsTrue(result.Definitions[0].IsImported);
        }

        [Test]
        public void FromStyleImportMultipleDefs()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b'; def3 = 'c';";
            var mfs = new MockFileSource((s) => file1);

            // from 'file1.txt' import def1, def3;
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        ModuleOrFile = "file1.txt",
                        ImportRefs = new[]
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def1",
                            },
                            new ImportRef
                            {
                                SourceName = "def3",
                                DestName = "def3",
                            },
                        }
                    }
                }
            };
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[1].Name == "def1");
            Assert.That(result.Definitions[0].Name == "def3" ||
                        result.Definitions[1].Name == "def3");
            Assert.IsTrue(result.Definitions[0].IsImported);
            Assert.IsTrue(result.Definitions[1].IsImported);
        }

        [Test]
        public void ImportingSingleDefinitionMultipleTimesOnlyReadsFileOnce()
        {
            // given
            int callCount = 0;
            var mfs = new MockFileSource((s) =>
            {
                callCount++;
                return "def1 = 'a';";
            });
            // import 'file1.txt';
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        IsModuleImport = false,
                        ModuleOrFile = "file1.txt",
                    }
                }
            };

            var importer = new ImportTransform(mfs);
            var errors1 = new List<Error>();
            var errors2 = new List<Error>();
            // when
            var result1 = importer.Transform(pg, errors1, mfs);
            var result2 = importer.Transform(pg, errors2, mfs);
            // then
            Assert.AreEqual(0, errors1.Count);
            Assert.IsNotNull(result1);
            Assert.AreEqual(1, result1.Definitions.Count);
            Assert.That(result1.Definitions[0].Name == "def1");
            // and
            Assert.AreEqual(0, errors2.Count);
            Assert.IsNotNull(result2);
            Assert.AreEqual(1, result2.Definitions.Count);
            Assert.That(result2.Definitions[0].Name == "def1");
            // and
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void DiamondPatternAllDefinitionsImported()
        {
            // given
            const string file1 = "def1 = 'a';";
            const string file2 = @"import 'file1.txt'; def2 = 'b';";
            const string file3 = @"import 'file1.txt'; def3 = 'c';";

            // import 'file2.txt';
            // import 'file3.txt';
            // def4 = 'd';
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>
                {
                    new DefinitionExpression("def4",
                        expr: new Expression(new LiteralSubExpression("d"))),
                },
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        ModuleOrFile = "file2.txt",
                        ImportAll = true
                    },
                    new ImportStatement
                    {
                        ModuleOrFile = "file3.txt",
                        ImportAll = true
                    },
                }
            };

            var mfs = new MockFileSource(s =>
            {
                if (s == "file1.txt") return file1;
                if (s == "file2.txt") return file2;
                if (s == "file3.txt") return file3;

                throw new FileNotFoundException(s);
            });
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[0].Name == "def2" ||
                        result.Definitions[0].Name == "def3" ||
                        result.Definitions[0].Name == "def4");
            Assert.That(result.Definitions[1].Name == "def1" ||
                        result.Definitions[1].Name == "def2" ||
                        result.Definitions[1].Name == "def3" ||
                        result.Definitions[1].Name == "def4");
            Assert.That(result.Definitions[2].Name == "def1" ||
                        result.Definitions[2].Name == "def2" ||
                        result.Definitions[2].Name == "def3" ||
                        result.Definitions[2].Name == "def4");
            Assert.That(result.Definitions[3].Name == "def1" ||
                        result.Definitions[3].Name == "def2" ||
                        result.Definitions[3].Name == "def3" ||
                        result.Definitions[3].Name == "def4");
        }

        [Test]
        public void ImportedDefinitionReplacesExistingWithSameName()
        {
            // given
            const string file1 = "def1 = 'a';";
            const string file2 = "def1 = 'b';";
            var mfs = new MockFileSource((s) =>
            {
                if (s == "file1.txt") return file1;
                if (s == "file2.txt") return file2;

                throw new FileNotFoundException(s);
            });

            // import 'file1.txt';
            // import 'file2.txt';
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        ModuleOrFile = "file1.txt",
                        ImportAll = true
                    },
                    new ImportStatement
                    {
                        ModuleOrFile = "file2.txt",
                        ImportAll = true
                    },
                }
            };
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Definitions.Count);
            Assert.AreEqual("def1", result.Definitions[0].Name);
            Assert.AreEqual(1, result.Definitions[0].Expr.Items.Count);
            Assert.IsInstanceOf<LiteralSubExpression>(result.Definitions[0].Expr.Items[0]);
            var literal = (LiteralSubExpression) result.Definitions[0].Expr.Items[0];
            Assert.AreEqual("b", literal.Value);
        }

        [Test]
        public void AliasRenamesImportedDefinition2()
        {
            // given
            const string file1 = "def1 = 'a';";
            const string file2 = "def1 = 'b';";
            var mfs = new MockFileSource((s) =>
            {
                if (s == "file1.txt") return file1;
                if (s == "file2.txt") return file2;

                throw new FileNotFoundException(s);
            });
            // from 'file1.txt' import def1;
            // from 'file2.txt' import def1 as def2;
            var pg = new PreGrammar
            {
                Definitions = new List<DefinitionExpression>(),
                ImportStatements = new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        ModuleOrFile = "file1.txt",
                        ImportRefs = new[]
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def1",
                            }
                        }
                    },
                    new ImportStatement
                    {
                        ModuleOrFile = "file2.txt",
                        ImportRefs = new[]
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def2",
                            }
                        }
                    },
                }
            };
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(pg, errors, mfs);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Definitions.Count);

            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[0].Name == "def2");
            Assert.That(result.Definitions[1].Name == "def1" ||
                        result.Definitions[1].Name == "def2");
            var def1 = (result.Definitions[0].Name == "def1" ?
                result.Definitions[0] : result.Definitions[1]);
            var def2 = (result.Definitions[0].Name == "def2" ?
                result.Definitions[0] : result.Definitions[1]);

            Assert.AreEqual(1, def1.Expr.Items.Count);
            Assert.IsInstanceOf<LiteralSubExpression>(def1.Expr.Items[0]);
            var literal1 = (LiteralSubExpression) def1.Expr.Items[0];
            Assert.AreEqual("a", literal1.Value);

            Assert.AreEqual(1, def2.Expr.Items.Count);
            Assert.IsInstanceOf<LiteralSubExpression>(def2.Expr.Items[0]);
            var literal2 = (LiteralSubExpression) def2.Expr.Items[0];
            Assert.AreEqual("b", literal2.Value);
        }
    }
}
