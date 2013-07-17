using System;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionInfo : Expression
    {
        public string Name = string.Empty;
        public readonly Set<DefinitionDirective> Directives = new Set<DefinitionDirective>();
    }
}

