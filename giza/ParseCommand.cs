using System;
using NCommander;
using System.IO;
using System.Collections.Generic;

namespace giza
{
    public class ParseCommand : Command
    {
        public ParseCommand()
        {
            Name = "parse";
            Description = "Parse the input file with a tokenized grammar, starting with a given symbol.";
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
                grammar = Console.In.ReadToEnd();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            string input;
            if (inputFilename == "-")
            {
                input = Console.In.ReadToEnd();
            }
            else
            {
                try
                {
                    input = File.ReadAllText(inputFilename);
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error while trying to open the input file:");
                    Console.WriteLine("  {0}", e.Message);
                    if (verbose)
                    {
                        Console.WriteLine("  {0}", e.ToString());
                    }
                    return;
                }
            }

            Program.Parse(verbose, grammar, input, startSymbol);
        }
    }

}

