
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

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
                new Option {
                    Name="show-all",
                    Description="Print out all parse trees, even if more than one valid parse is found",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var startDef = (string)args["start-def"];
            var inputs = (string[])args["input"];
            var fromFile = (string)args["from-file"];
            var verbose = (bool)args["verbose"];
            var showAll = (bool)args["show-all"];

            var printingOptions = SpanPrintingOptionsHelper.FromBools(verbose, showAll);

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

            var ec = new ExpressionChecker();
            var errors = ec.CheckDefinitionForParsing(Env.Values);

            Grammar grammar = null;
            if (!errors.ContainsNonWarnings())
            {
                var tgb = new TokenizedGrammarBuilder();
                var pg = new PreGrammar() {Defintions = Env.Values.ToList()};
                grammar = tgb.BuildTokenizedGrammar(pg);
                var dc = new DefinitionChecker();
                var errors2 = dc.CheckDefinitions(grammar.Definitions);
                errors.AddRange(errors2);
            }

            errors.PrintErrors(context: " in the grammar");

            if (errors.ContainsNonWarnings())
            {
                return;
            }

            if (inputs == null || inputs.Length < 1)
            {
                inputs = new [] { Program.ReadTextFromConsole() };
            }

            var startDefinition = grammar.FindDefinitionByName(startDef);

            foreach (var input in inputs)
            {
                ParseCommand.Parse(input, startDefinition, printingOptions);
            }
        }
    }
}

