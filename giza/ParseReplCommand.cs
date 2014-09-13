using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class ParseReplCommand : ReplCommand
    {
        public ParseReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "parse";
            Description = "Parse one or more inputs with a tokenized grammar, " +
                "starting with a given definition, and print how many valid " +
                "parse trees are found";
            HelpText = "Any arguments after the start-definition are treated " +
                "as inputs to be parsed. Each input is parsed separately. If " +
                "no argument is supplied as input and the --from-file option " +
                "is not specified, then input is taken from STDIN.";
            Params = new [] {
                new Parameter {
                    Name="start-def",
                    ParameterType=ParameterType.String,
                    Description="The name of the top definition in the parse tree",
                },
                new Parameter {
                    Name="input",
                    ParameterType=ParameterType.StringArray,
                    IsOptional=true,
                    Description="Input strings to parse",
                },
            };
            Options = new [] {
                new Option {
                    Name="from-file",
                    Type=ParameterType.String,
                    Description="Use the given file for input",
                },
                new Option {
                    Name="verbose",
                    Description="Also print out the parse tree, if only one valid parse is found",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var startDef = (string)args["start-def"];
            var inputs = (string[])args["input"];
            var fromFile = (string)args["from-file"];
            var verbose = (bool)args["verbose"];

            if (!Env.ContainsKey(startDef))
            {
                Console.WriteLine("Error: There is no definition named \"{0}\".", startDef);
                return;
            }

            if (!string.IsNullOrEmpty(fromFile))
            {
                var contents = File.ReadAllText(fromFile);
                if (inputs == null)
                {
                    inputs = new [] { contents };
                }
                else
                {
                    var temp = new List<string>();
                    temp.AddRange(inputs);
                    temp.Add(contents);
                    inputs = temp.ToArray();
                }
            }

            if (inputs == null || inputs.Length < 1)
            {
                inputs = new [] { Program.ReadTextFromConsole() };
            }

            var tgb = new TokenizedGrammarBuilder();
            var grammar = tgb.BuildTokenizedGrammar(Env.Values.ToArray());
            var startDefinition = grammar.FindDefinitionByName(startDef);

            foreach (var input in inputs)
            {
                ParseCommand.Parse(input, startDefinition, verbose);
            }
        }
    }
}

