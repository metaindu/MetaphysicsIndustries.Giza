using System.Collections.Generic;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class SupergrammarSpannerTest
    {
        [Test]
        public void TestGetGrammarSetsSources()
        {
            const string input = "def1 = 'a';";
            const string source = "grammar1.giza";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetGrammar(input, errors, source);
            // then
            Assert.IsNotNull(result);
            Assert.AreEqual(source, result.Source);
            Assert.AreEqual(1, result.Definitions.Count);
            Assert.AreEqual("def1", result.Definitions[0].Name);
            Assert.AreEqual(source, result.Definitions[0].Source);
        }
    }
}
