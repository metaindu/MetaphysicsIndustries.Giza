using System;

namespace MetaphysicsIndustries.Giza
{
    public class LiteralSubExpression : SubExpression
    {
        public LiteralSubExpression(string value="",
                                    string tag="",
                                    bool isSkippable=false,
                                    bool isRepeatable=false)
            : base(tag: tag, isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            Value = value;
        }

        public string Value;
    }
}

