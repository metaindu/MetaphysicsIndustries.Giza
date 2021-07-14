// TODO: add license info to all new files
using System;
using System.Collections.Generic;
using System.IO;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class PwdReplCommand : ReplCommand
    {
        public PwdReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "pwd";
            Description = "Print the current working directory.";
            HelpText = "This command prints the process's working " +
                       "directory. The working directory affects things like " +
                       "loading or rendering files. The format of the path " +
                       "of the directory depends on the underlying operating " +
                       "system.";
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var path = Directory.GetCurrentDirectory();
            Console.WriteLine(path);
        }
    }
}