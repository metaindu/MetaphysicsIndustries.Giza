
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
using System.Collections.Generic;
using System.IO;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class RenderCommand : Command
    {
        public RenderCommand()
        {
            Name = "render";
            Description = "Process the grammar file and print its definitions as a C# class.";
//            HelpText = "";
            Params = new [] {
                new Parameter {
                    Name="class-name",
                    ParameterType=ParameterType.String,
                    Description="The name of the C# class to generate",
                },
                new Parameter {
                    Name="grammar-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the grammar to render, or '-' for STDIN",
                },
            };
            Options = new [] {
                new Option {
                    Name="tokenized",
                    Description="The grammar is tokenized",
                },
                new Option {
                    Name="singleton",
                    Description="Add a single static readonly field to the class, as a default instance",
                },
                new Option {
                    Name="namespace",
                    Type=ParameterType.String,
                    Description="The namespace in which the C# class is defined (default is 'MetaphysicsIndustries.Giza')",
                },
                // TODO: add the new options from RenderReplCommand
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];
            var ns = (string)args["namespace"] ?? "MetaphysicsIndustries.Giza";
            var singleton = (bool)args["singleton"];

            var grammarFilename = (string)args["grammar-filename"];
            var className = (string)args["class-name"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Render(tokenized, ns, singleton, grammar, className);
        }

        public static void Render(bool tokenized, string ns, bool isSingleton, string grammar, string className)
        {

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var g = sgs.GetGrammar(grammar, errors);

            if (errors.Count > 0)
            {
                Console.WriteLine("There were errors in the grammar:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            if (tokenized)
            {
                errors = ec.CheckDefinitionForParsing(g.Definitions);
            }
            else
            {
                errors = ec.CheckDefinitions(g.Definitions);
            }

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

            NGrammar ng;

            // TODO: deduplicate
            if (tokenized)
            {
                var tgb = new TokenizeTransform();
                var g2 = tgb.Tokenize(g);
                var gc = new GrammarCompiler();
                ng = gc.Compile(g2);
            }
            else
            {
                var gc = new GrammarCompiler();
                ng = gc.Compile(g.Definitions);
            }

            var dr = new DefinitionRenderer();
            Console.Write(dr.RenderDefinitionsAsCSharpClass(className, ng.Definitions, ns: ns, singleton: isSingleton));
        }
    }
}

