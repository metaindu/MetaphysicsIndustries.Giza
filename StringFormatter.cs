using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public static class StringFormatter
    {
        static Grammar _grammar;
        static Spanner _spanner;

        static StringFormatter()
        {
            _grammar = new StringFormatterGrammar();
            _spanner = new Spanner(_grammar.FindDefinitionByName("format"));
        }

        public static string Format(this string format, Func<string, string> values)
        {
            throw new NotImplementedException();
        }

        public static string Format(this string format, IDictionary<string, string> values)
        {
            return Format(format, (x) => values[x]);
        }
    }
}
