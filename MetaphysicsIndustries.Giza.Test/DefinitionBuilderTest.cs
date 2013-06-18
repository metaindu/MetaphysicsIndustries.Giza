using System;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class DefinitionBuilderTest
    {
        [Test()]
        public void TestBuildDefintions2()
        {
            DefinitionBuilder db = new DefinitionBuilder();
            Supergrammar sg = new Supergrammar();

            string supergrammarText = File.ReadAllText("/Users/" + Environment.UserName + "/other-projects/MetaphysicsIndustries.Giza/Supergrammar.txt");

            Spanner spanner = new Spanner();
            string error;
            Span[] spans = spanner.Process(sg, "grammar", supergrammarText, out error);

            Assert.IsNull(error);
            Assert.AreEqual(1, spans.Length);
            Span span = spans[0];

            Definition.__id = 0;
            Definition[] defs1 = db.BuildDefinitions(sg, span);
            Definition.__id = 0;
            Definition[] defs2 = db.BuildDefinitions2(sg, span);

            GrammarComparer gc = new GrammarComparer();
            Dictionary<Definition, Definition> defmatchup = new Dictionary<Definition, Definition>();
            int i;
            for (i = 0; i < defs1.Length; i++)
            {
                defmatchup[defs1[i]] = defs2[i];
                defmatchup[defs2[i]] = defs1[i];
            }
            bool value2 = true;
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < defs1.Length; i++)
            {
                bool value = gc.AreEquivalent(defs1[i], defs2[i], defmatchup);
                sb.AppendFormat("defs {0} {1}", i, (value ? "are the same" : "differ"));
                sb.AppendLine();

                value2 = value2 && value;
            }
            Assert.IsTrue(value2);
        }
    }
}

