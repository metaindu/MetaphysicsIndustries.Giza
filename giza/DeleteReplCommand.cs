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
            Params = new [] {
                new Parameter {
                    Name="def-names",
                    ParameterType=ParameterType.StringArray,
                    Description="The names of the definitions to delete. If '*' is given, then all definitions are deleted.",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defsToDelete = (string[])args["def-names"];

            if (defsToDelete == null ||
                defsToDelete.Length < 1)
            {
                Console.WriteLine("Error: no definitions were specified.");
                return;
            }

            if (defsToDelete.Length == 1 &&
                defsToDelete[0] == "*")
            {
                Env.Clear();
                Console.WriteLine("All definitions were deleted.");
                return;
            }

            var someAreMissing = false;
            foreach (var name in defsToDelete)
            {
                if (!Env.ContainsKey(name))
                {
                    Console.WriteLine("Error: There is no definition named \"{0}\".", name);
                    someAreMissing = true;
                }
            }

            if (someAreMissing)
            {
                Console.WriteLine("No definitions were deleted.");
            }
            else
            {
                foreach (var name in defsToDelete)
                {
                    Env.Remove(name);
                }
            }
        }
    }
}

