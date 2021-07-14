
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
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class CheckReplCommand : ReplCommand
    {
        public CheckReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "check";
            Description = "Check definitions for errors";
            Params = new [] {
                new Parameter {
                    Name="def-names",
                    ParameterType=ParameterType.StringArray,
                    Description="Names of definitions to check; if no definitions are specified, then all definitions will be checked",
                    IsOptional=true,
                },
            };
            Options = new [] {
                new Option {
                    Name="tokenized",
                    Description="Treat the definitions as tokenized definitions",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defnames = (string[])args["def-names"];
            var tokenized = (bool)args["tokenized"];
            if (defnames.Length < 1)
            {
                defnames = Env.Keys.ToArray();
            }
            else
            {
                bool someAreMissing = false;
                foreach (var name in defnames)
                {
                    if (!Env.ContainsKey(name))
                    {
                        Console.WriteLine("Error: There is no definition named \"{0}\".", name);
                        someAreMissing = true;
                    }
                }
                if (someAreMissing) return;
            }

            var ec = new ExpressionChecker();
            var defs = defnames.Select(name => Env[name]);
            List<Error> errors;
            if (tokenized)
            {
                errors = ec.CheckDefinitionForParsing(defs);
            }
            else
            {
                errors = ec.CheckDefinitionsForSpanning(defs);
            }

            if (!errors.ContainsNonWarnings())
            {
                if (tokenized)
                {
                    var tgb = new TokenizeTransform();
                    var pg = new PreGrammar()
                    {
                        Definitions = defs.ToList()
                    };
                    var pg2 = tgb.Tokenize(pg);
                    var gc = new GrammarCompiler();
                    var g = gc.BuildGrammar(pg2);
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(g.Definitions);
                    errors.AddRange(errors2);
                }
                else
                {
                    var gc = new GrammarCompiler();
                    var g = gc.BuildGrammar(defs.ToArray());
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(g.Definitions);
                    errors.AddRange(errors2);
                }
            }

            errors.PrintErrors(true);
        }
    }
}

