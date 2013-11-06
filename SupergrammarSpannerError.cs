using System;

namespace MetaphysicsIndustries.Giza
{
    
    public class SupergrammarSpannerError : Error
    {
        public static readonly ErrorType NoValidSpans =         new ErrorType(name:"NoValidSpans",          descriptionFormat:"There are no valid spans of the grammar.");
        public static readonly ErrorType MultipleValidSpans =   new ErrorType(name:"MultipleValidSpans",    descriptionFormat:"There are more than one valid span of the grammar ({0}).");

        public override string Description
        {
            get
            {
                if (ErrorType == MultipleValidSpans)
                {
                    return string.Format(ErrorType.DescriptionFormat, NumSpans);
                }
                else
                {
                    return base.Description;
                }
            }
        }

        public int NumSpans = 0;
    }
}

