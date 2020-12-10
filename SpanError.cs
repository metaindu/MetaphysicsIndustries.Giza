
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

namespace MetaphysicsIndustries.Giza
{
    public class SpanError : Error
    {
        public Span Span;

        public static readonly ErrorType BadStartingNode =          new ErrorType(name:"BadStartingNode",       descriptionFormat:"BadStartingNode"       );
        public static readonly ErrorType BadEndingNode =            new ErrorType(name:"BadEndingNode",         descriptionFormat:"BadEndingNode"         );
        public static readonly ErrorType BadFollow =                new ErrorType(name:"BadFollow",             descriptionFormat:"BadFollow"             );
        public static readonly ErrorType NodeInWrongDefinition =    new ErrorType(name:"NodeInWrongDefinition", descriptionFormat:"NodeInWrongDefinition" );
        public static readonly ErrorType SpanHasNoSubspans =        new ErrorType(name:"SpanHasNoSubspans",     descriptionFormat:"SpanHasNoSubspans"     );
        public static readonly ErrorType CycleInSubspans =          new ErrorType(name:"CycleInSubspans",       descriptionFormat:"CycleInSubspans"       );
    }

}

