using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class OrExpression : ExpressionItem
    {
        public OrExpression(IEnumerable<Expression> expressions=null, bool isSkippable=false, bool isRepeatable=false)
            : base(isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            if (expressions != null)
            {
                Expressions.AddRange(expressions);
            }
        }

        public readonly List<Expression> Expressions = new List<Expression>();
    }
}

