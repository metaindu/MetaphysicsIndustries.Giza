using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public static class StringFormatter
    {
        static StringFormatterGrammar _grammar;
        static Spanner _spanner;

        static StringFormatter()
        {
            _grammar = new StringFormatterGrammar();
            _spanner = new Spanner(_grammar.def_0_format);
        }

        public static string Format(this string format, Func<string, string> values)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (values == null) throw new ArgumentNullException("values");

            var errors = new List<Error>();
            var spans = _spanner.Process(format, errors);

            if (errors.ContainsNonWarnings())
            {
                throw new FormatException("format is invalid");
            }
            if (spans.Length != 1)
            {
                throw new InvalidOperationException("format produced no spans");
            }

            var span = spans[0];
            StringBuilder sb = new StringBuilder();

            foreach (var item in span.Subspans)
            {
                if (item.DefRef == _grammar.def_1_text)
                {
                    sb.Append(item.CollectValue());
                }
                else if (item.DefRef == _grammar.def_2_escape)
                {
                    if (item.CollectValue() == "{{")
                    {
                        sb.Append("{");
                    }
                    else
                    {
                        sb.Append("}");
                    }
                }
                else if (item.DefRef == _grammar.def_3_param)
                {
                    var name = (
                        from s in item.Subspans 
                            where s.DefRef == _grammar.def_4_name
                            select s
                        ).First();

                    var value = values(name.CollectValue());
                    if (value != null)
                    {
                        sb.Append(value);
                    }
                    else
                    {
                        sb.Append(item.CollectValue());
                    }
                }
            }

            return sb.ToString();
        }

        public static string Format(this string format, IDictionary<string, string> values)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (values == null) throw new ArgumentNullException("values");

            return Format(format, ((x) => (values.ContainsKey(x) ? values[x] : null)));
        }
    }
}
