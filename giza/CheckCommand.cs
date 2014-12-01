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
            this.Params = new [] {
                new Parameter {
                    Name = "grammar-filename",
                    ParameterType = ParameterType.String,
                    Description = "The path to the file containing the grammar to check, or '-' for STDIN"
                },
            };
            this.Options = new [] {
                new Option {
                    Name = "tokenized",
                    Description="Treat the definitions as tokenized definitions",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var tokenized = (bool)args["tokenized"];

            var grammarFilename = (string)args["grammar-filename"];

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

            if (!errors.ContainsNonWarnings())
            {
                if (tokenized)
                {
                    var tgb = new TokenizedGrammarBuilder();
                    var g = tgb.BuildTokenizedGrammar(dis.ToArray());
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(g.Definitions);
                    errors.AddRange(errors2);
                }
                else
                {
                    var db = new DefinitionBuilder();
                    var defs2 = db.BuildDefinitions(dis.ToArray());
                    var dc = new DefinitionChecker();
                    var errors2 = dc.CheckDefinitions(defs2);
                    errors.AddRange(errors2);
                }
            }

            errors.PrintErrors();

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

