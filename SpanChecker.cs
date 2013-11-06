using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class SpanChecker
    {
        public List<Error> CheckSpan(Span span, Grammar grammar)
        {
            List<Error> errors = new List<Error>();

            CheckSpan(span, grammar, new Set<Span>(), new Set<Span>(), errors);

            return errors;
        }

        void CheckSpan(Span span, Grammar grammar, Set<Span> visited, Set<Span> ancestorSpans, List<Error> errors)
        {
            Definition spandef = (span.Node as DefRefNode).DefRef;

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

