using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;
using NDesk.Options;

namespace giza
{
    public class Program
    {
        static OptionSet _options;
        static OptionSet _options2;

        static bool showHelp = false;
        static bool showVersion = false;
        static bool verbose = false;
        static bool tokenized = false;

        public static void Main(string[] args)
        {
            _options = new OptionSet() {
                { "h|?|help", x => showHelp = true },
                { "v|version", x => showVersion = true },
                { "verbose", x => verbose = true },
            };
            _options2 = new OptionSet() {
                { "tokenized", x => tokenized = true },
            };

            var args2 = _options.Parse(args);

            try
            {
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
                else if (command == "parse")
                {
                    Parse(args2);
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
            catch (Exception ex)
            {
                Console.Write("There was an internal error");
                if (verbose)
                {
                    Console.WriteLine(":");
                    Console.WriteLine(ex.ToString());
                }
                else
                {
                    Console.WriteLine();
                }
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
            args = _options2.Parse(args);

            if (args.Count < 1)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];

            SupergrammarSpanner sgs = new SupergrammarSpanner();
            string gfile;
            if (grammarFilename == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(grammarFilename);
            }
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(gfile, errors);

            if (errors.Count > 0)
            {
                Console.WriteLine("There were errors in the grammar:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            if (tokenized)
            {
                errors = ec.CheckDefinitionInfosForParsing(dis);
            }
            else
            {
                errors = ec.CheckDefinitionInfos(dis);
            }

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            IEnumerable<Definition> defs;
            if (tokenized)
            {
                TokenizedGrammarBuilder tgb = new TokenizedGrammarBuilder();
                var g = tgb.BuildTokenizedGrammar(dis);
                defs = g.Definitions;
            }
            else
            {
                DefinitionBuilder db = new DefinitionBuilder();
                defs = db.BuildDefinitions(dis);
            }

            Console.WriteLine("There are {0} definitions in the grammar:", defs.Count());
            foreach (var def in defs)
            {
                Console.WriteLine("  {0}", def.Name);
            }
        }

        static void Render(List<string> args)
        {
            args = _options2.Parse(args);

            if (args.Count < 2)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            string className = args[1];

            var sgs = new SupergrammarSpanner();
            string gfile;
            if (grammarFilename == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(grammarFilename);
            }

            List<Error> errors = new List<Error>();
            var dis = sgs.GetExpressions(gfile, errors);

            if (errors.Count > 0)
            {
                Console.WriteLine("There were errors in the grammar:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            if (tokenized)
            {
                errors = ec.CheckDefinitionInfosForParsing(dis);
            }
            else
            {
                errors = ec.CheckDefinitionInfos(dis);
            }

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            Grammar g;

            if (tokenized)
            {
                var tgb = new TokenizedGrammarBuilder();
                g = tgb.BuildTokenizedGrammar(dis);
            }
            else
            {
                var db = new DefinitionBuilder();
                var defs = db.BuildDefinitions(dis);

                g = new Grammar();
                g.Definitions.AddRange(defs);
            }

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }
            else
            {
                var dr = new DefinitionRenderer();
                Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions));
            }
        }

        static void Parse(List<string> args)
        {
            if (args.Count < 3)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            var startSymbol = args[1];
            var inputFilename = args[2];

            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(grammarFilename);
            var errors = new List<Error>();
            var dis = spanner.GetExpressions(grammarFile, errors);

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            errors = ec.CheckDefinitionInfosForParsing(dis);

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            var tgb = new TokenizedGrammarBuilder();
            var g = tgb.BuildTokenizedGrammar(dis);

            string input;
            if (inputFilename == "-")
            {
                input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                input = File.ReadAllText(inputFilename);
            }

            var parser = new Parser(g.FindDefinitionByName(startSymbol));
            errors = new List<Error>();
            Span[] ss = parser.Parse(input, errors);
            //if (error != null)
            //{
            //    Console.WriteLine("There was an error in the input: {0}", error);
            //}
            //else 
            if (ss.Length < 1)
            {
                Console.WriteLine("No valid parses.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("{0} valid parses.", ss.Length);
                //foreach (Span s in ss)
                //{
                //    Console.WriteLine(s.RenderSpanHierarchy());
                //}
            }
            else
            {
                Console.WriteLine("1 valid parse.");
                if (verbose)
                {
                    Console.WriteLine(ss[0].RenderSpanHierarchy());
                }
            }
        }

        static void Span(List<string> args)
        {
            if (args.Count < 3)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            var startSymbol = args[1];
            var inputFilename = args[2];

            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(grammarFilename);
            var errors = new List<Error>();
            var dis = spanner.GetExpressions(grammarFile, errors);

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            errors = ec.CheckDefinitionInfos(dis);

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            DefinitionBuilder db = new DefinitionBuilder();
            var defs = db.BuildDefinitions(dis);

            string input;
            if (inputFilename == "-")
            {
                input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                input = File.ReadAllText(inputFilename);
            }

            Grammar g = new Grammar();
            g.Definitions.AddRange(defs);

            Spanner gs = new Spanner();
            Span[] ss = gs.Process(g, startSymbol, input, errors);
            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the input:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
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
                if (verbose)
                {
                    Console.WriteLine(ss[0].RenderSpanHierarchy());
                }
            }
        }
    }
}
