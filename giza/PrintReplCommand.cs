
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
using System.Linq;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class PrintReplCommand : ReplCommand
    {
        public PrintReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "print";
            Description = "Print definitions as text in giza grammar format";
            HelpText = "All definitions specified will be printed. If the " +
                "command is given without definitions, then all currently " +
                "defined definitions will be printed.";
            Params = new [] {
                new Parameter {
                    Name="def-names",
                    ParameterType=ParameterType.StringArray,
                    Description="The definitions to print",
                    IsOptional=true,
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defnames = (args["def-names"] as string[]).ToList();

            var dr = new DefinitionRenderer();
            int? width = Console.WindowWidth;
            if (width < 20)
                width = null;
            if (defnames.Count < 1)
            {
                defnames = Env.Keys.ToList();
            }
            defnames.Sort();
            var defs = defnames.Where(name => Env.ContainsKey(name)).Select(name => Env[name]);
            bool first = true;
            foreach (var name in defnames.Where(name => !Env.ContainsKey(name)))
            {
                if (first)
                {
                    Console.WriteLine();
                }
                first = false;
                Console.WriteLine("Error: There is no definition named \"{0}\".", name);
            }
            Console.Write(dr.RenderDefinitionExprsAsGrammarText(defs, width));
        }
    }
}

