using System;
using NCommander;
using System.IO;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class DeleteReplCommand : ReplCommand
    {
        public DeleteReplCommand(Dictionary<string, DefinitionExpression> env)
            : base(env)
        {
            Name = "delete";
            Description = "Delete the specified definitions.";
            Params = new Parameter[] {
                new Parameter { Name="defnames", ParameterType=ParameterType.StringArray },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defsToDelete = (string[])args["defnames"];
            var someAreMissing = false;
            foreach (var name in defsToDelete)
            {
                if (!Env.ContainsKey(name))
                {
                    Console.WriteLine("There is no definition named \"{0}\".", name);
                    someAreMissing = true;
                }
            }
            if (!someAreMissing)
            {
                foreach (var name in defsToDelete)
                {
                    Env.Remove(name);
                }
            }
        }
    }
}

