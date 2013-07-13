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
        static void Main(string[] args)
        {
            bool showHelp = false;
            bool showVersion = false;

            OptionSet options = new OptionSet() {
                { "h|?|help", x => showHelp = true },
                { "v|version", x => showVersion = true },
            };

            var args2 = options.Parse(args);

            if (showHelp || args.Length < 1)
            {
                ShowUsage(options);
            }
            else if (showVersion)
            {
                ShowVersion();
            }
            else if (args2[0].ToLower() == "super")
            {
                Super(args);
            }
            else if (args2[0].ToLower() == "render")
            {
                Render(args);
            }
            else if (args2[0].ToLower() == "tokenize")
            {
                Tokenize(args);
            }
            else if (args2[0] == "span")
            {
                Span(args);
            }
            else
            {
                Console.WriteLine(string.Format("Unknown command: \"{0}\"", args2[0]));
                ShowUsage(options);
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

        static void ShowUsage(OptionSet options)
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

            options.WriteOptionDescriptions(Console.Out);
        }

        static void Super(string[] args)
        {
            if (args.Length > 1)
            {
                SupergrammarSpanner s = new SupergrammarSpanner();
                string gfile;
                if (args[1] == "-")
                {
                    gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
                }
                else
                {
                    gfile = File.ReadAllText(args[1]);
                }
                if (args.Length > 2 && args[2] == "-2")
                {
                    Supergrammar supergrammar = new Supergrammar();
                    Spanner spanner = new Spanner();
                    string error;
                    Span[] ss = spanner.Process(supergrammar, "grammar", gfile, out error);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine(error);
                    }
                    else if (ss.Length != 1)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        DefinitionBuilder db = new DefinitionBuilder();
                        DefinitionRenderer dr = new DefinitionRenderer();
                        if (ss.Length != 1)
                        {
                            foreach (Span span in ss)
                            {
                                SpanChecker sc = new SpanChecker();
                                if (sc.CheckSpan(span, supergrammar).Count > 0)
                                {
                                    throw new InvalidOperationException();
                                }

                                ExpressionBuilder eb = new ExpressionBuilder();
                                DefinitionInfo[] dis = eb.BuildExpressions(supergrammar, span);
                                Definition[] dd2 = db.BuildDefinitions(dis);
                                string class2 = dr.RenderDefinitionsAsCSharpClass("FromBuildDefs2", dd2);
                                class2 = class2;

                                DefinitionChecker dc = new DefinitionChecker();
                                if (dc.CheckDefinitions(dd2).Count() > 0)
                                {
                                    throw new InvalidOperationException();
                                }

                                //                                    DefinitionCheckerTest dct = new DefinitionCheckerTest();
                                //                                    dct.TestSingleDefCycle();
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
//                ShowUsage();
            }
        }

        static void Render(string[] args)
        {
            if (args.Length > 2)
            {
                SupergrammarSpanner s = new SupergrammarSpanner();
                string gfile;
                if (args[1] == "-")
                {
                    gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
                }
                else
                {
                    gfile = File.ReadAllText(args[1]);
                }
                string error;
                Grammar g = s.GetGrammar(gfile, out error);

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
                else
                {
                    string className = args[2];

                    DefinitionRenderer dr = new DefinitionRenderer();
                    Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions));
                }
            }
            else
            {
//                ShowUsage();
            }
        }

        static void Tokenize(string[] args)
        {
            SupergrammarSpanner s = new SupergrammarSpanner();
            string gfile;
            if (args[1] == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(args[1]);
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
                if (args[2] == "-")
                {
                    infile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
                }
                else
                {
                    infile = File.ReadAllText(args[2]);
                }

                int index;
                if (args.Length < 4)
                {
                    index = 0;
                }
                else if (!int.TryParse(args[3], out index))
                {
                    Console.WriteLine("\"{0}\" is not a valid index.", args[3]);
                    return;
                }

                Tokenizer t = new Tokenizer(g);
                t.GetTokensAtLocation(infile, index, out error);
            }
        }

        static void Span(string[] args)
        {
            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(args[0]);
            string error;
            Grammar g = spanner.GetGrammar(grammarFile, out error);

            if (error != null)
            {
                Console.WriteLine(error);
            }
            else
            {
                string input;
                if (args[2] == "-")
                {
                    input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
                }
                else
                {
                    input = File.ReadAllText(args[2]);
                }

                Spanner gs = new Spanner();
                Span[] ss = gs.Process(g, args[1], input, out error);
                if (error != null)
                {
                    Console.WriteLine(error);
                }
                else
                {
                    DefinitionBuilder db = new DefinitionBuilder();
                    foreach (Span span in ss)
                    {
                        //                    Definition[] dd = db.BuildDefinitions2(g, span);
                    }
                }
            }
        }
    }
}
