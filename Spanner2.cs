using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class Spanner2 : ParserBase<InputChar>
    {
        public Spanner2(Definition definition)
            : base(definition)
        {
        }

        public Span[] Process(CharacterSource input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            return Parse(input, errors);
        }
        public NodeMatch<InputChar>[] Match(CharacterSource input, List<Error> errors, out bool endOfInput, out InputPosition endOfInputPosition, bool mustUseAllInput=true, int startIndex=0)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            endOfInput = false;
            endOfInputPosition = new InputPosition();

            return Match(input, errors);
        }

        protected override bool IsBranchTip(NodeMatch<InputChar> cur)
        {
            return (cur.Node is CharNode);
        }

        protected override bool BranchTipMatchesInputElement(NodeMatch<InputChar> branchTip, InputChar inputElement)
        {
            throw new NotImplementedException();
            //return (branchTip.Node as CharNode).Matches(inputElement.Value);
        }
    }
}

