﻿
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
    public class CheckCommand : Command
    {
        public CheckCommand()
        {
            Name = "check";
            Description = "Check a grammar for errors.";
            this.Params = new [] {
                new Parameter {
                    Name = "grammar-filename",
                    ParameterType = ParameterType.String,
                    Description = "The path to the file containing the grammar to check, or '-' for STDIN"
                },
            };
            this.Options = new [] {
                new Option {
                    Name = "tokenized",
                    Description="Treat the definitions as tokenized definitions",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];

            var grammarFilename = (string)args["grammar-filename"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Check(grammar, tokenized, grammarFilename);
        }

        public static void Check(string grammar, bool tokenized, string source)
        {
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var g = sgs.GetGrammar(grammar, errors, source);

            if (!errors.ContainsNonWarnings())
            {
                List<Error> errors2;

                var ec = new ExpressionChecker();
                if (tokenized)
                {
                    errors2 = ec.CheckDefinitionForParsing(g.Definitions);
                }
                else
                {
                    errors2 = ec.CheckDefinitions(g.Definitions);
                }

                errors.AddRange(errors2);
            }

            if (!errors.ContainsNonWarnings())
            {
                if (tokenized)
                {
                    var tt = new TokenizeTransform();
                    var g2 = tt.Tokenize(g);
                    var gc = new GrammarCompiler();
                    var ng = gc.Compile(g2);
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(ng.Definitions);
                    errors.AddRange(errors2);
                }
                else
                {
                    var gc = new GrammarCompiler();
                    var ng = gc.Compile(g.Definitions);
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(ng.Definitions);
                    errors.AddRange(errors2);
                }
            }

            errors.PrintErrors();

            if (!errors.ContainsNonWarnings())
            {
                NGrammar ng;
                var g2 = g;
                if (tokenized)
                {
                    var tt = new TokenizeTransform();
                    g2 = tt.Tokenize(g);
                }
                var gc = new GrammarCompiler();
                ng = gc.Compile(g2.Definitions);

                Console.WriteLine(
                    "There are {0} definitions in the grammar:",
                    ng.Definitions.Count());
                foreach (var def in ng.Definitions)
                {
                    Console.WriteLine("  {0}", def.Name);
                }
            }
        }
    }
}
