
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
            _spanner = new Spanner(_grammar.def_format);
        }

        public static string Format(this string format, Func<string, string> values)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (values == null) throw new ArgumentNullException("values");

            var errors = new List<Error>();
            var spans = _spanner.Process(format.ToCharacterSource(), errors);

            if (errors.ContainsNonWarnings())
            {
                throw new FormatException("format is invalid");
            }
            if (spans.Length < 1)
            {
                throw new InvalidOperationException("format produced no spans");
            }
            if (spans.Length > 1)
            {
                throw new FormatException("format is ambiguous");
            }

            var span = spans[0];
            StringBuilder sb = new StringBuilder();

            foreach (var item in span.Subspans)
            {
                if (item.DefRef == _grammar.def_text)
                {
                    sb.Append(item.CollectValue());
                }
                else if (item.DefRef == _grammar.def_escape)
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
                else if (item.DefRef == _grammar.def_param)
                {
                    var name = (
                        from s in item.Subspans 
                            where s.DefRef == _grammar.def_name
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
