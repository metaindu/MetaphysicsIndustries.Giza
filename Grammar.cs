using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public Grammar()
        {
            _definitions = new GrammarDefinitionParentChildrenCollection(this);
        }

        private GrammarDefinitionParentChildrenCollection _definitions;
        public GrammarDefinitionParentChildrenCollection Definitions
        {
            get { return _definitions; }
        }
    }
}

