using System;

namespace MetaphysicsIndustries.Giza
{
    public class CharacterSource
    {
        public CharacterSource(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public char this [ int index ]
        {
            get { return Value[index]; }
        }

        public int Length { get { return Value.Length; } }
    }
}

