using System;
using NCommander;
using System.IO;
using System.Collections.Generic;

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
                input = File.ReadAllText(inputFilename);
            }

            Program.Span(verbose, grammar, input, startSymbol);
        }
    }
}

