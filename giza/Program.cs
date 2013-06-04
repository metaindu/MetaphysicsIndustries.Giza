using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;

namespace giza
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowUsage();
            }
            else if (args[0].ToLower() == "--version" ||
                     args[0].ToLower() == "-v")
            {

            }
            else if (args[0].ToLower() == "--help")
            {
                ShowUsage();
            }
            else if (args[0].ToLower() == "--super")
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
                        Definition.__id = 0;
                        Spanner spanner = new Spanner();
                        string error;
                        Span[] ss = spanner.Process(supergrammar.Definitions.ToArray(), "grammar", gfile, out error);
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
                            Definition.__id = 0;
                            DefinitionRenderer dr = new DefinitionRenderer();
                            if (ss.Length != 1)
                            foreach (Span span in ss)
                            {
                                Definition.__id = 0;

                                SpanChecker sc = new SpanChecker();
                                if (sc.CheckSpan(span, supergrammar).Count > 0)
                                {
                                    throw new InvalidOperationException();
                                }

                                Definition[] dd2 = db.BuildDefinitions(supergrammar, span);
                                string class2 = dr.RenderDefinitionsAsCSharpClass("FromBuildDefs2", dd2);
                                class2 = class2;

                                DefinitionChecker dc = new DefinitionChecker();
                                if (dc.CheckDefinitions(dd2).Count() > 0)
                                {
                                    throw new InvalidOperationException();
                                }

                                DefinitionCheckerTest dct = new DefinitionCheckerTest();
                                dct.TestSingleDefCycle();
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
                    ShowUsage();
                }
            }
            else if (args[0].ToLower() == "--render")
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
                    ShowUsage();
                }
            }
            else if (args.Length < 3)
            {
                ShowUsage();
            }
            else
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
                    Span[] ss = gs.Process(g.Definitions.ToArray(), args[1], input, out error);
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

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    giza [options]");
            Console.WriteLine("    giza [options] [GRAMMAR FILE] [START SYMBOL] [FILE]");
            Console.WriteLine("    giza --super [GRAMMAR FILE]");
            //Console.WriteLine("    giza --print-super");
            //Console.WriteLine("    giza --compile [GRAMMAR FILE] [START SYMBOL] [OUTPUT EXE]");
            Console.WriteLine("    giza --render [GRAMMAR FILE] [CLASS NAME]");
            Console.WriteLine();
            Console.WriteLine("Reads grammar files and parses input.");
            Console.WriteLine();
            Console.WriteLine("    --version, -[vV]  Print version and exit successfully.");
            Console.WriteLine("    --help,           Print this help and exit successfully.");
            Console.WriteLine("    --super,          Process the grammar file only.");
            //Console.WriteLine("    --print-super,    Print the supergrammar and exit.");
            //Console.WriteLine("    --compile,      Print the supergrammar and exit.");
            Console.WriteLine("    --render,         Process the grammar file and print its definitions as a C# class.");
            Console.WriteLine();
            Console.WriteLine("If \"-\" is given for FILE, or for GRAMMAR FILE given to --super, then it is read from standard input.");
            Console.WriteLine();
        }

    }
}
