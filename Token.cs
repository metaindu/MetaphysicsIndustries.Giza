using System;

namespace MetaphysicsIndustries.Giza
{
    public struct Token
    {
        public Token(Definition definition=null, int startIndex=0, string value="", int indexOfNextTokenization=-1)
        {
            if (indexOfNextTokenization < 0)
            {
                indexOfNextTokenization = startIndex + (value ?? "").Length;
            }

            Definition = definition;
            StartIndex = startIndex;
            Value = value;
            IndexOfNextTokenization = indexOfNextTokenization;
        }

        public Definition Definition;
        public int StartIndex;
        public string Value;
        public int IndexOfNextTokenization;
    }
}

