
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
    public class LiteralSubExpression : SubExpression
    {
        public LiteralSubExpression(string value="",
                                    string tag="",
                                    bool isSkippable=false,
                                    bool isRepeatable=false)
            : base(tag: tag, isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            Value = value;
        }

        public string Value;

        public string ToDelimitedString()
        {
            var chs = new List<char>();
            chs.Add('\'');

            foreach (var ch in Value)
            {
                if (ch == '\r' ||
                    ch == '\n' ||
                    ch == '\t' ||
                    ch == '\'' ||
                    ch == '\\')
                {
                    char ch2 = ch;
                    switch (ch)
                    {
                        case '\r': ch2 = 'r'; break;
                        case '\n': ch2 = 'n'; break;
                        case '\t': ch2 = 't'; break;
                    }
                    chs.Add('\\');
                    chs.Add(ch2);
                }
//                else if (unicode)
//                {
//                }
                else
                {
                    chs.Add(ch);
                }
            }

            chs.Add('\'');
            return new string(chs.ToArray());
        }
    }
}

