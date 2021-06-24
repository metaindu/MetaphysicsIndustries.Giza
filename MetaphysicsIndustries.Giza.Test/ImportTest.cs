using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ImportTest
    {
        [Test]
        public void ImportingFileAddsDefinitionsToCurrentGrammar()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b';";
            var mfs = new MockFileSource((s) => file1);
            var file2 = @"import 'file1.txt';";
            var sgs = new SupergrammarSpanner(mfs);
            var errors = new List<Error>();
            // when
            var result = sgs.GetGrammar(file2, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[1].Name == "def1");
            Assert.That(result.Definitions[0].Name == "def2" ||
                        result.Definitions[1].Name == "def2");
        }

        [Test]
        public void FromStyleImportingOnlyImportsNamedDefinitions()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b';";
            var mfs = new MockFileSource((s) => file1);
            var file2 = @"from 'file1.txt' import def1;";
            var sgs = new SupergrammarSpanner(mfs);
            var errors = new List<Error>();
            // when
            var result = sgs.GetGrammar(file2, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1");
        }

        [Test]
        public void FromStyleImportMultipleDefs()
        {
            // given
            var file1 = @"def1 = 'a'; def2 = 'b'; def3 = 'c';";
            var mfs = new MockFileSource((s) => file1);
            var file2 = @"from 'file1.txt' import def1, def3;";
            var sgs = new SupergrammarSpanner(mfs);
            var errors = new List<Error>();
            // when
            var result = sgs.GetGrammar(file2, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Definitions.Count);
            Assert.That(result.Definitions[0].Name == "def1" ||
                        result.Definitions[1].Name == "def1");
            Assert.That(result.Definitions[0].Name == "def3" ||
                        result.Definitions[1].Name == "def3");
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
            var file2 = @"import 'file1.txt';";
            var sgs = new SupergrammarSpanner(mfs);
            var errors1 = new List<Error>();
            var errors2 = new List<Error>();
            // when
            var result1 = sgs.GetGrammar(file2, errors1);
            var result2 = sgs.GetGrammar(file2, errors2);
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
            int callCount = 0;
            const string file1 = "def1 = 'a';";
            const string file2 = @"import 'file1.txt'; def2 = 'b';";
            const string file3 = @"import 'file1.txt'; def3 = 'c';";
            const string file4 = @"import 'file2.txt'; import 'file3.txt'; def4 = 'd';";
            var mfs = new MockFileSource((s) =>
            {
                if (s == "file1.txt") return file1;
                if (s == "file2.txt") return file2;
                if (s == "file3.txt") return file3;
                if (s == "file4.txt") return file4;

                throw new FileNotFoundException(s);
            });
            var sgs = new SupergrammarSpanner(mfs);
            var errors = new List<Error>();
            // when
            var result = sgs.GetGrammar(file4, errors);
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
            int callCount = 0;
            const string file1 = "def1 = 'a';";
            const string file2 = "def1 = 'b';";
            const string file3 = "import 'file1.txt'; import 'file2.txt';";
            var mfs = new MockFileSource((s) =>
            {
                if (s == "file1.txt") return file1;
                if (s == "file2.txt") return file2;
                if (s == "file3.txt") return file3;

                throw new FileNotFoundException(s);
            });
            var sgs = new SupergrammarSpanner(mfs);
            var errors = new List<Error>();
            // when
            var result = sgs.GetGrammar(file3, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Definitions.Count);
            Assert.AreEqual("def1", result.Definitions[0].Name);
            Assert.AreEqual(1, result.Definitions[0].Nodes.Count);
            Assert.IsInstanceOf<CharNode>(result.Definitions[0].Nodes[0]);
            var node = (CharNode) (result.Definitions[0].Nodes[0]);
            Assert.That(node.CharClass.Matches('b'));
            Assert.That(!node.CharClass.Matches('a'));
        }
    }
}
