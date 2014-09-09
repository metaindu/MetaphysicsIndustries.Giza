using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;
using NDesk.Options;
using Mono.Terminal;
using NCommander;
using System.Reflection;

namespace giza
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool showVersion = false;
            var options = new OptionSet() {
                {   "v|version",
                    "Print version and exit",
                    x => showVersion = true },
            };

            var args2 = options.Parse(args);

            var commander = new Commander("giza", GetVersionStringFromAssembly());
            commander.Commands.Add("check", new CheckCommand());
            commander.Commands.Add("parse", new ParseCommand());
            commander.Commands.Add("span", new SpanCommand());
            commander.Commands.Add("render", new RenderCommand());

            try
            {
                if (showVersion)
                {
                    commander.ShowVersion();
                }
                else if (args2.Count < 1)
                {
                    Repl();
                }
                else
                {
                    commander.ProcessArgs(args2);
                }
            }
            catch (Exception ex)
            {
                Console.Write("There was an internal error:");
                Console.WriteLine(ex.ToString());
            }
        }


        public static string GetVersionStringFromAssembly()
        {
            var version = Assembly.GetCallingAssembly().GetName().Version;
            return version.ToString(version.Major == 0 ? 2 : 3);
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
            else
            {
                var dr = new DefinitionRenderer();
                Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions, ns: ns, singleton: isSingleton));
            }
        }

        public static void Parse(bool verbose, string grammar, string input, string startSymbol)
        {
            var spanner = new SupergrammarSpanner();
            var grammarErrors = new List<Error>();
            var dis = spanner.GetExpressions(grammar, grammarErrors);

            if (!grammarErrors.ContainsNonWarnings())
            {
                var ec = new ExpressionChecker();
                var errors2 = ec.CheckDefinitionInfosForParsing(dis);
                grammarErrors.AddRange(errors2);
            }

            if (grammarErrors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the grammar:");
            }
            else if (grammarErrors.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the grammar:");
            }

            foreach (var err in grammarErrors)
            {
                Console.WriteLine("  {0}", err.Description);
            }

            if (grammarErrors.ContainsNonWarnings())
            {
                return;
            }

            var tgb = new TokenizedGrammarBuilder();
            var g = tgb.BuildTokenizedGrammar(dis);

            var startDefinition = g.FindDefinitionByName(startSymbol);
            if (startDefinition == null)
            {
                Console.WriteLine("There is no defintion named \"{0}\".", startSymbol);
                Console.WriteLine("There are {0} definitions in the grammar:", g.Definitions.Count());
                foreach (var def in g.Definitions)
                {
                    Console.WriteLine("  {0}", def.Name);
                }
                return;
            }


            Parse(input, startDefinition, verbose);
        }
        public static void Parse(string input, Definition startDefinition, bool verbose)
        {
            var parser = new Parser(startDefinition);
            var inputErrors = new List<Error>();
            Span[] ss = parser.Parse(input.ToCharacterSource(), inputErrors);

            if (inputErrors.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the input:");
            }
            else if (inputErrors.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the input:");
            }

            foreach (var err in inputErrors)
            {
                Console.WriteLine("  {0}", err.Description);
            }

            if (inputErrors.ContainsNonWarnings())
            {
                return;
            }

            if (ss.Length < 1)
            {
                Console.WriteLine("There are no valid parses of the input.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("There are {0} valid parses of the input.", ss.Length);
                //foreach (Span s in ss)
                //{
                //    Console.WriteLine(s.RenderSpanHierarchy());
                //}
            }
            else
            {
                Console.WriteLine("There is 1 valid parse of the input.");
                if (verbose)
                {
                    Console.WriteLine(ss[0].RenderSpanHierarchy());
                }
            }
        }

        public static void Span(bool verbose, string grammar, string input, string startSymbol)
        {
            var spanner = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = spanner.GetExpressions(grammar, errors);

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

            var ec = new ExpressionChecker();
            errors = ec.CheckDefinitionInfos(dis);

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

            DefinitionBuilder db = new DefinitionBuilder();
            var defs = db.BuildDefinitions(dis);

            var startDefinition = defs.First(d => d.Name == startSymbol);
            Span(input, startDefinition, verbose);
        }
        public static void Span(string input, Definition startDefinition, bool verbose)
        {
            Spanner gs = new Spanner(startDefinition);
            var errors = new List<Error>();
            Span[] ss = gs.Process(input.ToCharacterSource(), errors);
            if (errors != null && errors.Count > 0)
            {
                Console.WriteLine("There are errors in the input:");
                foreach (var err in errors)
                {
                    Console.Write("  ");
                    Console.WriteLine(err.Description);
                }
                return;
            }
            else if (ss.Length < 1)
            {
                Console.WriteLine("No valid spans.");
            }
            else if (ss.Length > 1)
            {
                Console.WriteLine("{0} valid spans.", ss.Length);
//                foreach (Span s in ss)
//                {
//                    Console.WriteLine(s.RenderSpanHierarchy());
//                }
            }
            else
            {
                Console.WriteLine("1 valid span.");
                if (verbose)
                {
                    Console.WriteLine(ss[0].RenderSpanHierarchy());
                }
            }
        }

        static void Repl()
        {
            var spanner = new SupergrammarSpanner();
            var env = new Dictionary<string, DefinitionExpression>();

            var buffer = new StringBuilder();
            string primaryPrompt = ">>> ";
            string secondaryPrompt = "... ";

            bool gotCtrlC = false;
            EventHandler onInterrupt = (sender, a) => {
                gotCtrlC = true;
            };
            var editor = new LineEditor("giza");
            editor.StopEditingOnInterrupt = true;
            editor.EditingInterrupted += onInterrupt;

            var commander = new Commander("giza repl", GetVersionStringFromAssembly());
            commander.Commands.Add("list", new ListReplCommand(env));
            commander.Commands.Add("print", new PrintReplCommand(env));
            commander.Commands.Add("delete", new DeleteReplCommand(env));
            commander.Commands.Add("save", new SaveReplCommand(env));
            commander.Commands.Add("load", new LoadReplCommand(env));
            commander.Commands.Add("check", new CheckReplCommand(env));
            commander.Commands.Add("parse", new ParseReplCommand(env));
            commander.Commands.Add("span", new ParseReplCommand(env));

            string line;

            while (true)
            {
                buffer.Clear();

                gotCtrlC = false;
                line = editor.Edit(primaryPrompt, "");
                if (gotCtrlC) continue;
                if (line == null) break; // Ctrl+D

                if (line == "") continue;

                buffer.AppendLine(line);

                var _commandLine = line.Trim();
                bool splittingFailed = false;
                string command = null;
                List<string> args = null;
                try
                {
                    args = Splitter.SplitArgs(_commandLine).ToList();
                    command = args[0];
                }
                catch(Splitter.UnmatchedQuoteException ex)
                {
                    splittingFailed = true;
                }

                if (command == "exit" || command == "quit")
                {
                    break;
                }

                try
                {
                    if (!splittingFailed && commander.Commands.ContainsKey(command))
                    {
                        commander.ProcessArgs(args);
                    }
                    else
                    {
                        while (true)
                        {
                            var errors = new List<Error>();
                            var defexprs = spanner.GetExpressions(buffer.ToString(), errors);
                            if (!errors.ContainsNonWarnings())
                            {
                                // good to go

                                // print any errors
                                foreach (var error in errors)
                                {
                                    Console.WriteLine(error.Description);
                                }

                                // add new definitions to the list
                                foreach (var defexpr in defexprs)
                                {
                                    env[defexpr.Name] = defexpr;
                                }

                                break;
                            }

                            if (errors.Any(e => !e.IsWarning && e.ErrorType != ParserError.UnexpectedEndOfInput))
                            {
                                // something is wrong with the input
                                foreach (var error in errors)
                                {
                                    Console.WriteLine(error.Description);
                                }
                                break;
                            }

                            gotCtrlC = false;
                            line = editor.Edit(secondaryPrompt, "");
                            if (gotCtrlC)
                                break;
                            if (line == null)
                                break; // Ctrl+D

                            buffer.AppendLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("There was an internal error: ");
                    Console.WriteLine(ex);
                }
            }


        }

        public static string ReadTextFromConsole()
        {
            return Console.In.ReadToEnd();
        }
    }
}
