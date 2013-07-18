using System;

namespace MetaphysicsIndustries.Giza
{
    public class Error
    {
        public ErrorType ErrorType;

        public bool IsWarning
        {
            get { return ErrorType.IsWarning; }
        }

        public virtual string Description
        {
            get { return ErrorType.Description; }
        }
    }
}

