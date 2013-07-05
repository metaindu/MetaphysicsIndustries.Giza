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

        public IEnumerable<LiteralSubExpression> EnumerateLiterals()
        {
            foreach (var item in this.Items)
            {
                if (item is LiteralSubExpression)
                {
                    yield return (item as LiteralSubExpression);
                }
                else if (item is OrExpression)
                {
                    foreach (var expr in (item as OrExpression).Expressions)
                    {
                        foreach (var literal in expr.EnumerateLiterals())
                        {
                            yield return literal;
                        }
                    }
                }
            }
        }

        public IEnumerable<CharClassSubExpression> EnumerateCharClasses()
        {
            foreach (var item in this.Items)
            {
                if (item is CharClassSubExpression)
                {
                    yield return (item as CharClassSubExpression);
                }
                else if (item is OrExpression)
                {
                    foreach (var expr in (item as OrExpression).Expressions)
                    {
                        foreach (var cc in expr.EnumerateCharClasses())
                        {
                            yield return cc;
                        }
                    }
                }
            }
        }
    }
}

