using System;

namespace MetaphysicsIndustries.Giza
{
    public struct Token
    {
        public Definition Definition;
        public int StartIndex;
        public int Length;

        public string Value;
        public void SetValueFromInput(string input)
        {
            Value = input.Substring(StartIndex, Length);
        }
    }
}

