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
        public LoadReplCommand(Dictionary<string, DefinitionExpression> env)
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

            if (!File.Exists(filename))
            {
                Console.WriteLine("Can't find the file \"{0}\"", filename);
                return;
            }

            try
            {
                string contents = File.ReadAllText(filename);
                var errors = new List<Error>();
                var spanner = new SupergrammarSpanner();
                var defs = spanner.GetExpressions(contents, errors);
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
            catch (Exception ex)
            {
                Console.WriteLine("There was an internal error:");
                Console.WriteLine(ex);
            }
        }
    }
}

