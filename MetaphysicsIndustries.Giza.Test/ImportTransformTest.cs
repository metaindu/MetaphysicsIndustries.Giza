
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                    }
                });
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(2));
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                        ImportRefs = new[]
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def1",
                            }
                        }
                    }
                });
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(1));
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
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
                });
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(2));
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                    }
                });

            var importer = new ImportTransform(mfs);
            var errors1 = new List<Error>();
            var errors2 = new List<Error>();
            // when
            var result1 = importer.Transform(g, errors1, mfs);
            var result2 = importer.Transform(g, errors2, mfs);
            // then
            Assert.That(errors1.Count, Is.EqualTo(0));
            Assert.IsNotNull(result1);
            Assert.That(result1.Definitions.Count, Is.EqualTo(1));
            Assert.That(result1.Definitions[0].Name == "def1");
            // and
            Assert.That(errors2.Count, Is.EqualTo(0));
            Assert.IsNotNull(result2);
            Assert.That(result2.Definitions.Count, Is.EqualTo(1));
            Assert.That(result2.Definitions[0].Name == "def1");
            // and
            Assert.That(callCount, Is.EqualTo(1));
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
            var g = new Grammar(
                new List<Definition>
                {
                    new Definition("def4",
                        expr: new Expression(new LiteralSubExpression("d"))),
                },
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file2.txt",
                        ImportAll = true
                    },
                    new ImportStatement
                    {
                        Filename = "file3.txt",
                        ImportAll = true
                    },
                });

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
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(4));
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                        ImportAll = true
                    },
                    new ImportStatement
                    {
                        Filename = "file2.txt",
                        ImportAll = true
                    },
                });
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(1));
            Assert.That(result.Definitions[0].Name, Is.EqualTo("def1"));
            Assert.That(result.Definitions[0].Expr.Items.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<LiteralSubExpression>(result.Definitions[0].Expr.Items[0]);
            var literal = (LiteralSubExpression) result.Definitions[0].Expr.Items[0];
            Assert.That(literal.Value, Is.EqualTo("b"));
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
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
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
                        Filename = "file2.txt",
                        ImportRefs = new[]
                        {
                            new ImportRef
                            {
                                SourceName = "def1",
                                DestName = "def2",
                            }
                        }
                    },
                });
            var importer = new ImportTransform(mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(2));

            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[0].Name == "def2");
            Assert.That(result.Definitions[1].Name == "def1" ||
                        result.Definitions[1].Name == "def2");
            var def1 = (result.Definitions[0].Name == "def1" ?
                result.Definitions[0] : result.Definitions[1]);
            var def2 = (result.Definitions[0].Name == "def2" ?
                result.Definitions[0] : result.Definitions[1]);

            Assert.That(def1.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<LiteralSubExpression>(def1.Expr.Items[0]);
            var literal1 = (LiteralSubExpression) def1.Expr.Items[0];
            Assert.That(literal1.Value, Is.EqualTo("a"));

            Assert.That(def2.Expr.Items.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<LiteralSubExpression>(def2.Expr.Items[0]);
            var literal2 = (LiteralSubExpression) def2.Expr.Items[0];
            Assert.That(literal2.Value, Is.EqualTo("b"));
        }

        [Test]
        public void ImportingSetsTheDefinitionSource()
        {
            // given
            var file1 = @"def1 = 'a';";
            var mfs = new MockFileSource((s) => file1);

            // import 'file1.txt';
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                    }
                },
                "src1");
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Source, Is.EqualTo("src1"));
            Assert.That(result.Definitions.Count, Is.EqualTo(1));
            Assert.That(result.Definitions[0].Name, Is.EqualTo("def1"));
            Assert.IsTrue(result.Definitions[0].IsImported);
            Assert.That(result.Definitions[0].Source, Is.EqualTo("file1.txt"));
        }

        [Test]
        public void ImportingRelativePathSetsSource()
        {
            // given
            const string file1 = "import '../path2/file2.giza';";
            const string file2 = "def1 = 'a';";
            var mfs = new MockFileSource(s =>
                {
                    if (s == "/base/path1/file1.giza") return file1;
                    if (s == "/base/path2/file2.giza") return file2;

                    throw new FileNotFoundException(s);
                },
                "/base");

            // import '../path2/file2.giza';
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "../path2/file2.giza",
                    }
                },
                "/base/path1/file1.giza");
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Source, Is.EqualTo("/base/path1/file1.giza"));
            Assert.That(result.Definitions.Count, Is.EqualTo(1));
            Assert.That(result.Definitions[0].Name == "def1");
            Assert.IsTrue(result.Definitions[0].IsImported);
            Assert.That(result.Definitions[0].Source,
                Is.EqualTo("/base/path2/file2.giza"));
        }

        [Test]
        public void TransformRemovesImportStatments()
        {
            // given
            const string file1 = "";
            var mfs = new MockFileSource(s => file1);

            // import 'file1.txt';
            var g = new Grammar(
                new List<Definition>(),
                new List<ImportStatement>
                {
                    new ImportStatement
                    {
                        Filename = "file1.txt",
                    }
                },
                "src");
            var importer = new ImportTransform(fileSource: mfs);
            var errors = new List<Error>();
            // when
            var result = importer.Transform(g, errors, mfs);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(0));
            Assert.That(result.ImportStatements.Count, Is.EqualTo(0));
            Assert.That(result.Source, Is.EqualTo("src"));
        }
    }
}
