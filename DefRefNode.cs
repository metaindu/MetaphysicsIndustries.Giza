
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

namespace MetaphysicsIndustries.Giza
{

    public class DefRefNode : Node
    {
        public DefRefNode(NDefinition def)
            : this(def, def == null ? string.Empty : def.Name)
        {
        }
        public DefRefNode(NDefinition def, string tag)
            : base(tag)
        {
            if (def == null) { throw new ArgumentNullException("def"); }

            _defRef = def;
        }

        private readonly NDefinition _defRef;
        public NDefinition DefRef => _defRef;

        public override string ToString()
        {
            return string.Format("[{0}] \"[{1}] {2}\", {3}", ID, DefRef.ID, DefRef.Name, Tag);
        }
    }
    
}
