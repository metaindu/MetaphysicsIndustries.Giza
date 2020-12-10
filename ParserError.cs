
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

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
                if (ErrorType == InvalidInputElement)
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
        public static readonly ErrorType InvalidInputElement =  new ErrorType(name:"InvalidInputElement",   descriptionFormat:"InvalidInputElement"   );
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
                var expectedNode = ExpectedNodes.First();
                if (expectedNode is DefRefNode)
                {
                    expect = string.Format("Expected {0}.", ((DefRefNode)expectedNode).DefRef.Name);
                }
                else //expectedNode is CharNode
                {
                    expect = string.Format("Expected [{0}].", ((CharNode)expectedNode).CharClass.ToUndelimitedString());
                }
            }

            return expect;
        }
    }
}

