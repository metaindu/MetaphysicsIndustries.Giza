
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
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
                Directives.UnionWith(directives);
            }
        }

        public string Name = string.Empty;
        public readonly HashSet<DefinitionDirective> Directives = new HashSet<DefinitionDirective>();
    }
}

