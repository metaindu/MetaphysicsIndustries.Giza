using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct ErrorType : IEquatable<ErrorType>
    {
        public string Name;
        public bool IsWarning;
        public string Description;

        public static bool operator==(ErrorType a, ErrorType b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(ErrorType a, ErrorType b)
        {
            return !a.Equals(b);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ErrorType)) return false;
            return Equals((ErrorType)obj);
        }
        public bool Equals(ErrorType other)
        {
            return
                Name.Equals(other.Name) &&
                IsWarning.Equals(other.IsWarning) &&
                Description.Equals(other.Description);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ IsWarning.GetHashCode() ^ Description.GetHashCode();
        }
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

