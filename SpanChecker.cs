using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class SpanChecker
    {
        public enum ErrorType
        {
            BadStartingNode,
            BadEndingNode,
            BadFollow,
            NodeInWrongDefinition,
            SpanHasNoSubspans,
            CycleInSubspans,
            SpanReused,
        }

        public struct ScError
        {
            public ErrorType ErrorType;
            public Span Span;
        }

        public List<ScError> CheckSpan(Span span, Grammar grammar)
        {
            return CheckSpan(span, grammar, new Set<Span>{span}, null);
        }

        class SpanStack
        {
            public Span Span;
            public SpanStack Parent;
        }

         List<ScError> CheckSpan(Span span, Grammar grammar, Set<Span> visited, SpanStack stack)
        {
            Definition spandef = (span.Node as DefRefNode).DefRef;
            List<ScError> errors = new List<ScError>();

            SpanStack stack2 = stack;
            while (stack2 != null)
            {
                if (span == stack2.Span)
                {
                    errors.Add(new ScError{
                        ErrorType=ErrorType.CycleInSubspans,
                        Span=span,
                    });
                    break;
                }
                else
                {
                    stack2 = stack2.Parent;
                }
            }

            Span first = span.Subspans.FirstOrDefault();
            if (first != null)
            {
                if (spandef != null &&
                    !spandef.StartNodes.Contains(first.Node))
                {
                    errors.Add(new ScError{
                        ErrorType=ErrorType.BadStartingNode,
                        Span=first,
                    });
                }

                stack2 = new SpanStack { Span=span, Parent=stack };
                int i;
                for (i = 0; i < span.Subspans.Count; i++)
                {
                    Span next = span.Subspans[i];

                    if (i > 0)
                    {
                        Span prev = span.Subspans[i - 1];
                        if (!prev.Node.NextNodes.Contains(next.Node))
                        {
                            errors.Add(new ScError{
                                ErrorType=ErrorType.BadFollow,
                                Span=next,
                            });
                        }
                    }

                    if (visited.Contains(next))
                    {
                        errors.Add(new ScError{
                            ErrorType=ErrorType.SpanReused,
                            Span=next,
                        });
                    }
                    visited.Add(next);

                    if (next.Node.ParentDefinition != spandef)
                    {
                        errors.Add(new ScError{
                            ErrorType=ErrorType.NodeInWrongDefinition,
                            Span=next,
                        });
                    }

                    if (next.Subspans.Count > 0)
                    {
                        errors.AddRange(CheckSpan(next, grammar, visited, stack2));
                    }
                }

                if (spandef != null &&
                    !spandef.EndNodes.Contains(span.Subspans.Last().Node))
                {
                    errors.Add(new ScError{
                        ErrorType=ErrorType.BadEndingNode,
                        Span=span.Subspans.Last(),
                    });
                }
            }
            else
            {
                if (span.Node is DefRefNode)
                {
                    errors.Add(new ScError{
                        ErrorType=ErrorType.SpanHasNoSubspans,
                        Span=span,
                    });
                }
            }

            return errors;
        }
    }
}

