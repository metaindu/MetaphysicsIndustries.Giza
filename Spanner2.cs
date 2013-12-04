using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class Spanner2 : ParserBase
    {
        public Spanner2(Definition definition)
            : base(definition)
        {
        }

        public Span[] Process(CharacterSource input, ICollection<Error> errors)
        {
            return Parse(input, errors);
        }
        public NodeMatch[] Match(CharacterSource input, List<Error> errors, out bool endOfInput, out InputPosition endOfInputPosition, bool mustUseAllInput=true, int startIndex=0)
        {
            endOfInput = false;
            endOfInputPosition = new InputPosition();

            return Match(input, errors);
        }
    }
}

