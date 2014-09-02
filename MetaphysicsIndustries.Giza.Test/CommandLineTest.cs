using System;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class CommandLineTest
    {
        [Test]
        public void TestCommandLine()
        {
            giza.Program.Main(new string[] { "check", "../../../giza/json.giza", "--tokenized" });
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
            giza.Program.Main(new string[] { "check", "--tokenized", "-" });

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

        class MultipleStdIn : TextReader
        {
            public readonly List<string> InputStreams = new List<string>();
            public int Index = 0;

            public override string ReadToEnd()
            {
                var stream = InputStreams[Index];
                Index++;
                return stream;
            }
        }

        [Test]
        public void TestCommandLineParse()
        {
            // setup
            string testGrammarText =
                "expr = value ( [+-*/] value )*; " +
                "value = ( number | varref ); " +
                "<token> number = [\\d]+ ( '.' [\\d]* )?; " +
                "<token> varref = [\\l]+; ";
            string inputText = "1234.6 + abc - 123. * zxcv";
            var stdin = new MultipleStdIn();
            stdin.InputStreams.Add(testGrammarText);
            stdin.InputStreams.Add(inputText);

            StringWriter newStdout = new StringWriter();
            StringWriter newStderr = new StringWriter();
            System.Console.SetIn(stdin);
            System.Console.SetOut(newStdout);
            System.Console.SetError(newStderr);

            // action
            giza.Program.Main(new string[] { "parse", "-", "expr", "-" });

            // assertions
            Assert.AreEqual("", newStderr.ToString());
            Assert.AreEqual(
                "There is 1 valid parse of the input.\n",
                newStdout.ToString());
        }

        [Test]
        public void TestCommandLineSpan()
        {
            // setup
            string testGrammarText =
                "expr = value ( [+-*/] value )*; " +
                "value = ( number | varref ); " +
                "<mind whitespace> number = [\\d]+ ( '.' [\\d]* )?; " +
                "<mind whitespace> varref = [\\l]+; ";
            string inputText = "1234.6 + abc - 123. * zxcv";
            var stdin = new MultipleStdIn();
            stdin.InputStreams.Add(testGrammarText);
            stdin.InputStreams.Add(inputText);

            StringWriter newStdout = new StringWriter();
            StringWriter newStderr = new StringWriter();
            System.Console.SetIn(stdin);
            System.Console.SetOut(newStdout);
            System.Console.SetError(newStderr);

            // action
            giza.Program.Main(new string[] { "span", "-", "expr", "-" });

            // assertions
            Assert.AreEqual("", newStderr.ToString());
            Assert.AreEqual(
                "1 valid span.\n",
                newStdout.ToString());
        }
    }
}

