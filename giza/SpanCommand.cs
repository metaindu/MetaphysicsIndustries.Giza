
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
    public class SpanCommand : Command
    {
        public SpanCommand()
        {
            Name = "span";
            Description = "Process the input file with a non-tokenized grammar, starting with a given definition, and print how many valid matches are found";
            Params = new [] {
                new Parameter {
                    Name="grammar-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the grammar to use for spanning, or '-' for STDIN",
                },
                new Parameter {
                    Name="start-def",
                    ParameterType=ParameterType.String,
                    Description="The name of the top definition in the span tree",
                },
                new Parameter {
                    Name="input-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to a file to use for input to be spanned, or '-' for STDIN",
                },
            };
            Options = new [] {
                new Option { 
                    Name="verbose",
                    Description="Also print out the span tree, if only one valid parse is found",
                },
                new Option {
                    Name="show-all",
                    Description="Print out all parse trees, even if more than one valid parse is found",
                },
            };
        }
        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var grammarFilename = (string)args["grammar-filename"];
            var startDef = (string)args["start-def"];
            var inputFilename = (string)args["input-filename"];
            var verbose = (bool)args["verbose"];
            var showAll = (bool)args["show-all"];

            var printingOptions = SpanPrintingOptionsHelper.FromBools(verbose, showAll);

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

            Span(grammar, input, startDef, printingOptions, grammarFilename);
        }

        public static void Span(string grammar, string input, string startDef,
            SpanPrintingOptions printingOptions, string source)
        {
            var spanner = new SupergrammarSpanner();
            var errors = new List<Error>();
            var g = spanner.GetGrammar(grammar, errors, source);

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
            errors = ec.CheckDefinitions(g.Definitions);

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

            var gc = new GrammarCompiler();
            var ng = gc.Compile(g.Definitions);
            var startDefinition = ng.Definitions.First(d => d.Name == startDef);
            Span(input, startDefinition, printingOptions);
        }
        public static void Span(string input, NDefinition startDefinition, SpanPrintingOptions printingOptions)
        {
            var dc = new DefinitionChecker();
            var errors = new List<Error>();
            dc.CheckDefinitions(startDefinition.ParentGrammar.Definitions, errors);

            Span[] ss = null;
            if (!errors.ContainsNonWarnings())
            {
                var gs = new Spanner(startDefinition);
                var errors2 = new List<Error>();
                ss = gs.Process(input.ToCharacterSource(), errors2);
                errors.AddRange(errors2);
            }

            errors.PrintErrors(context: " in the grammar");

            if (errors.ContainsNonWarnings())
            {
                return;
            }

            if (ss.Length < 1)
            {
                Console.WriteLine("There are no valid spans.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("There are {0} valid spans.", ss.Length);
                if (printingOptions == SpanPrintingOptions.All)
                {
                    int k = 0;
                    foreach (var s in ss)
                    {
                        k++;
                        Console.WriteLine("=== Span {0} ===========", k);
                        ParseCommand.PrintSpanHierarchy(s);
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("There is 1 valid span of the input.");
                if (printingOptions == SpanPrintingOptions.All ||
                    printingOptions == SpanPrintingOptions.One)
                {
                    ParseCommand.PrintSpanHierarchy(ss[0]);
                }
            }
        }
    }
}

