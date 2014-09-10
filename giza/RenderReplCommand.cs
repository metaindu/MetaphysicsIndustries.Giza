using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using NCommander;
using System.IO;
using System.Linq;

namespace giza
{
    public class RenderReplCommand : ReplCommand
    {
        public RenderReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "render";
            Description = "Process the grammar file and print its definitions as a C# class.";
            Params = new [] {
                new Parameter { Name="className", ParameterType=ParameterType.String },
                new Parameter { Name="defNames", ParameterType=ParameterType.StringArray },
            };
            Options = new [] {
                new Option { Name="tokenized" },
                new Option { Name="singleton" },
                new Option { Name="namespace", Type=ParameterType.String },
                new Option { Name="to-file", Type=ParameterType.String },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];
            var ns = (string)args["namespace"] ?? "MetaphysicsIndustries.Giza";
            var singleton = (bool)args["singleton"];
            var toFile = (string)args["to-file"];

            var className = (string)args["className"];
            var defnames = (string[])args["defNames"];

            if (defnames == null || defnames.Length < 1)
            {
                throw new ArgumentException("No definitions specified", "defNames");
            }

            var someAreMissing = false;
            foreach (var name in defnames)
            {
                if (!Env.ContainsKey(name))
                {
                    Console.WriteLine("There is no definition named \"{0}\".", name);
                    someAreMissing = true;
                }
            }

            if (someAreMissing) return;

            var defs = defnames.Select(name => Env[name]).ToArray();
            var alldefs = SaveReplCommand.GetAllReferencedDefinitions(defs, Env, ref someAreMissing);

            Grammar g;
            if (tokenized)
            {
                var tgb = new TokenizedGrammarBuilder();
                g = tgb.BuildTokenizedGrammar(alldefs);
            }
            else
            {
                var db = new DefinitionBuilder();
                g = new Grammar(db.BuildDefinitions(alldefs));
            }

            var dr = new DefinitionRenderer();
            var cs = dr.RenderDefinitionsAsCSharpClass(className, g.Definitions, ns, singleton);

            if (!string.IsNullOrEmpty(toFile))
            {
                File.WriteAllText(toFile, cs);
            }
            else
            {
                Console.WriteLine(cs);
            }
        }
    }
}

