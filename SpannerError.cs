using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class SpannerError : Error
    {
        public static readonly ErrorType InvalidCharacter =     new ErrorType(name:"InvalidCharacter",      descriptionFormat:"InvalidCharacter"      );
        public static readonly ErrorType UnexpectedEndOfInput = new ErrorType(name:"UnexpectedEndOfInput",  descriptionFormat:"UnexpectedEndOfInput"  );
        public static readonly ErrorType ExcessRemainingInput = new ErrorType(name:"ExcessRemainingInput",  descriptionFormat:"ExcessRemainingInput"  );

        public string DescriptionString = string.Empty;
        public char OffendingCharacter;
        public InputPosition Position;
        public Node PreviousNode;
        public Definition ExpectedDefinition;
        public IEnumerable<Node> ExpectedNodes;

        public override string Description
        {
            get { return DescriptionString; }
        }
    }
}

