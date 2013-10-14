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

        public InputPosition GetPosition(int index)
        {
            int line = 1;
            int column = 1;

            int i;
            for (i = 0; i < index; i++)
            {
                char ch = this[i];

                if (ch == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }

            return new InputPosition(index, line, column);
        }
    }
}

