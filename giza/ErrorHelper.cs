
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
using MetaphysicsIndustries.Giza;
using System.Linq;
using NCommander;

namespace giza
{
    public static class ErrorHelper
    {
        public static void PrintErrors(this IEnumerable<Error> errors, bool printIfThereAreNone=false, string context=null)
        {
            if (errors == null) return;

            if (context == null)
            {
                context = "";
            }

            var errors2 = errors.ToList();

            if (errors2.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors{0}:", context);
            }
            else if (errors2.ContainsWarnings())
            {
                Console.WriteLine("There are warnings{0}:", context);
            }
            else if (printIfThereAreNone)
            {
                Console.WriteLine("There are no errors or warnings.");
            }

            foreach (var err in errors2)
            {
                Console.WriteLine("  {0}", err.Description);
            }
        }
    }
}

