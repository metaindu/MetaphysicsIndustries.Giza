using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class InputElementSet<T>
        where T : IInputElement
    {
        public bool EndOfInput;
        public InputPosition EndOfInputPosition;
        public IEnumerable<T> Tokens;
        public List<Error> Errors = new List<Error>();
    }

}

