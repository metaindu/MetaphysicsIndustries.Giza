using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Expression
    {
        public readonly List<ExpressionItem> Items = new List<ExpressionItem>();

        public IEnumerable<DefRefSubExpression> EnumerateDefRefs()
        {
            foreach (var item in this.Items)
            {
                if (item is DefRefSubExpression)
                {
                    yield return (item as DefRefSubExpression);
                }
                else if (item is OrExpression)
                {
                    foreach (var expr in (item as OrExpression).Expressions)
                    {
                        foreach (var defref in expr.EnumerateDefRefs())
                        {
                            yield return defref;
                        }
                    }
                }
            }
        }
    }
}

