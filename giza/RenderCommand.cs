using System;
using NCommander;
using System.Collections.Generic;
using System.IO;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class RenderCommand : Command
    {
        public RenderCommand()
        {
            Name = "render";
            Description = "Process the grammar file and print its definitions as a C# class.";
            Params = new Parameter[] {
                new Parameter { Name="grammarFilename", ParameterType=ParameterType.String },
                new Parameter { Name="className", ParameterType=ParameterType.String },
            };
            Options = new NCommander.Option[] {
                new NCommander.Option { Name="tokenized" },
                new NCommander.Option { Name="singleton" },
                new NCommander.Option { Name="namespace", Type=ParameterType.String },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];
            var ns = (string)args["namespace"] ?? "MetaphysicsIndustries.Giza";
            var singleton = (bool)args["singleton"];

            var grammarFilename = (string)args["grammarFilename"];
            var className = (string)args["className"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Render(tokenized, ns, singleton, grammar, className);
        }

        public static void Render(bool tokenized, string ns, bool isSingleton, string grammar, string className)
        {

            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(grammar, errors);

            if (errors.Count > 0)
            {
                Console.WriteLine("There were errors in the grammar:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Description);
                }
                return;
            }

            var ec = new ExpressionChecker();
            if (tokenized)
            {
                errors = ec.CheckDefinitionInfosForParsing(dis);
            }
            else
            {
                errors = ec.CheckDefinitionInfos(dis);
            }

            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the grammar:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }

            Grammar g;

            if (tokenized)
            {
                var tgb = new TokenizedGrammarBuilder();
                g = tgb.BuildTokenizedGrammar(dis);
            }
            else
            {
                var db = new DefinitionBuilder();
                var defs = db.BuildDefinitions(dis);

                g = new Grammar();
                g.Definitions.AddRange(defs);
            }

            var dr = new DefinitionRenderer();
            Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions, ns: ns, singleton: isSingleton));
        }


    }
}

