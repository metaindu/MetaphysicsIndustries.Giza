
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
        public DefinitionExpression[] GetExpressions(string input, List<Error> errors)
        {
            Supergrammar supergrammar = new Supergrammar();
            Spanner spanner = new Spanner(supergrammar.def_grammar);
            Span[] s2 = spanner.Process(input.ToCharacterSource(), errors);

            if (errors.Count > 0)
            {
                return null;
            }

            if (s2.Length < 1)
            {
                errors.Add(new SupergrammarSpannerError {
                    ErrorType = SupergrammarSpannerError.NoValidSpans,
                });
                return null;
            }

            if (s2.Length > 1)
            {
                errors.Add(new SupergrammarSpannerError {
                    ErrorType = SupergrammarSpannerError.MultipleValidSpans,
                    NumSpans = s2.Length,
                });
                return null;
            }

            ExpressionBuilder eb = new ExpressionBuilder();
            DefinitionExpression[] dis = eb.BuildExpressions(supergrammar, s2[0]);
            return dis;
        }

        public Grammar GetGrammar(string input, List<Error> errors)
        {
            DefinitionExpression[] dis = GetExpressions(input, errors);

            if (errors.Count > 0 || dis == null)
            {
                return null;
            }

            DefinitionBuilder db = new DefinitionBuilder();
            Definition[] defs = db.BuildDefinitions(dis);

            Grammar grammar = new Grammar();
            grammar.Definitions.AddRange(defs);

            return grammar;
        }
    }
}


