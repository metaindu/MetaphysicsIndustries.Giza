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

