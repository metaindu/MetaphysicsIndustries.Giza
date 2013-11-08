using System;

namespace MetaphysicsIndustries.Giza
{
    public class CharacterSource
    {
        public CharacterSource(string value)
        {
            Value = value;
        }

        public static implicit operator CharacterSource(string value)
        {
            return new CharacterSource(value);
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
                Logger.WriteLine("CharacterSource: Rewinding from {0} to {1}", CurrentPosition.Index, index);
                CurrentPosition = GetPosition(index);
            }
            else if (index > CurrentPosition.Index)
            {
                if (index >= Value.Length + 1)
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

        public InputChar Peek()
        {
            return new InputChar(this[CurrentPosition.Index], CurrentPosition);
        }

        public InputChar GetNextValue()
        {
            InputChar ch = Peek();
            SetCurrentIndex(CurrentPosition.Index + 1);
            return ch;
        }

        public bool IsAtEnd
        {
            get { return (CurrentPosition.Index >= Length); }
        }
    }
}

