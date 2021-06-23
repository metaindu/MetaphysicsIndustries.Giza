using System.Collections.Generic;
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
    }
}
