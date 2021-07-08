using System;
using System.Collections.Generic;
using System.IO;
using MetaphysicsIndustries.Giza;
using NCommander;

namespace giza
{
    public class CdReplCommand : ReplCommand
    {
        public CdReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "cd";
            Description = "Change the current working directory.";
            HelpText = "This command changes the process's working " +
                       "directory. The working directory affects things like " +
                       "loading or rendering files. The format of the path " +
                       "of the directory to change to depends on the " +
                       "underlying operating system.";
            Params = new[]
            {
                new Parameter
                {
                    Name = "path",
                    ParameterType = ParameterType.String,
                    Description = "A path to the directory to change to. Can " +
                                  "be absolute or relative. If '-', the " +
                                  "working directory will be changed to the " +
                                  "previous one.",
                },
            };
        }

        private string _oldPwd;

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var path = (string) args["path"];
            path = path.Trim();
            string toPrint = null;
            if (path == "-")
            {
                path = _oldPwd;
                toPrint = path;
            }

            var oldPwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
            _oldPwd = oldPwd;
            if (toPrint != null)
                Console.WriteLine(toPrint);
        }
    }
}