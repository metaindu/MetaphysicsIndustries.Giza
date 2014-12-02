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
        public PrintReplCommand(Dictionary<string, DefinitionExpression> env)
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

