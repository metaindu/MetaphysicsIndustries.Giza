using System;

namespace MetaphysicsIndustries.Giza
{
    public struct InputChar
    {
        public InputChar(char value, InputPosition position)
        {
            Value = value;
            Position = position;
        }

        public readonly char Value;
        public readonly InputPosition Position;
    }
}

