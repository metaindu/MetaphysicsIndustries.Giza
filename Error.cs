
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
using System.Reflection;

namespace MetaphysicsIndustries.Giza
{
    public class Error
    {
        public ErrorType ErrorType;

        public bool IsWarning
        {
            get { return ErrorType.IsWarning; }
        }

        public virtual string Description
        {
            get
            {
                return ErrorType.DescriptionFormat.Format(this.GetResolver());
            }
        }

        public override string ToString()
        {
            return ErrorType.Name + (IsWarning ? " (warning)" : "");
        }

        public static readonly ErrorType Unknown = new ErrorType(name: "Unknown");

        Func<string, string> _resolver;
        public Func<string, string> GetResolver()
        {
            if (_resolver == null)
            {
                _resolver = MakeResolver();
            }

            return _resolver;
        }

        Func<string, string> MakeResolver()
        {
            var type = this.GetType();
            Dictionary<string, FieldInfo> gettableFields;
            Dictionary<string, PropertyInfo> gettableProperties;
            if (_fields.ContainsKey(type))
            {
                gettableFields = _fields[type];
                gettableProperties = _properties[type];
            }
            else
            {
                var members = type.GetMembers(
                    BindingFlags.GetField |
                    BindingFlags.GetProperty |
                    BindingFlags.Static |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                gettableFields = new Dictionary<string, FieldInfo>();
                gettableProperties = new Dictionary<string, PropertyInfo>();
                foreach (var member in members)
                {
                    try
                    {
                        if (member.Name == "Description")
                        {
                            continue;
                        }

                        if (member is FieldInfo)
                        {
                            var field = (member as FieldInfo);
                            field.GetValue(this);
                            gettableFields.Add(field.Name, field);
                        }
                        else if (member is PropertyInfo)
                        {
                            var property = (member as PropertyInfo);
                            property.GetValue(this, null);
                            gettableProperties.Add(property.Name, property);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            Func<string, string> resolver =
                (x) =>
                {
                    object value;

                    if (gettableFields.ContainsKey(x))
                    {
                        value = gettableFields[x].GetValue(this);
                    }
                    else if (gettableProperties.ContainsKey(x))
                    {
                    value = gettableProperties[x].GetValue(this, null);
                    }
                    else
                    {
                        return null;
                    }

                    if (value != null) return value.ToString();

                    return null;
                };

            return resolver;
        }
        static Dictionary<Type, Dictionary<string, FieldInfo>> _fields = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        static Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
    }

    public static class ErrorHelpers
    {
        public static bool ContainsWarnings(this IEnumerable<Error> errors)
        {
            foreach (var err in errors)
            {
                if (err.IsWarning) return true;
            }

            return false;
        }

        public static bool ContainsNonWarnings(this IEnumerable<Error> errors)
        {
            foreach (var err in errors)
            {
                if (!err.IsWarning) return true;
            }

            return false;
        }

        public static int GetWarningsCount(this IEnumerable<Error> errors)
        {
            int n = 0;
            foreach (var err in errors)
            {
                if (err.IsWarning) n++;
            }

            return n;
        }

        public static int GetNonWarningsCount(this IEnumerable<Error> errors)
        {
            int n = 0;
            foreach (var err in errors)
            {
                if (!err.IsWarning) n++;
            }

            return n;
        }

        public static Error GetFirstWarning(this IEnumerable<Error> errors)
        {
            foreach (var err in errors)
            {
                if (err.IsWarning) return err;
            }

            return null;
        }

        public static Error GetFirstNonWarning(this IEnumerable<Error> errors)
        {
            foreach (var err in errors)
            {
                if (!err.IsWarning) return err;
            }

            return null;
        }
    }
}

