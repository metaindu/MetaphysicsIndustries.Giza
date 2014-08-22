using System;

namespace MetaphysicsIndustries.Giza
{
    public class CharClassSubExpression : SubExpression
    {
        public CharClassSubExpression(CharClass charClass=null,
                                      string tag="",
                                      bool isSkippable=false,
                                      bool isRepeatable=false)
            : base(tag: tag, isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            CharClass = charClass;
        }

        public CharClass CharClass;
    }
}

