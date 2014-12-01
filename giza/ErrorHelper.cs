using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Giza;
using System.Linq;
using NCommander;

namespace giza
{
    public static class ErrorHelper
    {
        public static void PrintErrors(this IEnumerable<Error> errors, bool printIfThereAreNone=false)
        {
            if (errors == null) return;

            var errors2 = errors.ToList();

            if (errors2.ContainsNonWarnings())
            {
                Console.WriteLine("There are errors in the grammar:");
            }
            else if (errors2.ContainsWarnings())
            {
                Console.WriteLine("There are warnings in the grammar:");
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

