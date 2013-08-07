using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class SpanChecker
    {
        public class ScError : Error
        {
            public Span Span;

            public static readonly ErrorType BadStartingNode =          new ErrorType() { Name="BadStartingNode",       Description="BadStartingNode"       };
            public static readonly ErrorType BadEndingNode =            new ErrorType() { Name="BadEndingNode",         Description="BadEndingNode"         };
            public static readonly ErrorType BadFollow =                new ErrorType() { Name="BadFollow",             Description="BadFollow"             };
            public static readonly ErrorType NodeInWrongDefinition =    new ErrorType() { Name="NodeInWrongDefinition", Description="NodeInWrongDefinition" };
            public static readonly ErrorType SpanHasNoSubspans =        new ErrorType() { Name="SpanHasNoSubspans",     Description="SpanHasNoSubspans"     };
            public static readonly ErrorType CycleInSubspans =          new ErrorType() { Name="CycleInSubspans",       Description="CycleInSubspans"       };
            public static readonly ErrorType SpanReused =               new ErrorType() { Name="SpanReused",            Description="SpanReused"            };
        }

        public List<ScError> CheckSpan(Span span, Grammar grammar)
        {
            return CheckSpan(span, grammar, new Set<Span>(), new Set<Span>());
        }

        List<ScError> CheckSpan(Span span, Grammar grammar, Set<Span> visited, Set<Span> ancestorSpans)
        {
            Definition spandef = (span.Node as DefRefNode).DefRef;
            List<ScError> errors = new List<ScError>();

            if (ancestorSpans.Contains(span))
            {
                errors.Add(new ScError{
                    ErrorType=ScError.CycleInSubspans,
                    Span=span,
                });
                return errors;
            }

            if (visited.Contains(span))
            {
                errors.Add(new ScError{
                    ErrorType=ScError.SpanReused,
                    Span=span,
                });
                return errors;
            }
            visited.Add(span);

            Span first = span.Subspans.FirstOrDefault();
            if (first != null)
            {
                if (spandef != null &&
                    !spandef.StartNodes.Contains(first.Node))
                {
                    errors.Add(new ScError{
                        ErrorType=ScError.BadStartingNode,
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
                            errors.Add(new ScError{
                                ErrorType=ScError.BadFollow,
                                Span=next,
                            });
                        }
                    }

                    if (next.Node.ParentDefinition != spandef)
                    {
                        errors.Add(new ScError{
                            ErrorType=ScError.NodeInWrongDefinition,
                            Span=next,
                        });
                    }

                    if (next.Subspans.Count > 0)
                    {
                        errors.AddRange(CheckSpan(next, grammar, visited, ancestorSpans));
                    }
                }
                ancestorSpans.Remove(span);

                if (spandef != null &&
                    !spandef.EndNodes.Contains(span.Subspans.Last().Node))
                {
                    errors.Add(new ScError{
                        ErrorType=ScError.BadEndingNode,
                        Span=span.Subspans.Last(),
                    });
                }
            }
            else
            {
                if (span.Node is DefRefNode)
                {
                    errors.Add(new ScError{
                        ErrorType=ScError.SpanHasNoSubspans,
                        Span=span,
                    });
                }
            }

            return errors;
        }
    }
}

