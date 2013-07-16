using System;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class CommandLineTest
    {
        [Test]
        public void TestCommandLine()
        {
            giza.Program.Main(new string[] { "super", "../../../giza/json.giza", "--tokenized" });
        }
    }
}

