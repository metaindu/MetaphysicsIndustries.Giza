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
            var file2 = @"import 'file1.txt';";
            var sgs = new SupergrammarSpanner();
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
    }
}
