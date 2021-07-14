
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Giza;
using Mono.Terminal;
using NCommander;
using System.Reflection;

namespace giza
{
    public class Program
    {
        public static bool UseLineEditor = true;

        public static void Main(string[] args)
        {
            Editor = new LineEditor("giza");
            Editor.StopEditingOnInterrupt = true;

            var commander = new Commander("giza", GetVersionStringFromAssembly());
            commander.Commands.Add("check", new CheckCommand());
            commander.Commands.Add("parse", new ParseCommand());
            commander.Commands.Add("span", new SpanCommand());
            commander.Commands.Add("render", new RenderCommand());

            try
            {
                if (args.Length > 0 && args[0] == "--version")
                {
                    commander.ShowVersion();
                }
                else if (args.Length < 1)
                {
                    Repl();
                }
                else
                {
                    try
                    {
                        commander.ProcessArgs(args);
                    }
                    catch (NCommanderException ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
                    catch(System.UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
                    catch (System.IO.IOException ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
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

        static LineEditor Editor;

        static void Repl()
        {
            var spanner = new SupergrammarSpanner();
            var env = new Dictionary<string, DefinitionExpression>();

            var buffer = new StringBuilder();
            string primaryPrompt = ">>> ";
            string secondaryPrompt = "... ";

            var editor = Editor;

            var commander = new Commander(">>>", GetVersionStringFromAssembly());
            commander.Commands.Add("list", new ListReplCommand(env));
            commander.Commands.Add("print", new PrintReplCommand(env));
            commander.Commands.Add("delete", new DeleteReplCommand(env));
            commander.Commands.Add("save", new SaveReplCommand(env));
            commander.Commands.Add("load", new LoadReplCommand(env));
            commander.Commands.Add("check", new CheckReplCommand(env));
            commander.Commands.Add("parse", new ParseReplCommand(env));
            commander.Commands.Add("span", new SpanReplCommand(env));
            commander.Commands.Add("render", new RenderReplCommand(env));
            commander.Commands.Add("cd", new CdReplCommand(env));
            commander.Commands.Add("pwd", new PwdReplCommand(env));

            string line;

            while (true)
            {
                buffer.Clear();

                line = editor.Edit(primaryPrompt, "");
                if (editor.EditingWasInterrupted) continue; // Ctrl+C
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
                catch(UnmatchedQuoteException ex)
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
                        try
                        {
                            commander.ProcessArgs(args);
                        }
                        catch(NCommanderException ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                        }
                        catch(System.UnauthorizedAccessException ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                        }
                        catch (System.IO.IOException ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            var errors = new List<Error>();
                            var pg = spanner.GetPreGrammar(buffer.ToString(), errors);
                            var defs = pg.Definitions;
                            if (!errors.ContainsNonWarnings())
                            {
                                // good to go

                                // print any errors
                                foreach (var error in errors)
                                {
                                    Console.WriteLine(error.Description);
                                }

                                // add new definitions to the list
                                foreach (var defexpr in defs)
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

                            line = editor.Edit(secondaryPrompt, "");
                            if (editor.EditingWasInterrupted) 
                                break; // Ctrl+C
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

        public static string ReadTextFromConsole(string prompt="", string secondaryPrompt=null)
        {
            if (string.IsNullOrEmpty(secondaryPrompt)) secondaryPrompt = prompt;

            var buffer = new StringBuilder();

            while (true)
            {
                var line = ReadLineFromConsole(prompt);
                prompt = secondaryPrompt;
                if (Editor.EditingWasInterrupted) continue; // Ctrl+C
                if (line == null) break; // Ctrl+D

                if (line == "") continue;

                buffer.AppendLine(line);
            }

            return buffer.ToString();
        }

        public static string ReadLineFromConsole(string prompt="")
        {
            if (UseLineEditor)
            {
                return Editor.Edit(prompt, "");
            }
            else
            {
                return Console.ReadLine();
            }
        }
    }
}
