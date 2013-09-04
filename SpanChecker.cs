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

            public static readonly ErrorType BadStartingNode =          new ErrorType(name:"BadStartingNode",       descriptionFormat:"BadStartingNode"       );
            public static readonly ErrorType BadEndingNode =            new ErrorType(name:"BadEndingNode",         descriptionFormat:"BadEndingNode"         );
            public static readonly ErrorType BadFollow =                new ErrorType(name:"BadFollow",             descriptionFormat:"BadFollow"             );
            public static readonly ErrorType NodeInWrongDefinition =    new ErrorType(name:"NodeInWrongDefinition", descriptionFormat:"NodeInWrongDefinition" );
            public static readonly ErrorType SpanHasNoSubspans =        new ErrorType(name:"SpanHasNoSubspans",     descriptionFormat:"SpanHasNoSubspans"     );
            public static readonly ErrorType CycleInSubspans =          new ErrorType(name:"CycleInSubspans",       descriptionFormat:"CycleInSubspans"       );
        }

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
                errors.Add(new ScError{
                    ErrorType=ScError.CycleInSubspans,
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
                        CheckSpan(next, grammar, visited, ancestorSpans, errors);
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
        }
    }
}

