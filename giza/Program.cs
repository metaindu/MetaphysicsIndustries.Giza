using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using System.IO;
using NDesk.Options;
using Mono.Terminal;

namespace giza
{
    public class Program
    {
        static OptionSet _options;
        static OptionSet _options2;

        static bool showHelp = false;
        static bool showVersion = false;
        static bool verbose = false;
        static bool tokenized = false;

        public static void Main(string[] args)
        {
            _options = new OptionSet() {
                {   "h|?|help",
                    "Print this help text and exit",
                    x => showHelp = true },
                {   "v|version",
                    "Print version and exit",
                    x => showVersion = true },
                {   "verbose",
                    "Print extra information with some subcommands",
                    x => verbose = true },
            };
            _options2 = new OptionSet() {
                { "tokenized", x => tokenized = true },
            };

            var args2 = _options.Parse(args);

            try
            {
                if (showHelp)
                {
                    ShowUsage();
                    return;
                }

                if (showVersion)
                {
                    ShowVersion();
                    return;
                }

                if (args.Length < 1)
                {
                    Repl();
                    return;
                }

                var command = args2[0].ToLower();
                args2.RemoveAt(0);

                if (command == "super")
                {

                    var args3 = _options2.Parse(args2);

                    if (args3.Count < 1)
                    {
                        ShowUsage();
                        return;
                    }

                    var grammarFilename = args3[0];

                    string grammar;
                    if (grammarFilename == "-")
                    {
                        grammar = Console.In.ReadToEnd();
                    }
                    else
                    {
                        grammar = File.ReadAllText(grammarFilename);
                    }

                    Super(grammar);
                }
                else if (command == "render")
                {
                    Render(args2);
                }
                else if (command == "parse")
                {
                    Parse(args2);
                }
                else if (command == "span")
                {
                    Span(args2);
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown command: \"{0}\"", command));
                    ShowUsage();
                }
            }
            catch (Exception ex)
            {
                Console.Write("There was an internal error");
                if (verbose)
                {
                    Console.WriteLine(":");
                    Console.WriteLine(ex.ToString());
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }

        private static int CountLines(string input, int length)
        {
            int n = 1;
            int i;
            for (i=0;i<length;i++)
            {
                if (input[i] == '\n') n++;
            }
            return n;
        }
        private static int CountCharactersOnLine(string input, int endIndex)
        {
            int n = 0;
            int i = endIndex;
            do
            {
                if (input[i] != '\r') n++;
                i--;
            } while (i >= 0 && input[i] != '\n');

            return n;
        }

        static void ShowVersion()
        {
            Console.WriteLine("giza.exe version x.y.z");
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    giza [options]");
            Console.WriteLine("    giza span [options] [GRAMMAR FILE] [START SYMBOL] [FILE]");
            Console.WriteLine("    giza parse [options] [GRAMMAR FILE] [START SYMBOL] [FILE]");
            Console.WriteLine("    giza super [GRAMMAR FILE]");
            Console.WriteLine("    giza render [GRAMMAR FILE] [CLASS NAME]");
            Console.WriteLine();
            Console.WriteLine("Subcommands:");
            Console.WriteLine();
            Console.WriteLine("    span,           Process the input file with a non-tokenized grammar, starting with a given symbol.");
            Console.WriteLine("    parse,          Parse the input file with a tokenized grammar, starting with a given symbol.");
            Console.WriteLine("    super,          Process the grammar file only.");
            Console.WriteLine("    render,         Process the grammar file and print its definitions as a C# class.");
            Console.WriteLine();
            Console.WriteLine("If \"-\" is given for FILE, or for GRAMMAR FILE given to super, then it is read from standard input.");
            Console.WriteLine();

            _options.WriteOptionDescriptions(Console.Out);
        }

        static void Super(string grammar)
        {
            SupergrammarSpanner sgs = new SupergrammarSpanner();
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

        static void Render(List<string> args)
        {
            string ns = "MetaphysicsIndustries.Giza";

            bool singleton = false;

            var options2 = new OptionSet() {
                { "tokenized", x => tokenized = true },
                { "ns|namespace=", x => ns = x ?? ns },
                { "singleton", x => singleton = true },
            };

            args = options2.Parse(args);

            if (args.Count < 2)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            string className = args[1];

            var sgs = new SupergrammarSpanner();
            string gfile;
            if (grammarFilename == "-")
            {
                gfile = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                gfile = File.ReadAllText(grammarFilename);
            }

            List<Error> errors = new List<Error>();
            var dis = sgs.GetExpressions(gfile, errors);

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
                Console.Write(dr.RenderDefinitionsAsCSharpClass(className, g.Definitions, ns: ns, singleton: singleton));
            }
        }

        static void Parse(List<string> args)
        {
            if (args.Count < 3)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            var startSymbol = args[1];
            var inputFilename = args[2];

            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(grammarFilename);
            var grammarErrors = new List<Error>();
            var dis = spanner.GetExpressions(grammarFile, grammarErrors);

            if (!grammarErrors.ContainsNonWarnings())
            {
                var ec = new ExpressionChecker();
                var errors2 = ec.CheckDefinitionInfosForParsing(dis);
                grammarErrors.AddRange(errors2);
            }

            Grammar g;
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
            g = tgb.BuildTokenizedGrammar(dis);

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

            string input;
            if (inputFilename == "-")
            {
                input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                try
                {
                    input = File.ReadAllText(inputFilename);
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error while trying to open the input file:");
                    Console.WriteLine("  {0}", e.Message);
                    if (verbose)
                    {
                        Console.WriteLine("  {0}", e.ToString());
                    }
                    return;
                }
            }

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

        static void Span(List<string> args)
        {
            if (args.Count < 3)
            {
                ShowUsage();
                return;
            }

            var grammarFilename = args[0];
            var startSymbol = args[1];
            var inputFilename = args[2];

            SupergrammarSpanner spanner = new SupergrammarSpanner();
            string grammarFile = File.ReadAllText(grammarFilename);
            var errors = new List<Error>();
            var dis = spanner.GetExpressions(grammarFile, errors);

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

            string input;
            if (inputFilename == "-")
            {
                input = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                input = File.ReadAllText(inputFilename);
            }

            Grammar g = new Grammar();
            g.Definitions.AddRange(defs);

            Spanner gs = new Spanner(g.FindDefinitionByName(startSymbol));
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

                var command = line.Trim();

                if (command == "exit" || command == "quit")
                {
                    break;
                }

                if (command == "list")
                {
                    var names = env.Keys.ToList();
                    names.Sort();
                    foreach (var name in names)
                    {
                        Console.WriteLine(name);
                    }
                    continue;
                }

                if (command == "print" || command.StartsWith("print "))
                {
                    var defnames = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

                    var dr = new DefinitionRenderer();
                    int? width = Console.WindowWidth;
                    if (width < 20)
                        width = null;
                    if (defnames.Count < 1)
                    {
                        defnames = env.Keys.ToList();
                    }
                    defnames.Sort();
                    var defs = defnames.Where(name => env.ContainsKey(name)).Select(name => env[name]);
                    bool first = true;
                    foreach (var name in defnames.Where(name => !env.ContainsKey(name)))
                    {
                        if (first)
                        {
                            Console.WriteLine();
                        }
                        first = false;
                        Console.WriteLine("There is no definition named \"{0}\".", name);
                    }
                    Console.Write(dr.RenderDefinitionExprsAsGrammarText(defs, width));
                    continue;
                }

                if (command.StartsWith("delete "))
                {
                    var defsToDelete = command.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries).Skip(1);
                    var someAreMissing = false;
                    foreach (var name in defsToDelete)
                    {
                        if (!env.ContainsKey(name))
                        {
                            Console.WriteLine("There is no definition named \"{0}\".", name);
                            someAreMissing = true;
                        }
                    }
                    if (!someAreMissing)
                    {
                        foreach (var name in defsToDelete)
                        {
                            env.Remove(name);
                        }
                    }
                    continue;
                }

                if (command.StartsWith("save "))
                {
                    var parts = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
                    var filename = parts[0];
                    parts.RemoveAt(0);
                    var names = parts;

                    var someAreMissing = false;
                    foreach (var name in names)
                    {
                        if (!env.ContainsKey(name))
                        {
                            Console.WriteLine("There is no definition named \"{0}\".", name);
                            someAreMissing = true;
                        }
                    }

                    if (someAreMissing) continue;

                    var defs = names.Select(name => env[name]);
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
                                    Console.WriteLine("There is no definition named \"{0}\".", defref.DefinitionName);
                                    someAreMissing = true;
                                }
                            }
                        }
                        next.ExceptWith(alldefs);
                        alldefs.UnionWith(next);
                        prev.Clear();
                        prev.UnionWith(next);
                    }

                    if (someAreMissing) continue;

                    try
                    {
                        var dr = new DefinitionRenderer();
                        var fileContents = dr.RenderDefinitionExprsAsGrammarText(alldefs);
                        string header = string.Format("// File saved at {0}", DateTime.Now);
                        Console.WriteLine(header);
                        using (var f = new StreamWriter(filename))
                        {
                            f.WriteLine(header);
                            f.WriteLine();
                            f.Write(fileContents);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an error:");
                        Console.WriteLine(ex);
                    }

                    continue;
                }

                if (command.StartsWith("load "))
                {
                    var parts = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
                    var filename = parts[0];

                    if (!File.Exists(filename))
                    {
                        Console.WriteLine("Can't find the file \"{0}\"", filename);
                        continue;
                    }

                    try
                    {
                        string contents = File.ReadAllText(filename);
                        var errors = new List<Error>();
                        var defs = spanner.GetExpressions(contents, errors);
                        if (errors.ContainsNonWarnings())
                        {
                            Console.WriteLine("There are errors in the loaded file:");
                            foreach (var error in errors.Where(e => !e.IsWarning))
                            {
                                Console.WriteLine(error.Description);
                            }
                            continue;
                        }

                        foreach (var def in defs)
                        {
                            if (env.ContainsKey(def.Name))
                            {
                                Console.WriteLine("{0} was replaced", def.Name);
                            }
                            else
                            {
                                Console.WriteLine("{0} was added", def.Name);
                            }

                            env[def.Name] = def;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an internal error:");
                        Console.WriteLine(ex);
                    }

                    continue;
                }

                if (command == "check" || command.StartsWith("check "))
                {

                    var defnames = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
                    if (defnames.Count < 1)
                    {
                        defnames = env.Keys.ToList();
                    }
                    else
                    {
                        bool someAreMissing = false;
                        foreach (var name in defnames)
                        {
                            if (!env.ContainsKey(name))
                            {
                                Console.WriteLine("There is no definition named \"{0}\".", name);
                                someAreMissing = true;
                            }
                        }
                        if (someAreMissing) continue;
                    }

                    var ec = new ExpressionChecker();
                    var defs = defnames.Select(name => env[name]);
                    var errors = ec.CheckDefinitionInfosForSpanning(defs);

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

                    continue;
                }

                try
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
                        if (gotCtrlC) break;
                        if (line == null) break; // Ctrl+D

                        buffer.AppendLine(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("There was an internal error: ");
                    Console.WriteLine(ex);
                }
            }


        }
    }
}
