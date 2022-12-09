
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
    public class CharClass
    {
        public CharClass(char ch)
            : this(new char[1]{ch})
        {
        }
        public CharClass(char[] chars)
            : this(chars, false, false, false, false)
        {
        }
        public CharClass(char[] chars, bool exclude)
            : this(chars, exclude, false, false, false)
        {
        }
        public CharClass(char[] chars, bool exclude, bool letter, bool digit, bool whitespace)
        {
            if (chars == null) { throw new ArgumentNullException("chars"); }

            _letter = letter;
            _digit = digit;
            _whitespace = whitespace;
            _exclude = exclude;

            HashSet<char> chs = new HashSet<char>(chars);

            if (_letter || chs.ContainsAll(LetterChars))
            {
                _letter = true;
                chs.ExceptWith(LetterChars);
            }

            if (_digit || chs.ContainsAll(DigitChars))
            {
                _digit = true;
                chs.ExceptWith(DigitChars);
            }

            if (_whitespace || chs.ContainsAll(WhitespaceChars))
            {
                _whitespace = true;
                chs.ExceptWith(WhitespaceChars);
            }

            _chars = chs.ToArray();
        }

        bool _letter;
        public bool Letter
        {
            get { return _letter; }
        }
        bool _digit;
        public bool Digit
        {
            get { return _digit; }
        }
        bool _whitespace;
        public bool Whitespace
        {
            get { return _whitespace; }
        }
        bool _exclude;
        public bool Exclude
        {
            get { return _exclude; }
        }

        char[] _chars = new char[0];

        private static char[] LetterChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static char[] DigitChars = "0123456789".ToCharArray();
        private static char[] WhitespaceChars = "\r\n\t ".ToCharArray();

        public bool Matches(char ch)
        {
            if (Letter && char.IsLetter(ch))
            {
                return !Exclude;
            }
            if (Digit && char.IsDigit(ch))
            {
                return !Exclude;
            }
            if (Whitespace && char.IsWhiteSpace(ch))
            {
                return !Exclude;
            }

            foreach (char ch2 in _chars)
            {
                if (ch == ch2)
                {
                    return !Exclude;
                }
            }

            return Exclude;
        }

        public bool MatchesIgnoringCase(char ch)
        {
            if (Letter && char.IsLetter(ch))
            {
                return !Exclude;
            }
            if (Digit && char.IsDigit(ch))
            {
                return !Exclude;
            }
            if (Whitespace && char.IsWhiteSpace(ch))
            {
                return !Exclude;
            }

            char lower = char.ToLower(ch);
            foreach (char ch2 in _chars)
            {
                if (lower == char.ToLower(ch2))
                {
                    return !Exclude;
                }
            }

            return Exclude;
        }

        public override string ToString()
        {
            return "[" + ToUndelimitedString() + "]";
        }

        public string ToUndelimitedString()
        {
            StringBuilder sb = new StringBuilder();
            if (Exclude) sb.Append("^");
            if (Letter) sb.Append("\\l");
            if (Digit) sb.Append("\\d");
            if (Whitespace) sb.Append("\\s");
            var chs = new List<char>(_chars);
            chs.Sort();
            foreach (char ch in chs)
            {
                if (ch == ']' || ch == '\\' || ch == '[')
                {
                    sb.Append("\\");
                    sb.Append(ch.ToString());
                }
                else if (ch == '\t') sb.Append("\\t");
                else if (ch == '\r') sb.Append("\\r");
                else if (ch == '\n') sb.Append("\\n");
                else sb.Append(ch.ToString());
            }

            return sb.ToString();
        }

        public bool Intersects(CharClass cc)
        {
            CharClass intersection = Intersection(this, cc);

            if (intersection.GetAllChars().Length > 0 && !intersection.Exclude)
            {
                return true;
            }

            return false;
        }

        public char[] GetAllChars()
        {
            HashSet<char> chs = new HashSet<char>(_chars);

            if (Letter) chs.UnionWith(LetterChars);
            if (Digit) chs.UnionWith(DigitChars);
            if (Whitespace) chs.UnionWith(WhitespaceChars);

            return chs.ToArray();
        }

        public int GetAllCharsCount()
        {
            int count = GetNonClassCharsCount();

            if (Letter) count += LetterChars.Length;
            if (Digit) count += DigitChars.Length;
            if (Whitespace) count += WhitespaceChars.Length;

            return count;
        }

        public char[] GetNonClassChars()
        {
            return (char[])_chars.Clone();
        }

        public int GetNonClassCharsCount()
        {
            return _chars.Length;
        }

        public CharClass GetIgnoreCase()
        {
            List<char> list = new List<char>(GetAllChars());
            HashSet<char> set = new HashSet<char>();

            set.UnionWith(list);
            foreach (char ch in list)
            {
                if (char.IsLetter(ch))
                {
                    set.Add(char.ToUpper(ch));
                    set.Add(char.ToLower(ch));
                }
            }

            return new CharClass(set.ToArray());
        }

        public static CharClass Union(CharClass a, CharClass b)
        {
            bool exclude = false;

            HashSet<char> achs = new HashSet<char>(a.GetAllChars());
            HashSet<char> bchs = new HashSet<char>(b.GetAllChars());
            HashSet<char> cchs = new HashSet<char>();

            if (a.Exclude && b.Exclude)
            {
                //!a || !b == !(a && b)

                exclude = true;

                cchs.UnionWith(achs.Intersect(bchs));
            }
            else if (a.Exclude)
            {
                //!a || b == !(a-b)
                exclude = true;
                cchs.UnionWith(achs.Except(bchs));
            }
            else if (b.Exclude)
            {
                //a || !b == !(b-a)
                exclude = true;
                cchs.UnionWith(bchs.Except(achs));
            }
            else
            {
                //a || b == a || b
                exclude = false;
                cchs.UnionWith(achs);
                cchs.UnionWith(bchs);
            }

            return new CharClass(cchs.ToArray(), exclude);
        }

        public static CharClass Intersection(CharClass a, CharClass b)
        {
            bool exclude = false;

            HashSet<char> achs = new HashSet<char>(a.GetAllChars());
            HashSet<char> bchs = new HashSet<char>(b.GetAllChars());
            HashSet<char> cchs = new HashSet<char>();

            if (a.Exclude && b.Exclude)
            {
                //!a && !b == !(a || b)

                exclude = true;
                cchs.UnionWith(achs.Union(bchs));
            }
            else if (a.Exclude)
            {
                //!a && b == (b-a)
                exclude = false;
                cchs.UnionWith(bchs.Except(achs));
            }
            else if (b.Exclude)
            {
                //a && !b == (a-b)
                exclude = false;
                cchs.UnionWith(achs.Except(bchs));
            }
            else
            {
                //a && b == a && b
                exclude = false;
                cchs.UnionWith(achs.Intersect(bchs));
            }

            return new CharClass(cchs.ToArray(), exclude);
        }

        public static CharClass FromUndelimitedCharClassText(string text)
        {
            bool exclude = false;
            bool letter = false;
            bool digit = false;
            bool space = false;

            int i = 0;
            HashSet<char> chars = new HashSet<char>();
            if (text[0] == '^')
            {
                exclude = true;
                i++;
            }
            for (; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '\\' && i < text.Length - 1)
                {
                    i++;
                    char ch2 = text[i];
                    if (ch2 == 'l' || ch2 == 'w')
                    {
                        letter = true;
                    }
                    else if (ch2 == 'd')
                    {
                        digit = true;
                    }
                    else if (ch2 == 's')
                    {
                        space = true;
                    }
                    else
                    {
                        if (ch2 == 'r') ch2 = '\r';
                        if (ch2 == 'n') ch2 = '\n';
                        if (ch2 == 't') ch2 = '\t';

                        chars.Add(ch2);
                    }
                }
                else
                {
                    chars.Add(ch);
                }
            }

            if (letter)
            {
                chars.UnionWith(CharClass.LetterChars);
            }
            if (digit)
            {
                chars.UnionWith(CharClass.DigitChars);
            }
            if (space)
            {
                chars.UnionWith(CharClass.WhitespaceChars);
            }

            return new CharClass(chars.ToArray(), exclude);
        }
    }
}
