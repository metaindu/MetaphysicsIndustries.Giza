using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public struct TokenizationInfo
    {
        public bool EndOfInput;
        public InputPosition EndOfInputPosition;
        public IEnumerable<Token> Tokens;
        public List<Error> Errors;
    }

}

