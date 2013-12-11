using System;

namespace MetaphysicsIndustries.Giza
{
    public abstract class SubExpression : ExpressionItem
    {
        protected SubExpression(string tag="", bool isSkippable=false, bool isRepeatable=false)
            : base(isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            Tag = tag;
        }

        public string Tag = string.Empty;
    }
}

