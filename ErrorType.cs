using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct ErrorType
    {
        public string Name;
        public bool IsWarning;
        public string Description;
    }

    public static class ErrorTypeHelpers
    {
        public static bool ContainsWarnings(this IEnumerable<ErrorType> errorTypes)
        {
            foreach (var et in errorTypes)
            {
                if (et.IsWarning) return true;
            }

            return false;
        }

        public static bool ContainsNonWarnings(this IEnumerable<ErrorType> errorTypes)
        {
            foreach (var et in errorTypes)
            {
                if (!et.IsWarning) return true;
            }

            return false;
        }
    }
}

