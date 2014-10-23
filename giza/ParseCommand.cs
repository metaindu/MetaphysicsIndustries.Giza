using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class ParseCommand : Command
    {
        public ParseCommand()
        {
            Name = "parse";
            Description = "Parse the input file with a tokenized grammar, " +
                "starting with a given definition, and print how many valid " +
                "parse trees are found.";
            Params = new [] {
                new Parameter {
                    Name="grammar-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the grammar to use for parsing, or '-' for STDIN",
                },
                new Parameter {
                    Name="start-def",
                    ParameterType=ParameterType.String,
                    Description="The name of the top definition in the parse tree",
                },
                new Parameter {
                    Name="input-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to a file to use for input to be parsed, or '-' for STDIN",
                },
            };
            Options = new [] {
                new Option {
                    Name="verbose",
                    Description="Also print out the parse tree, if only one valid parse is found",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var grammarFilename = (string)args["grammar-filename"];
            var startDef = (string)args["start-def"];
            var inputFilename = (string)args["input-filename"];
            var verbose = (bool)args["verbose"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            string input;
            if (inputFilename == "-")
            {
                input = Program.ReadTextFromConsole();
            }
            else
            {
                input = File.ReadAllText(inputFilename);
            }

            Parse(verbose, grammar, input, startDef);
        }

        public static void Parse(bool verbose, string grammar, string input, string startDef)
        {
            var spanner = new SupergrammarSpanner();
            var grammarErrors = new List<Error>();
            var dis = spanner.GetExpressions(grammar, grammarErrors);

            if (!grammarErrors.ContainsNonWarnings())
            {
                var ec = new ExpressionChecker();
                var errors2 = ec.CheckDefinitionInfosForParsing(dis);
                grammarErrors.AddRange(errors2);
            }

            if (grammarErrors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the grammar:");
            }
            else if (grammarErrors.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the grammar:");
            }

            foreach (var err in grammarErrors)
            {
                Console.WriteLine("  {0}", err.Description);
            }

            if (grammarErrors.ContainsNonWarnings())
            {
                return;
            }

            var tgb = new TokenizedGrammarBuilder();
            var g = tgb.BuildTokenizedGrammar(dis);

            var startDefinition = g.FindDefinitionByName(startDef);
            if (startDefinition == null)
            {
                Console.WriteLine("Error: There is no defintion named \"{0}\".", startDef);
                return;
            }

            Parse(input, startDefinition, verbose);
        }
        public static void Parse(string input, Definition startDefinition, bool verbose)
        {
            var parser = new Parser(startDefinition);
            var inputErrors = new List<Error>();
            Span[] ss = parser.Parse(input.ToCharacterSource(), inputErrors);

            if (inputErrors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the input:");
            }
            else if (inputErrors.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the input:");
            }

            foreach (var err in inputErrors)
            {
                Console.WriteLine("  {0}", err.Description);
            }

            if (inputErrors.ContainsNonWarnings())
            {
                return;
            }

            if (ss.Length < 1)
            {
                Console.WriteLine("There are no valid parses of the input.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("There are {0} valid parses of the input.", ss.Length);
                //foreach (Span s in ss)
                //{
                //    Console.WriteLine(s.RenderSpanHierarchy());
                //}
            }
            else
            {
                Console.WriteLine("There is 1 valid parse of the input.");
                if (verbose)
                {
                    PrintSpanHierarchy(ss[0]);
                }
            }
        }

        static void PrintSpanHierarchy(Span s)
        {
            const string indent = "  ";

            var items = new List<Tuple<string, string>>();
            GatherSpanHierarchy(s, items);

            var width = items.Max(i => i.Item1.Length);

            var format = "{0," + (-width - 4) + "} {1}";
            foreach (var i in items)
            {
                Console.WriteLine(format, i.Item1, i.Item2);
            }
        }

        static void GatherSpanHierarchy(Span s, List<Tuple<string,string>> items, string indent="")
        {
            string label;
            if (s.Node is CharNode)
            {
                label = (s.Node as CharNode).CharClass.ToString();
            }
            else
            {
                label = (s.Node as DefRefNode).DefRef.Name;
            }

            string value;
            if (s.Subspans.Any())
            {
                value = "";
            }
            else
            {
                value = s.Value ?? "";
            }

            items.Add(new Tuple<string, string>(indent + label, value));

            foreach (var sub in s.Subspans)
            {
                GatherSpanHierarchy(sub, items, indent + "  ");
            }
        }
    }

}

