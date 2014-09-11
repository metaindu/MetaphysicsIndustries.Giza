using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;

namespace giza
{
    public class CheckCommand : Command
    {
        public CheckCommand()
        {
            Name = "check";
            Description = "Check a grammar for errors.";
            this.Options = new NCommander.Option[] {
                new NCommander.Option { Name = "tokenized" },
            };
            this.Params = new Parameter[] {
                new Parameter { Name = "grammarFilename", ParameterType = ParameterType.String },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];

            var grammarFilename = (string)args["grammarFilename"];

            string grammar;
            if (grammarFilename == "-")
            {
                grammar = Program.ReadTextFromConsole();
            }
            else
            {
                grammar = File.ReadAllText(grammarFilename);
            }

            Check(grammar, tokenized);
        }

        public static void Check(string grammar, bool tokenized)
        {
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(grammar, errors);

            if (!errors.ContainsNonWarnings())
            {
                List<Error> errors2;

                var ec = new ExpressionChecker();
                if (tokenized)
                {
                    errors2 = ec.CheckDefinitionInfosForParsing(dis);
                }
                else
                {
                    errors2 = ec.CheckDefinitionInfos(dis);
                }

                errors.AddRange(errors2);
            }

            if (errors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the grammar:");
            }
            else if (errors.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the grammar:");
            }

            foreach (var error in errors)
            {
                Console.Write("  ");
                Console.WriteLine(error.Description);
            }

            if (!errors.ContainsNonWarnings())
            {
                IEnumerable<Definition> defs;
                if (tokenized)
                {
                    TokenizedGrammarBuilder tgb = new TokenizedGrammarBuilder();
                    var g = tgb.BuildTokenizedGrammar(dis);
                    defs = g.Definitions;
                }
                else
                {
                    DefinitionBuilder db = new DefinitionBuilder();
                    defs = db.BuildDefinitions(dis);
                }

                Console.WriteLine("There are {0} definitions in the grammar:", defs.Count());
                foreach (var def in defs)
                {
                    Console.WriteLine("  {0}", def.Name);
                }
            }
        }


    }

}

