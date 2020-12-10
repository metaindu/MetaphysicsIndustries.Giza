
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


