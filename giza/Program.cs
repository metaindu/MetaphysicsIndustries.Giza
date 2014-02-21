using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;
using MetaphysicsIndustries.Build;

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
                    string gfile = File.ReadAllText(args[1]);
                    Span g = s.Getgrammar(gfile);
                    PrintSpan(g);
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
                Span g = spanner.Getgrammar(grammarFile);

                DefinitionBuilder db = new DefinitionBuilder();
                SimpleDefinitionNode[] defs = db.BuildDefinitions(g);

                string input = File.ReadAllText(args[2]);

                GenericSpanner gs = new GenericSpanner();
                Span s = gs.Process(defs, args[1], input);

                if (s != null)
                {
                    PrintSpan(s);
                }
                else if (gs.ErrorContext != null)
                {
                    int errorLine = CountLines(input, gs.ErrorCharacter);
                    int errorChar = CountCharactersOnLine(input, gs.ErrorCharacter);
                    Console.Write("{0}({1},{2}): Expected ", args[2], errorLine, errorChar);
                    if (gs.ErrorContext.NextNodes.Count > 1)
                    {
                        Console.Write("one of ('");

                        bool first = true;
                        foreach (Node next in gs.ErrorContext.NextNodes)
                        {
                            Console.Write(next.Tag);
                            if (first) first = false;
                            else Console.Write("', '");
                        }

                        Console.WriteLine("')");
                    }
                    else
                    {
                        Console.Write("'");
                        Console.Write(gs.ErrorContext.NextNodes.GetFirst().Tag);
                        Console.Write("'");
                        Console.WriteLine();
                    }
                }
                else 
                {
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

        static string BuildSpanJson(Span s)
        {
            return BuildSpanJson(s, "  ");
        }
        static string BuildSpanJson(Span s, string indent)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            if (indent.Length > 0) sb.AppendLine();

            BuildSpanJson(s, indent, sb);

            if (indent.Length > 0) sb.AppendLine();
            sb.Append("}");
            if (indent.Length > 0) sb.AppendLine();

            return sb.ToString();
        }
        static void BuildSpanJson(Span s, string indent, StringBuilder sb)
        {
            if (indent.Length > 0) sb.Append(indent);

            sb.Append(EscapeString(s.Tag));
            sb.Append(": ");

            if (s.Subspans != null && s.Subspans.Length > 0)
            {
                if (indent.Length > 0)
                    sb.AppendLine("{");
                else
                    sb.Append("{");

                bool first = true;
                foreach (Span ss in s.Subspans)
                {
                    if (first)
                        first = false;
                    else if (indent.Length > 0)
                        sb.AppendLine(",");
                    else
                        sb.Append(",");

                    BuildSpanJson(ss, indent + "  ", sb);
                }

                if (indent.Length > 0)
                {
                    sb.AppendLine();
                    sb.Append(indent);
                }

                sb.Append("}");
            }
            else
            {
                sb.Append(EscapeString(s.Value));
            }
        }

        private static void PrintSpan(Span s)
        {
            Console.Write(BuildSpanJson(s));
        }

        private static string EscapeString(string str)
        {
            return "\"" + str.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    giza [options]");
            Console.WriteLine("    giza [options] [GRAMMAR FILE] [START SYMBOL] [FILE]");
            Console.WriteLine("    giza --super [GRAMMAR FILE]");
            //Console.WriteLine("    giza --print-super");
            //Console.WriteLine("    giza --compile [GRAMMAR FILE] [START SYMBOL] [OUTPUT EXE]");
            Console.WriteLine();
            Console.WriteLine("Converts input into base64-encoded form.");
            Console.WriteLine();
            Console.WriteLine("    --version, -[vV]  Print version and exit successfully.");
            Console.WriteLine("    --help,           Print this help and exit successfully.");
            Console.WriteLine("    --super,          Process the grammar file only.");
            //Console.WriteLine("    --print-super,    Print the supergrammar and exit.");
            //Console.WriteLine("    --compile,      Print the supergrammar and exit.");
            Console.WriteLine();
        }

    }
}
