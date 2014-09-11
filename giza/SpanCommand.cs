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
            Description = "Process the input file with a non-tokenized grammar, starting with a given symbol.";
            Params = new Parameter[] {
                new Parameter { Name="grammarFilename", ParameterType=ParameterType.String },
                new Parameter { Name="startSymbol", ParameterType=ParameterType.String },
                new Parameter { Name="inputFilename", ParameterType=ParameterType.String },
            };
            Options = new NCommander.Option[] {
                new NCommander.Option { Name="verbose" },
            };
        }
        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var grammarFilename = (string)args["grammarFilename"];
            var startSymbol = (string)args["startSymbol"];
            var inputFilename = (string)args["inputFilename"];
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

            Span(verbose, grammar, input, startSymbol);
        }

        public static void Span(bool verbose, string grammar, string input, string startSymbol)
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

            var startDefinition = defs.First(d => d.Name == startSymbol);
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

