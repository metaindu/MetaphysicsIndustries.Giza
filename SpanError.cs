using System;

namespace MetaphysicsIndustries.Giza
{
    public class SpanError : Error
    {
        public Span Span;

        public static readonly ErrorType BadStartingNode =          new ErrorType(name:"BadStartingNode",       descriptionFormat:"BadStartingNode"       );
        public static readonly ErrorType BadEndingNode =            new ErrorType(name:"BadEndingNode",         descriptionFormat:"BadEndingNode"         );
        public static readonly ErrorType BadFollow =                new ErrorType(name:"BadFollow",             descriptionFormat:"BadFollow"             );
        public static readonly ErrorType NodeInWrongDefinition =    new ErrorType(name:"NodeInWrongDefinition", descriptionFormat:"NodeInWrongDefinition" );
        public static readonly ErrorType SpanHasNoSubspans =        new ErrorType(name:"SpanHasNoSubspans",     descriptionFormat:"SpanHasNoSubspans"     );
        public static readonly ErrorType CycleInSubspans =          new ErrorType(name:"CycleInSubspans",       descriptionFormat:"CycleInSubspans"       );
    }

}

