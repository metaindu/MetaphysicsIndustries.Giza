using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;

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

            Set<char> chs = new Set<char>(chars);

            if (_letter || Collection.ContainsAll(chs, LetterChars))
            {
                _letter = true;
                chs.RemoveRange(LetterChars);
            }

            if (_digit || Collection.ContainsAll(chs, DigitChars))
            {
                _digit = true;
                chs.RemoveRange(DigitChars);
            }

            if (_whitespace || Collection.ContainsAll(chs, WhitespaceChars))
            {
                _whitespace = true;
                chs.RemoveRange(WhitespaceChars);
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
            foreach (char ch in _chars)
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
            Set<char> chs = new Set<char>(_chars);

            if (Letter) chs.AddRange(LetterChars);
            if (Digit) chs.AddRange(DigitChars);
            if (Whitespace) chs.AddRange(WhitespaceChars);

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
            Set<char> set = new Set<char>();

            set.AddRange(list);
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

            Set<char> achs = new Set<char>(a.GetAllChars());
            Set<char> bchs = new Set<char>(b.GetAllChars());
            Set<char> cchs = new Set<char>();

            if (a.Exclude && b.Exclude)
            {
                //!a || !b == !(a && b)

                exclude = true;

                cchs.AddRange(Set<char>.Intersection(achs, bchs));
            }
            else if (a.Exclude)
            {
                //!a || b == !(a-b)
                exclude = true;
                cchs.AddRange(Set<char>.Difference(achs.ToArray(), bchs.ToArray()));
            }
            else if (b.Exclude)
            {
                //a || !b == !(b-a)
                exclude = true;
                cchs.AddRange(Set<char>.Difference(bchs.ToArray(), achs.ToArray()));
            }
            else
            {
                //a || b == a || b
                exclude = false;
                cchs.AddRange(achs);
                cchs.AddRange(bchs);
            }

            return new CharClass(cchs.ToArray(), exclude);
        }

        public static CharClass Intersection(CharClass a, CharClass b)
        {
            bool exclude = false;

            Set<char> achs = new Set<char>(a.GetAllChars());
            Set<char> bchs = new Set<char>(b.GetAllChars());
            Set<char> cchs = new Set<char>();

            if (a.Exclude && b.Exclude)
            {
                //!a && !b == !(a || b)

                exclude = true;
                cchs.AddRange(Set<char>.Union(achs, bchs));
            }
            else if (a.Exclude)
            {
                //!a && b == (b-a)
                exclude = false;
                cchs.AddRange(Set<char>.Difference(bchs.ToArray(), achs.ToArray()));
            }
            else if (b.Exclude)
            {
                //a && !b == (a-b)
                exclude = false;
                cchs.AddRange(Set<char>.Difference(achs.ToArray(), bchs.ToArray()));
            }
            else
            {
                //a && b == a && b
                exclude = false;
                cchs.AddRange(Set<char>.Intersection(achs, bchs));
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
            Set<char> chars = new Set<char>();
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
                chars.AddRange(CharClass.LetterChars);
            }
            if (digit)
            {
                chars.AddRange(CharClass.DigitChars);
            }
            if (space)
            {
                chars.AddRange(CharClass.WhitespaceChars);
            }

            return new CharClass(chars.ToArray(), exclude);
        }
    }
}
