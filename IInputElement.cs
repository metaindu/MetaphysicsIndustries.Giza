using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public interface IInputElement
    {
        string Value { get; }
        InputPosition Position { get; }
        int IndexOfNextElement { get; }
    }
}

