using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class CheckReplCommand : ReplCommand
    {
        public CheckReplCommand(Dictionary<string, DefinitionExpression> env)
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
                errors = ec.CheckDefinitionInfosForParsing(defs);
            }
            else
            {
                errors = ec.CheckDefinitionInfosForSpanning(defs);
            }

            if (errors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors:");
            }
            else if (errors.Count > 0)
            {
                Console.WriteLine("There are warnings:");
            }
            else
            {
                Console.WriteLine("There are no errors or warnings.");
            }

            foreach (var error in errors)
            {
                Console.WriteLine(error.Description);
            }
        }
    }
}

