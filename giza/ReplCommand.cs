using System;
using NCommander;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public abstract class ReplCommand : Command
    {
        protected ReplCommand(Dictionary<string, DefinitionExpression> env)
        {
            if (env == null) throw new ArgumentNullException("env");
            Env = env;
        }

        protected readonly Dictionary<string, DefinitionExpression> Env;
    }
}

