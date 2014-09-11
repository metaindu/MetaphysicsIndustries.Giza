using System;
using NCommander;
using System.Collections.Generic;
using System.IO;

namespace giza
{
    public class RenderCommand : Command
    {
        public RenderCommand()
        {
            Name = "render";
            Description = "Process the grammar file and print its definitions as a C# class.";
            Params = new Parameter[] {
                new Parameter { Name="grammarFilename", ParameterType=ParameterType.String },
                new Parameter { Name="className", ParameterType=ParameterType.String },
            };
            Options = new NCommander.Option[] {
                new NCommander.Option { Name="tokenized" },
                new NCommander.Option { Name="singleton" },
                new NCommander.Option { Name="namespace", Type=ParameterType.String },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];
            var ns = (string)args["namespace"] ?? "MetaphysicsIndustries.Giza";
            var singleton = (bool)args["singleton"];

            var grammarFilename = (string)args["grammarFilename"];
            var className = (string)args["className"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Program.Render(tokenized, ns, singleton, grammar, className);
        }
    }
}

