using System;
using NCommander;
using System.IO;
using System.Collections.Generic;

namespace giza
{
    public class CheckCommand : Command
    {
        public CheckCommand()
        {
            Name = "check";
            Description = "Check a grammar for errors.";
            this.Options = new NCommander.Option[] {
                new NCommander.Option { Name = "tokenized" },
            };
            this.Params = new Parameter[] {
                new Parameter { Name = "grammarFilename", ParameterType = ParameterType.String },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];

            var grammarFilename = (string)args["grammarFilename"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Program.Check(grammar, tokenized);
        }
    }

}

