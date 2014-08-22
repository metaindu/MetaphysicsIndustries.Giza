using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class CharNode : Node
    {
        public CharNode(char ch)
            : this(new CharClass(ch))
        {
        }
        public CharNode(char ch, string tag)
            : this(new CharClass(ch), tag)
        {
        }
        public CharNode(CharClass cc)
            : this(cc, cc.ToUndelimitedString())
        {
        }
        public CharNode(CharClass cc, string tag)
            : base(tag)
        {
            if (cc == null) throw new ArgumentNullException("cc");
            _charClass = cc;
        }

        CharClass _charClass;
        public CharClass CharClass
        {
            get { return _charClass; }
        }

        public bool Matches(char ch)
        {
            if (ParentDefinition.IgnoreCase)
            {
                return CharClass.MatchesIgnoringCase(ch);
            }
            else
            {
                return CharClass.Matches(ch);
            }
        }
        public static CharNode[] FromString(string text)
        {
            return FromString(text, text);
        }

        public static CharNode[] FromString(string text, string tag)
        {
            List<CharNode> list = new List<CharNode>();

            int i = 0;
            foreach (char ch in text)
            {
                list.Add(new CharNode(ch, tag));// + "_" + i.ToString()));
                i++;
            }
            for (i = 1; i < list.Count; i++)
            {
                list[i - 1].NextNodes.Add(list[i]);
            }

            return list.ToArray();
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2}", ID, CharClass, Tag);
        }
    }
    
}
