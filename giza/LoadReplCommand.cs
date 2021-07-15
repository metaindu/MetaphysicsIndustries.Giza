
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
    public class LoadReplCommand : ReplCommand
    {
        public LoadReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "load";
            Description = "Load definitions from a file";
            Params = new [] {
                new Parameter {
                    Name="filename",
                    ParameterType=ParameterType.String,
                    Description="The path to the file containing the definitions to load.",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var filename = (string)args["filename"];

            string contents = File.ReadAllText(filename);

            var errors = new List<Error>();
            var spanner = new SupergrammarSpanner();
            var g = spanner.GetGrammar(contents, errors, filename);
            var defs = g.Definitions;
            if (errors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the loaded file:");
                foreach (var error in errors.Where(e => !e.IsWarning))
                {
                    Console.WriteLine(error.Description);
                }
                return;
            }

            foreach (var def in defs)
            {
                if (Env.ContainsKey(def.Name))
                {
                    Console.WriteLine("{0} was replaced", def.Name);
                }
                else
                {
                    Console.WriteLine("{0} was added", def.Name);
                }

                Env[def.Name] = def;
            }
        }
    }
}

