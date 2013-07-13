using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;
using NDesk.Options;

namespace giza
{
    class Program
    {
        static OptionSet _options;

        static void Main(string[] args)
        {
            bool showHelp = false;
            bool showVersion = false;

            _options = new OptionSet() {
                { "h|?|help", x => showHelp = true },
                { "v|version", x => showVersion = true },
            };

            var args2 = _options.Parse(args);

            if (showHelp || args.Length < 1)
            {
                ShowUsage();
                return;
            }

            if (showVersion)
            {
                ShowVersion();
                return;
            }

            var command = args2[0].ToLower();
            args2.RemoveAt(0);

            if (command == "super")
            {
                Super(args2);
            }
            else if (command == "render")
            {
                Render(args2);
            }
            else if (command == "tokenize")
            {
                Tokenize(args2);
            }
            else if (command == "span")
            {
                Span(args2);
            }
            else
            {
                Console.WriteLine(string.Format("Unknown command: \"{0}\"", command));
                ShowUsage();
            }
        }

        private static int CountLines(string input, int length)
        {
            int n = 1;
            int i;
            for (i=0;i<length;i++)
            {
                if (input[i] == '\n') n++;
            }
            return n;
        }
        private static int CountCharactersOnLine(string input, int endIndex)
        {
            int n = 0;
            int i = endIndex;
            do
            {
                if (input[i] != '\r') n++;
                i--;
            } while (i >= 0 && input[i] != '\n');

            return n;
        }

        static void ShowVersion()
        {
            Console.WriteLine("giza.exe version x.y.z");
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    giza [options]");
            Console.WriteLine("    giza span [options] [GRAMMAR FILE] [START SYMBOL] [FILE]");
            Console.WriteLine("    giza super [GRAMMAR FILE]");
            //Console.WriteLine("    giza print-super");
            Console.WriteLine("    giza render [GRAMMAR FILE] [CLASS NAME]");
            Console.WriteLine();
            Console.WriteLine("Reads grammar files and parses input.");
            Console.WriteLine();
            Console.WriteLine("    version, -v     Print version and exit successfully.");
            Console.WriteLine("    --help, -h, -?  Print this help and exit successfully.");
            Console.WriteLine("    super,          Process the grammar file only.");
            //Console.WriteLine("    print-super,    Print the supergrammar and exit.");
            Console.WriteLine("    render,         Process the grammar file and print its definitions as a C# class.");
            Console.WriteLine();
            Console.WriteLine("If \"-\" is given for FILE, or for GRAMMAR FILE given to --super, then it is read from standard input.");
            Console.WriteLine();

            _options.WriteOptionDescriptions(Console.Out);
        }

        static void Super(List<string> args)
        {
            if (args.Count < 1)
            {
                ShowUsage();
                return;
            }

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            string gfile;
            if (args[0] == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(args[0]);
            }
            string error;
            var dis = sgs.GetExpressions(gfile, out error);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("There was an error in the grammar: {0}", error);
                return;
            }

            var ec = new ExpressionChecker();
            var errors = ec.CheckDefinitionInfos(dis);

            if (error != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.GetDescription());
                }
            }

            DefinitionBuilder db = new DefinitionBuilder();
            var defs = db.BuildDefinitions(dis);

            Console.WriteLine("There are {0} definitions in the grammar:", defs.Length);
            foreach (var def in defs)
            {
                Console.WriteLine("  {0}", def.Name);
            }
        }

        static void Render(List<string> args)
        {
            if (args.Count < 2)
            {
                ShowUsage();
                return;
            }

            SupergrammarSpanner s = new SupergrammarSpanner();
            string gfile;
            if (args[0] == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(args[0]);
            }
            string error;
            Grammar g = s.GetGrammar(gfile, out error);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }
            else
            {
                string className = args[1];

                DefinitionRenderer dr = new DefinitionRenderer();
                Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions));
            }
        }

        static void Tokenize(List<string> args)
        {
            if (args.Count < 2)
            {
                ShowUsage();
                return;
            }

            SupergrammarSpanner s = new SupergrammarSpanner();
            string gfile;
            if (args[0] == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(args[0]);
            }
            string error;
            Grammar g = s.GetGrammar(gfile, out error);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }
            else
            {
                string infile;
                if (args[1] == "-")
                {
                    infile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
                }
                else
                {
                    infile = File.ReadAllText(args[1]);
                }

                int index;
                if (args.Count < 3)
                {
                    index = 0;
                }
                else if (!int.TryParse(args[2], out index))
                {
                    Console.WriteLine("Error: \"{0}\" is not a valid index.", args[2]);
                    ShowUsage();
                    return;
                }

                Tokenizer t = new Tokenizer(g);
                t.GetTokensAtLocation(infile, index, out error);
            }
        }

        static void Span(List<string> args)
        {
            if (args.Count < 3)
            {
                ShowUsage();
                return;
            }

            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(args[0]);
            string error;
            var dis = spanner.GetExpressions(grammarFile, out error);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("There was an error in the grammar: {0}", error);
                return;
            }

            var ec = new ExpressionChecker();
            var errors = ec.CheckDefinitionInfos(dis);

            if (error != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.GetDescription());
                }
            }

            DefinitionBuilder db = new DefinitionBuilder();
            var defs = db.BuildDefinitions(dis);

            string input;
            if (args[2] == "-")
            {
                input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                input = File.ReadAllText(args[2]);
            }

            Grammar g = new Grammar();
            g.Definitions.AddRange(defs);

            Spanner gs = new Spanner();
            Span[] ss = gs.Process(g, args[1], input, out error);
            if (error != null)
            {
                Console.WriteLine("There was an error in the input: {0}", error);
            }
            else if (ss.Length < 1)
            {
                Console.WriteLine("No valid spans.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("{0} valid spans.", ss.Length);
//                foreach (Span s in ss)
//                {
//                    Console.WriteLine(s.RenderSpanHierarchy());
//                }
            }
            else
            {
                Console.WriteLine("1 valid span.");
            }
        }
    }
}
