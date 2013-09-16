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
            get { return string.Format(ErrorType.DescriptionFormat, ErrorType.Name); }
        }

        public override string ToString()
        {
            return ErrorType.Name + (IsWarning ? " (warning)" : "");
        }

        public static readonly ErrorType Unknown = new ErrorType(name: "Unknown");

        Func<string, string> _resolver;
        public Func<string, string> Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    _resolver = MakeResolver();
                }

                return _resolver;
            }
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
                    gettableFields.ContainsKey(x) ? gettableFields[x].GetValue(this).ToString() :
                    gettableProperties.ContainsKey(x) ? gettableProperties[x].GetValue(this, null).ToString() :
                    null;
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

