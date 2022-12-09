
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

namespace MetaphysicsIndustries.Giza
{
    public class CharacterSource : IInputSource<InputChar>
    {
        public CharacterSource(string value)
        {
            Value = value;
            CurrentPosition = new InputPosition(index: 0, line: 1, column: 1);
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

        public InputPosition CurrentPosition { get; set; }

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

                while (CurrentPosition.Index < index)
                {
                    char ch = this[CurrentPosition.Index];

                    var newpos = CurrentPosition;
                    if (ch == '\n')
                    {
                        newpos.Line++;
                        newpos.Column = 1;
                    }
                    else
                    {
                        newpos.Column++;
                    }
                    newpos.Index++;
                    CurrentPosition = newpos;
                }
            }
        }

        public InputElementSet<InputChar> Peek()
        {
            return new InputElementSet<InputChar> {
                EndOfInput = false,
                EndOfInputPosition = new InputPosition(-1),
                Errors = new List<Error>(),
                InputElements = new [] { new InputChar(this[CurrentPosition.Index], CurrentPosition) }
            };
        }

        public InputElementSet<InputChar> GetNextValue()
        {
            InputElementSet<InputChar> ies = Peek();
            SetCurrentIndex(CurrentPosition.Index + 1);
            return ies;
        }

        public bool IsAtEnd
        {
            get { return (CurrentPosition.Index >= Length); }
        }

        #region IInputSource implementation

        public InputElementSet<InputChar> GetInputAtLocation(int index)
        {
            return new InputElementSet<InputChar> {
                InputElements = new InputChar[] {
                    new InputChar(this[index], GetPosition(index))
                }
            };
        }

        #endregion
    }

    public static class CharacterSourceHelper
    {
        public static CharacterSource ToCharacterSource(this string s)
        {
            return new CharacterSource(s);
        }
    }
}

