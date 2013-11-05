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

        public InputPosition CurrentPosition = new InputPosition(index: 0, line: 1, column: 1);

        public void SetCurrentIndex(int index)
        {
            if (index < CurrentPosition.Index)
            {
                CurrentPosition = GetPosition(index);
            }
            else if (index > CurrentPosition.Index)
            {
                if (index >= Value.Length)
                {
                    throw new NotImplementedException();
                }

                for (; CurrentPosition.Index < index; CurrentPosition.Index++)
                {
                    char ch = this[CurrentPosition.Index];

                    if (ch == '\n')
                    {
                        CurrentPosition.Line++;
                        CurrentPosition.Column = 1;
                    }
                    else
                    {
                        CurrentPosition.Column++;
                    }
                }
            }
        }

        public InputChar CurrentValue
        {
            get { return new InputChar(this[CurrentPosition.Index], CurrentPosition); }
        }

        public InputChar GetNextValue()
        {
            SetCurrentIndex(CurrentPosition.Index + 1);
            return CurrentValue;
        }
    }
}

