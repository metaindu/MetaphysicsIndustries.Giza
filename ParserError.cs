using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class ParserError<T> : ParserError
        where T : IInputElement
    {
        public T OffendingInputElement;

        public override string Description
        {
            get
            {
                if (ErrorType == InvalidToken)
                {
                    return string.Format(
                        "Invalid token '{0}' at position {1},{2} (index {4}). {3}",
                        OffendingInputElement.Value,
                        Line,
                        Column,
                        GetExpectedNodesString(),
                        Index);
                }
                else if (ErrorType == ExcessRemainingInput)
                {
                    return string.Format(
                        "Excess remaining input after parsing had completed. " +
                        "Got token '{0}' at position {1},{2} (index {4}). {3}",
                        OffendingInputElement.Value,
                        Line,
                        Column,
                        GetExpectedNodesString(),
                        Index);
                }
                else
                {
                    return base.Description;
                }
            }
        }
    }
    public abstract class ParserError : Error
    {
        public static readonly ErrorType InvalidToken =         new ErrorType(name:"InvalidToken",          descriptionFormat:"InvalidToken"          );
        public static readonly ErrorType UnexpectedEndOfInput = new ErrorType(name:"UnexpectedEndOfInput",  descriptionFormat:"UnexpectedEndOfInput"  );
        public static readonly ErrorType ExcessRemainingInput = new ErrorType(name:"ExcessRemainingInput",  descriptionFormat:"ExcessRemainingInput"  );

        public InputPosition Position;
        public int Index { get { return Position.Index; } }
        public int Line { get { return Position.Line; } }
        public int Column { get { return Position.Column; } }
        public Node LastValidMatchingNode;
        public IEnumerable<Node> ExpectedNodes;

        public override string Description
        {
            get
            {
                if (ErrorType == UnexpectedEndOfInput)
                {
                    return string.Format(
                        "Unexpected end of input at position {0},{1} (index {2}). {3}",
                        Line,
                        Column,
                        Index,
                        GetExpectedNodesString());
                }

                return base.Description;
            }
        }

        protected string GetExpectedNodesString()
        {
            if (ExpectedNodes == null) return "";

            string expect = "";
            if (ExpectedNodes.Count() > 1)
            {
                var expects = new List<string>();
                foreach (var enode in ExpectedNodes)
                {
                    expects.Add(((DefRefNode)ExpectedNodes.First()).DefRef.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("Expected ");
                int i;
                for (i = 1; i < expects.Count; i++)
                {
                    sb.Append(expects[i - 1]);
                    sb.Append(", ");
                }
                sb.Append("or ");
                sb.Append(expects.Last());
                expect = sb.ToString();
            }
            else if (ExpectedNodes.Count() > 0)
            {
                expect = string.Format("Expected {0}.", ((DefRefNode)ExpectedNodes.First()).DefRef.Name);
            }

            return expect;
        }
    }
}

