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
    public class ParseCommand : Command
    {
        public ParseCommand()
        {
            Name = "parse";
            Description = "Parse the input file with a tokenized grammar, " +
                "starting with a given definition, and print how many valid " +
                "parse trees are found.";
            Params = new [] {
                new Parameter {
                    Name="grammar-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the grammar to use for parsing, or '-' for STDIN",
                },
                new Parameter {
                    Name="start-def",
                    ParameterType=ParameterType.String,
                    Description="The name of the top definition in the parse tree",
                },
                new Parameter {
                    Name="input-filename",
                    ParameterType=ParameterType.String,
                    Description="The path to a file to use for input to be parsed, or '-' for STDIN",
                },
            };
            Options = new [] {
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

            Parse(grammar, input, startDef, printingOptions, grammarFilename);
        }

        public static void Parse(string grammar, string input, string startDef,
            SpanPrintingOptions printingOptions, string source)
        {
            var spanner = new SupergrammarSpanner();
            var grammarErrors = new List<Error>();
            var g = spanner.GetGrammar(grammar, grammarErrors, source);

            if (!grammarErrors.ContainsNonWarnings())
            {
                var ec = new ExpressionChecker();
                var errors2 = ec.CheckDefinitionForParsing(g.Definitions);
                grammarErrors.AddRange(errors2);
            }

            NGrammar ng = null;
            if (!grammarErrors.ContainsNonWarnings())
            {
                var tt = new TokenizeTransform();
                var g2 = tt.Tokenize(g);
                var gc = new GrammarCompiler();
                ng = gc.Compile(g2);

                var dc = new DefinitionChecker();
                var errors2 = dc.CheckDefinitions(ng.Definitions);
                grammarErrors.AddRange(errors2);
            }

            grammarErrors.PrintErrors(context: " in the grammar");

            if (grammarErrors.ContainsNonWarnings())
            {
                return;
            }

            var startDefinition = ng.FindDefinitionByName(startDef);
            if (startDefinition == null)
            {
                Console.WriteLine("Error: There is no defintion named \"{0}\".", startDef);
                return;
            }

            Parse(input, startDefinition, printingOptions);
        }
        public static void Parse(string input, NDefinition startDefinition, SpanPrintingOptions spanPrintingOption=SpanPrintingOptions.None)
        {
            var parser = new Parser(startDefinition);
            var inputErrors = new List<Error>();
            Span[] ss = parser.Parse(input.ToCharacterSource(), inputErrors);

            inputErrors.PrintErrors(context: " in the input");

            if (inputErrors.ContainsNonWarnings())
            {
                return;
            }

            if (ss.Length < 1)
            {
                Console.WriteLine("There are no valid parses of the input.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("There are {0} valid parses of the input.", ss.Length);
                if (spanPrintingOption == SpanPrintingOptions.All)
                {
                    int k = 0;
                    foreach (var s in ss)
                    {
                        k++;
                        Console.WriteLine("=== Parse {0} ===========", k);
                        PrintSpanHierarchy(s);
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("There is 1 valid parse of the input.");
                if (spanPrintingOption == SpanPrintingOptions.All ||
                    spanPrintingOption == SpanPrintingOptions.One)
                {
                    PrintSpanHierarchy(ss[0]);
                }
            }
        }

        public static void PrintSpanHierarchy(Span s)
        {
            var items = new List<Tuple<string, string>>();
            GatherSpanHierarchy(s, items);

            var width = items.Max(i => i.Item2.Length);

            var format = "{0," + (-width - 4) + "} {1}";
            foreach (var i in items)
            {
                Console.WriteLine(format, i.Item2, i.Item1);
            }
        }

        static void GatherSpanHierarchy(Span s, List<Tuple<string,string>> items, string indent="")
        {
            string label;
            if (s.Node is CharNode)
            {
                label = (s.Node as CharNode).CharClass.ToString();
            }
            else
            {
                label = (s.Node as DefRefNode).DefRef.Name;
            }

            string value;
            if (s.Subspans.Any())
            {
                value = "";
            }
            else
            {
                value = s.Value ?? "";
            }

            items.Add(new Tuple<string, string>(indent + label, value));

            foreach (var sub in s.Subspans)
            {
                GatherSpanHierarchy(sub, items, indent + "  ");
            }
        }
    }

}

