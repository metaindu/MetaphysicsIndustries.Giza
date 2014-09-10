﻿using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class SpanReplCommand : ReplCommand
    {
        public SpanReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "span";
            Params = new [] {
                new Parameter { Name="start-definition", ParameterType=ParameterType.String },
                new Parameter { Name="input", ParameterType=ParameterType.StringArray, IsOptional=true },
            };
            Options = new [] {
                new NCommander.Option { Name="from-file", Type=ParameterType.String },
                new NCommander.Option { Name="verbose" },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var startDef = (string)args["start-definition"];
            var inputs = (string[])args["input"];
            var fromFile = (string)args["from-file"];
            var verbose = (bool)args["verbose"];

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

            var db = new DefinitionBuilder();
            var grammar = new Grammar(db.BuildDefinitions(Env.Values.ToArray()));
            var startDefinition = grammar.FindDefinitionByName(startDef);

            foreach (var input in inputs)
            {
                Program.Span(input, startDefinition, verbose);
            }
        }
    }
}
