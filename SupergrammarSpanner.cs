
/*****************************************************************************
 *                                                                           *
 *  SupergrammarSpanner.cs                                                   *
 *  6 July 2010                                                              *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2010 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class SupergrammarSpanner
    {
        public DefinitionExpression[] GetExpressions(string input, out string error)
        {
            Supergrammar supergrammar = new Supergrammar();
            Spanner spanner = new Spanner();
            Span[] s2 = spanner.Process(supergrammar, "grammar", input, out error);

            if (!string.IsNullOrEmpty(error))
            {
                return null;
            }

            if (s2.Length < 1)
            {
                error = "There are no valid spans of the grammar.";
                return null;
            }

            if (s2.Length > 1)
            {
                error = string.Format("There are more than one valid span of the grammar ({0}).", s2.Length);
                return null;
            }

            ExpressionBuilder eb = new ExpressionBuilder();
            DefinitionExpression[] dis = eb.BuildExpressions(supergrammar, s2[0]);
            return dis;
        }

        public Grammar GetGrammar(string input, out string error)
        {
            DefinitionExpression[] dis = GetExpressions(input, out error);

            if (string.IsNullOrEmpty(error) && dis != null)
            {
                DefinitionBuilder db = new DefinitionBuilder();
                Definition[] defs = db.BuildDefinitions(dis);

                Grammar grammar = new Grammar();
                grammar.Definitions.AddRange(defs);

                return grammar;
            }
            else
            {
                return null;
            }
        }
    }
}


