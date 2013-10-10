using System;

namespace MetaphysicsIndustries.Giza
{
    public struct Token
    {
        public Token(Definition definition=null, int startIndex=0, string value="", int indexOfNextToken=-1)
        {
            if (indexOfNextToken < 0)
            {
                indexOfNextToken = startIndex + (value ?? "").Length;
            }

            Definition = definition;
            StartIndex = startIndex;
            Value = value;
            IndexOfNextToken = indexOfNextToken;
        }

        public Definition Definition;
        public int StartIndex;
        public string Value;
        public int IndexOfNextToken;
    }
}

