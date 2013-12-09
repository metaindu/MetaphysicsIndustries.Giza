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
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            var tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            return Parse(tokenSource, errors);
        }
        public NodeMatch[] Match(CharacterSource input, List<Error> errors, out bool endOfInput, out InputPosition endOfInputPosition, bool mustUseAllInput=true, int startIndex=0)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            var tokenSource = new Tokenizer(_definition.ParentGrammar, input);

            endOfInput = false;
            endOfInputPosition = new InputPosition();

            return Match(tokenSource, errors);
        }

        protected override bool IsBranchTip(NodeMatch cur)
        {
            return (cur.Node is CharNode);
        }
    }
}

