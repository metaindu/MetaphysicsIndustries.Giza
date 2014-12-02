using System;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;


namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class CommandLineTest
    {
        public CommandLineTest()
        {
            giza.Program.UseLineEditor = false;
        }

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
            public readonly List<TextReader> Inputs = new List<TextReader>();
            public int Index = 0;

            bool atEnd = false;
            protected TextReader GetNextAvailableReader()
            {
                if (Inputs[Index].Peek() < 0)
                {
                    if (atEnd)
                    {
                        Index++;
                        atEnd = false;
                    }
                    else
                    {
                        atEnd = true;
                    }
                }

                return Inputs[Index];
            }

            public void AddInput(TextReader input)
            {
                Inputs.Add(input);
            }
            public void AddInput(string input)
            {
                Inputs.Add(new StringReader(input));
            }

            public override string ReadToEnd()
            {
                return GetNextAvailableReader().ReadToEnd();
            }

            public override string ReadLine()
            {
                return GetNextAvailableReader().ReadLine();
            }

            public override void Close()
            {
                foreach (var input in Inputs)
                {
                    input.Close();
                }

                base.Close();
            }

            public override int Peek()
            {
                return GetNextAvailableReader().Peek();
            }

            public override int Read()
            {
                return GetNextAvailableReader().Read();
            }

            public override int Read(char[] buffer, int index, int count)
            {
                return GetNextAvailableReader().Read(buffer, index, count);
            }

            public override int ReadBlock(char[] buffer, int index, int count)
            {
                return GetNextAvailableReader().ReadBlock(buffer, index, count);
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
            stdin.AddInput(testGrammarText);
            stdin.AddInput(inputText);

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
            stdin.AddInput(testGrammarText);
            stdin.AddInput(inputText);

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
                "There is 1 valid span of the input.\n",
                newStdout.ToString());
        }
    }
}

