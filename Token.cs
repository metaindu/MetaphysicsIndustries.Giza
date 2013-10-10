using System;

namespace MetaphysicsIndustries.Giza
{
    public struct Token
    {
        public Token(Definition definition=null, int startIndex=0, string value="")
        {
            Definition = definition;
            StartIndex = startIndex;
            Value = value;
        }

        public Definition Definition;
        public int StartIndex;
        public string Value;
    }
}

