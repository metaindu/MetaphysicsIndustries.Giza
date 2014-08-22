using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct InputChar : IInputElement
    {
        public InputChar(char value, InputPosition position)
        {
            Value = value;
            Position = position;
        }

        public readonly char Value;
        public readonly InputPosition Position;

        #region IInputElement implementation
        string IInputElement.Value { get { return Value.ToString(); } }
        InputPosition IInputElement.Position { get { return Position; } }
        int IInputElement.IndexOfNextElement { get { return Position.Index + 1; } }
        #endregion
    }
}

