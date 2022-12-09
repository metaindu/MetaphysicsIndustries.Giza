
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
    /* NDefinitions are graphs of interconnected nodes. The graphs of nodes
       that result from using the supergrammar are not all of the possible
       graphs that Spanner can deal with. That is, there are other graphs
       that could be constructed manually, or by some system other than
       GrammarCompiler, that would still work. The purpose of the
       DefinitionChecker then, is to confirm that a given NDefinition can be
       used by Spanner (a looser requirement), and NOT to confirm that the
       NDefinition conforms to what the supergrammar can output (a narrower
       requirement). */

    public class DefinitionError : Error
    {
        public static readonly ErrorType NextNodeLinksOutsideOfDefinition =     new ErrorType(name:"NextNodeLinksOutsideOfDefinition",  descriptionFormat:"NextNodeLinksOutsideOfDefinition"  );
        public static readonly ErrorType StartNodeHasWrongParentDefinition =    new ErrorType(name:"StartNodeHasWrongParentDefinition", descriptionFormat:"StartNodeHasWrongParentDefinition" );
        public static readonly ErrorType EndNodeHasWrongParentDefinition =      new ErrorType(name:"EndNodeHasWrongParentDefinition",   descriptionFormat:"EndNodeHasWrongParentDefinition"   );
        public static readonly ErrorType LeadingReferenceCycle =                new ErrorType(name:"LeadingReferenceCycle",             descriptionFormat:"LeadingReferenceCycle"             );
        public static readonly ErrorType NodeHasNoPathFromStart =               new ErrorType(name:"NodeHasNoPathFromStart");
        public static readonly ErrorType NodeHasNoPathToEnd =                   new ErrorType(name:"NodeHasNoPathToEnd");

        public Node Node;
        public List<NDefinition> Cycle;
    }
}

