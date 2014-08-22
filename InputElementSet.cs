using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class InputElementSet<T>
        where T : IInputElement
    {
        public IEnumerable<T> InputElements;
        public bool EndOfInput;
        public InputPosition EndOfInputPosition;
        public List<Error> Errors = new List<Error>();
    }

}

