using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct ErrorType : IEquatable<ErrorType>
    {
        public ErrorType(string name, string descriptionFormat="{0}", bool isWarning=false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (descriptionFormat == null) throw new ArgumentNullException("descriptionFormat");

            Name = name;
            DescriptionFormat = descriptionFormat;
            IsWarning = isWarning;
        }

        public string Name;
        public string DescriptionFormat;
        public bool IsWarning;

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
                DescriptionFormat.Equals(other.DescriptionFormat);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ IsWarning.GetHashCode() ^ DescriptionFormat.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
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

