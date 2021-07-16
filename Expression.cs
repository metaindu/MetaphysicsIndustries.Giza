
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Expression
    {
        public Expression(params ExpressionItem[] items)
            : this((IEnumerable<ExpressionItem>)items)
        {
        }
        public Expression(IEnumerable<ExpressionItem> items=null)
        {
            if (items != null)
            {
                Items.AddRange(items);
            }
        }

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

