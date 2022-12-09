
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
