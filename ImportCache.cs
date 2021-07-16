
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

using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class ImportCache
    {
        public bool ContainsKey(string name)
        {
            return _cache.ContainsKey(name);
        }

        public Dictionary<string, Definition> this[string key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        private Dictionary<string, Dictionary<string, Definition>>
            _cache =
                new Dictionary<string,
                    Dictionary<string, Definition>>();
    }
}
