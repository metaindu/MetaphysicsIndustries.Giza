
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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


namespace MetaphysicsIndustries.Giza
{
    public class SpanChecker
    {
        public List<Error> CheckSpan(Span span, NGrammar grammar)
        {
            List<Error> errors = new List<Error>();

            CheckSpan(span, grammar, new HashSet<Span>(), new HashSet<Span>(), errors);

            return errors;
        }

        void CheckSpan(Span span, NGrammar grammar, HashSet<Span> visited, 
            HashSet<Span> ancestorSpans, List<Error> errors)
        {
            var spandef = (span.Node as DefRefNode)?.DefRef;

            if (ancestorSpans.Contains(span))
            {
                errors.Add(new SpanError{
                    ErrorType=SpanError.CycleInSubspans,
                    Span=span,
                });
                return;
            }

            Span first = span.Subspans.FirstOrDefault();
            if (first != null)
            {
                if (spandef != null &&
                    !spandef.StartNodes.Contains(first.Node))
                {
                    errors.Add(new SpanError{
                        ErrorType=SpanError.BadStartingNode,
                        Span=first,
                    });
                }

                ancestorSpans.Add(span);
                int i;
                for (i = 0; i < span.Subspans.Count; i++)
                {
                    Span next = span.Subspans[i];

                    if (i > 0)
                    {
                        Span prev = span.Subspans[i - 1];
                        if (!prev.Node.NextNodes.Contains(next.Node))
                        {
                            errors.Add(new SpanError{
                                ErrorType=SpanError.BadFollow,
                                Span=next,
                            });
                        }
                    }

                    if (next.Node.ParentDefinition != spandef)
                    {
                        errors.Add(new SpanError{
                            ErrorType=SpanError.NodeInWrongDefinition,
                            Span=next,
                        });
                    }

                    if (next.Subspans.Count > 0)
                    {
                        CheckSpan(next, grammar, visited, ancestorSpans, errors);
                    }
                }
                ancestorSpans.Remove(span);

                if (spandef != null &&
                    !spandef.EndNodes.Contains(span.Subspans.Last().Node))
                {
                    errors.Add(new SpanError{
                        ErrorType=SpanError.BadEndingNode,
                        Span=span.Subspans.Last(),
                    });
                }
            }
            else
            {
                if (span.Node is DefRefNode)
                {
                    errors.Add(new SpanError{
                        ErrorType=SpanError.SpanHasNoSubspans,
                        Span=span,
                    });
                }
            }
        }
    }
}

