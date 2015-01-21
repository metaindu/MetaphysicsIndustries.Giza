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
        public SaveReplCommand(Dictionary<string, DefinitionExpression> env)
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

        public static DefinitionExpression[] GetAllReferencedDefinitions(IEnumerable<DefinitionExpression> defs, Dictionary<string, DefinitionExpression> env, ref bool someAreMissing)
        {
            var next = new HashSet<DefinitionExpression>();
            var prev = new HashSet<DefinitionExpression>(defs);
            var alldefs = new HashSet<DefinitionExpression>(defs);

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

