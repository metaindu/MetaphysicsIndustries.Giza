using System;

namespace MetaphysicsIndustries.Giza
{
    public abstract class ExpressionItem
    {
        protected ExpressionItem(bool isSkippable=false, bool isRepeatable=false)
        {
            IsSkippable = isSkippable;
            IsRepeatable = isRepeatable;
        }

        public bool IsSkippable;
        public bool IsRepeatable;
    }
}

