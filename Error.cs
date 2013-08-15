using System;
using System.Collections.Generic;

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
            get { return ErrorType.Description; }
        }

        public override string ToString()
        {
            return ErrorType.Name + (IsWarning ? " (warning)" : "");
        }

        public static readonly ErrorType Unknown = new ErrorType() { Name="Unknown", Description="Unknown" };
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

