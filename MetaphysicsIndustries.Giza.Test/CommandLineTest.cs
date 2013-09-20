using System;
using NUnit.Framework;
using System.IO;

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

        [Test]
        public void TestCommandLine2()
        {
            // setup
            string testGrammarText =
                "expr = value ( [+-*/] value )*; " +
                "value = ( number | varref ); " +
                "<token> number = [\\d]+ ( '.' [\\d]* )?; " +
                "<token> varref = [\\l]+; ";
            StringReader newStdin = new StringReader(testGrammarText);
            StringWriter newStdout = new StringWriter();
            StringWriter newStderr = new StringWriter();
            System.Console.SetIn(newStdin);
            System.Console.SetOut(newStdout);
            System.Console.SetError(newStderr);

            // action
            giza.Program.Main(new string[] { "super", "--tokenized", "-" });

            // assertions
            Assert.AreEqual("", newStderr.ToString());
            Assert.AreEqual(
                "There are 5 definitions in the grammar:\n" +
                "  expr\n" +
                "  value\n" +
                "  number\n" +
                "  varref\n" +
                "  $implicit char class *+-/\n",
                newStdout.ToString());
        }
    }
}

