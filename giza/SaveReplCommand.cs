
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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
    public class SaveReplCommand : ReplCommand
    {
        public SaveReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "save";
            Description = "Save definitions to a file as text in giza grammar format";
            Params = new [] {
                new Parameter {
                    Name="filename",
                    ParameterType=ParameterType.String,
                    Description="The file to save the definitions to",
                },
                new Parameter {
                    Name="def-names",
                    ParameterType=ParameterType.StringArray,
                    Description="The definitions to save",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var filename = (string)args["filename"];
            var names = (string[])args["def-names"];

            if (names.Length < 1)
            {
                Console.WriteLine("Error: No definitions were specified to save.");
                return;
            }

            var someAreMissing = false;
            foreach (var name in names)
            {
                if (!Env.ContainsKey(name))
                {
                    Console.WriteLine("Error: There is no definition named \"{0}\".", name);
                    someAreMissing = true;
                }
            }

            if (someAreMissing) return;

            var defs = names.Select(name => Env[name]);
            var alldefs = GetAllReferencedDefinitions(defs, Env, ref someAreMissing);

            if (someAreMissing) return;

            var dr = new DefinitionRenderer();
            var fileContents = dr.RenderDefinitionExprsAsGrammarText(alldefs);
            string header = string.Format("// File saved at {0}", DateTime.Now);
            using (var f = new StreamWriter(filename))
            {
                f.WriteLine(header);
                f.WriteLine();
                f.Write(fileContents);
            }
            Console.WriteLine(header);
        }

        public static Definition[] GetAllReferencedDefinitions(IEnumerable<Definition> defs, Dictionary<string, Definition> env, ref bool someAreMissing)
        {
            var next = new HashSet<Definition>();
            var prev = new HashSet<Definition>(defs);
            var alldefs = new HashSet<Definition>(defs);

            while (prev.Count > 0)
            {
                next.Clear();
                foreach (var def in prev)
                {
                    foreach (var defref in def.EnumerateDefRefs())
                    {
                        if (env.ContainsKey(defref.DefinitionName))
                        {
                            next.Add(env[defref.DefinitionName]);
                        }
                        else
                        {
                            Console.WriteLine("Error: There is no definition named \"{0}\".", defref.DefinitionName);
                            someAreMissing = true;
                        }
                    }
                }
                next.ExceptWith(alldefs);
                alldefs.UnionWith(next);
                prev.Clear();
                prev.UnionWith(next);
            }

            return alldefs.ToArray();
        }
    }
}

