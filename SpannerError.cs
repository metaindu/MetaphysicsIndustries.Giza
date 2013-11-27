using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Text;
using System.Linq;

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
            get
            {
                if (ErrorType == InvalidCharacter)
                {
                    Set<char> vowels = new Set<char> {
                        'a', 'e', 'i', 'o', 'u',
                        'A', 'E', 'I', 'O', 'U',
                    };
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("Invalid character '{0}' at ({1},{2})", OffendingCharacter, Position.Line, Position.Column);

                    if (PreviousNode != null)
                    {
                        string an = "a";
                        string after = "";

                        if (PreviousNode is CharNode)
                        {
                            after = GetDescriptionsOfCharClass((PreviousNode as CharNode).CharClass)[0];
                        }
                        else
                        {
                            after = (PreviousNode as DefRefNode).DefRef.Name;
                        }

                        if (vowels.Contains(after[0]))
                        {
                            an = "an";
                        }
                        sb.AppendFormat(", after {0} {1}: expected ", an, after);
                    }

                    if (PreviousNode == null)
                    {
                        //failed to start

                        string an = "a";
                        if (vowels.Contains(ExpectedDefinition.Name[0]))
                        {
                            an = "an";
                        }

                        sb.AppendFormat(": {0} {1} must start with ", an, ExpectedDefinition.Name);

                    }

                    if (ExpectedNodes != null)
                    {
                        CharClass expectedChars = new CharClass(new char[0]);
                        Set<Definition> expectedDefs = new Set<Definition>();
                        foreach (Node node in ExpectedNodes)
                        {
                            if (node is CharNode)
                            {
                                expectedChars =
                                    CharClass.Union(
                                        (node as CharNode).CharClass,
                                        expectedChars);
                            }
                            else
                            {
                                expectedDefs.Add((node as DefRefNode).DefRef);
                            }
                        }

                        List<string> expects = new List<string>();

                        foreach (Definition expdef in expectedDefs)
                        {
                            expects.Add(expdef.Name);
                        }

                        expects.AddRange(GetDescriptionsOfCharClass(expectedChars));

                        int i;
                        for (i = 2; i < expects.Count; i++)
                        {
                            sb.AppendFormat("{0}, ", expects[i-2]);
                        }
                        if (expects.Count > 1)
                        {
                            sb.Append(expects[expects.Count - 2]);
                            if (expects.Count > 2)
                            {
                                sb.Append(",");
                            }
                            sb.Append(" or ");
                        }
                        if (expects.Count > 0)
                        {
                            sb.Append(expects.Last());
                        }
                        else
                        {
                            sb.AppendLine("[We don't expect anything?]");
                        }
                    }

                    return sb.ToString();
                }
                else
                {
                    return DescriptionString;
                }
            }
        }

        public static List<string> GetDescriptionsOfCharClass(CharClass expectedChars)
        {
            List<string> expects2 = new List<string>();
            if (!expectedChars.Exclude &&
                !expectedChars.Digit &&
                !expectedChars.Letter &&
                !expectedChars.Whitespace &&
                expectedChars.GetNonClassCharsCount() > 0 &&
                expectedChars.GetNonClassCharsCount() <= 3)
            {
                // only a few characters - list each of them in quotes
                foreach (char ch in expectedChars.GetNonClassChars())
                {
                    expects2.Add(string.Format("'{0}'", ch.ToString()));
                }
            }
            else if (expectedChars.Digit ||
                expectedChars.Letter ||
                expectedChars.Whitespace ||
                expectedChars.GetNonClassCharsCount() > 0)
            {
                // treat as char class
                expects2.Add(string.Format("a character that matches {0}", expectedChars));
            }
            else
            {
                // empty char class - don't do anything
            }
            return expects2;
        }
    }
}

