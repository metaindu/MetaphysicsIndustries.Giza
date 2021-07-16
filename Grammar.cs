
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
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public Grammar(IEnumerable<Definition> definitions = null,
            IEnumerable<ImportStatement> importStatements = null,
            string source = null)
        {
            if (definitions != null) Definitions.AddRange(definitions);
            if (importStatements != null)
                ImportStatements.AddRange(importStatements);
            Source = source;
        }

        public readonly List<Definition> Definitions = new List<Definition>();
        public readonly List<ImportStatement> ImportStatements =
            new List<ImportStatement>();
        public string Source;

        public Definition FindDefinitionByName(string name)
        {
            return Definitions.FirstOrDefault(def => def.Name == name);
        }

        public Grammar Clone(IEnumerable<Definition> newDefinitions=null,
            IEnumerable<ImportStatement> newImportStatements=null,
            string newSource=null)
        {
            if (newDefinitions == null) newDefinitions = Definitions;
            if (newImportStatements == null)
                newImportStatements = ImportStatements;
            if (newSource == null) newSource = Source;

            return new Grammar(newDefinitions, newImportStatements, newSource);
        }
    }
}
