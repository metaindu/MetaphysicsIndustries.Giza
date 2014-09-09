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
            Description = "Print out each of the definitions specified, or all definitions if none are specified.";
            Params = new Parameter[] {
                new Parameter { Name="defnames", ParameterType=ParameterType.StringArray },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defnames = (args["defnames"] as string[]).ToList();

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
                Console.WriteLine("There is no definition named \"{0}\".", name);
            }
            Console.Write(dr.RenderDefinitionExprsAsGrammarText(defs, width));
        }
    }
}

