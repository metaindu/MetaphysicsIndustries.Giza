
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

using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct ErrorType : IEquatable<ErrorType>
    {
        public ErrorType(string name, string descriptionFormat="{Name}", bool isWarning=false)
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

