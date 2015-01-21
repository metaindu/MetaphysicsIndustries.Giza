using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class ListReplCommand : ReplCommand
    {
        public ListReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "list";
            Description = "List all of the definitions currently defined.";
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var names = Env.Keys.ToList();
            names.Sort();
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }
        }
    }
}

