using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class SpanCommand : Command
    {
        public SpanCommand()
        {
            Name = "span";
            Description = "Process the input file with a non-tokenized grammar, starting with a given definition, and print how many valid matches are found";
            Params = new [] {
                new Parameter {
                    Name="grammar-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the grammar to use for spanning, or '-' for STDIN",
                },
                new Parameter {
                    Name="start-def",
                    ParameterType=ParameterType.String,
                    Description="The name of the top definition in the span tree",
                },
                new Parameter {
                    Name="input-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to a file to use for input to be spanned, or '-' for STDIN",
                },
            };
            Options = new [] {
                new Option { 
                    Name="verbose",
                    Description="Also print out the span tree, if only one valid parse is found",
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

            Span(verbose, grammar, input, startDef);
        }

        public static void Span(bool verbose, string grammar, string input, string startDef)
        {
            var spanner = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = spanner.GetExpressions(grammar, errors);

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

            var startDefinition = defs.First(d => d.Name == startDef);
            var g = new Grammar(defs);
            Span(input, startDefinition, verbose);
        }
        public static void Span(string input, Definition startDefinition, bool verbose)
        {
            Spanner gs = new Spanner(startDefinition);
            var errors = new List<Error>();
            Span[] ss = gs.Process(input.ToCharacterSource(), errors);
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
            }
            else
            {
                Console.WriteLine("1 valid span.");
                ParseCommand.PrintSpanHierarchy(ss[0]);
            }
        }


    }
}

