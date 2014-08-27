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

