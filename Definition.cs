
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

/*
 * Definition and related classes form the set of domain objects for the
 * supergrammar. That is, instances of the classes represent element of a
 * grammar. There are no nodes.
 *
 * In contrast, the NDefinition class represents a definition within a grammar
 * that is ready to be used by Parser. It is equivalent to a kind of
 * state machine, with all of the states represented by nodes. It does not use
 * hierarchical trees of Expression and ExpressionItem.
 *
 * TODO: rename these and related classes to make the distinction more apparent.
 */

using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Definition
    {
        public Definition(string name = "",
            IEnumerable<DefinitionDirective> directives = null,
            Expression expr = null)
        {
            Name = name;
            if (directives != null)
            {
                Directives.UnionWith(directives);
            }

            if (expr == null)
                expr = new Expression();
            Expr = expr;
        }

        public override string ToString() => $"Definition {Name}";

        public string Name = string.Empty;
        public readonly HashSet<DefinitionDirective> Directives = new HashSet<DefinitionDirective>();
        public Expression Expr;

        public bool MindWhitespace => Directives.Contains(DefinitionDirective.MindWhitespace);
        public bool IgnoreCase => Directives.Contains(DefinitionDirective.IgnoreCase);
        public bool Atomic => Directives.Contains(DefinitionDirective.Atomic);

        public bool IsTokenized =>
            Directives.Contains(DefinitionDirective.Token) ||
            Directives.Contains(DefinitionDirective.Subtoken) ||
            Directives.Contains(DefinitionDirective.Comment);

        public bool IsComment => Directives.Contains(DefinitionDirective.Comment);
        public bool IsImported { get; set; } = false;
        
        public IEnumerable<DefRefSubExpression> EnumerateDefRefs() => 
            Expr.EnumerateDefRefs();

        public IEnumerable<LiteralSubExpression> EnumerateLiterals() => 
            Expr.EnumerateLiterals();

        public IEnumerable<CharClassSubExpression> EnumerateCharClasses() => 
            Expr.EnumerateCharClasses();
    }
}

