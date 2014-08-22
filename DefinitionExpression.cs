using System;
using MetaphysicsIndustries.Collections;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionExpression : Expression
    {
        public DefinitionExpression(string name="",
                                    IEnumerable<DefinitionDirective> directives=null,
                                    IEnumerable<ExpressionItem> items=null)
            : base(items: items)
        {
            Name = name;
            if (directives != null)
            {
                Directives.AddRange(directives);
            }
        }

        public string Name = string.Empty;
        public readonly Set<DefinitionDirective> Directives = new Set<DefinitionDirective>();
    }
}

