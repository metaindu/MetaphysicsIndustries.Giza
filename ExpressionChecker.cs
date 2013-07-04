using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class ExpressionChecker
    {
        public enum Error
        {
            ReusedDefintion,
            NullDefinition,
            ReusedExpressionOrItem,
            NullOrEmptyDefinitionName,
            NullDefinitionExpression,
            EmptyExpressionItems,
            NullExpressionItem,
            EmptyOrexprExpressionList,
            NullOrexprExpression,
            NullSubexprTag,
            NullOrEmptyDefrefName,
            NullOrEmptyLiteralValue,
            NullOrEmptyCharClass,
            DuplicateDefinitionName,
            AllItemsSkippable,
            SkippableOrexprExpressions,
            DefRefNameNotFound,

            TokenizedDirectiveInNonTokenizedGrammar,

            MixedTokenizedDirectives,
            ReferencedComment,
            NonTokenReferencesSubtoken,
            SubtokenReferencesNonToken,
            TokenReferencesNonToken,
            SubtokenReferencesToken,
        }

        public class InvalidDefinitionException : Exception
        {
            public Error Error;
            public DefinitionInfo DefinitionInfo;
            public int Index;
        }

        public class InvalidExpressionException : Exception
        {
            public Error Error;
            public Expression Expression;
            public ExpressionItem ExpressionItem;
            public DefinitionInfo DefinitionInfo;
            public int Index;
        }

        public struct ErrorInfo
        {
            public Error Error;
            public Expression Expression;
            public ExpressionItem ExpressionItem;
            public DefinitionInfo DefinitionInfo;
            public int Index;
        }

        public List<ErrorInfo> CheckDefinitionInfosForParsing(IEnumerable<DefinitionInfo> defs)
        {
            List<ErrorInfo> errors = CheckDefinitionInfos(defs);

            Dictionary<string, DefinitionInfo> defsByName = new Dictionary<string, DefinitionInfo>();
            foreach (var def in defs)
            {
                defsByName[def.Name] = def;
            }
            foreach (var def in defs)
            {
                if ((def.Directives.Contains(DefinitionDirective.Token) && 
                    def.Directives.Contains(DefinitionDirective.Subtoken)) ||
                    (def.Directives.Contains(DefinitionDirective.Comment) && 
                    def.Directives.Contains(DefinitionDirective.Subtoken)))
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.MixedTokenizedDirectives,
                        DefinitionInfo = def,
                    });
                }

                foreach (DefRefSubExpression defref in EnumerateDefRefs(def.Expression))
                {
                    if (!defsByName.ContainsKey(defref.DefinitionName)) continue;

                    DefinitionInfo target = defsByName[defref.DefinitionName];

                    if (target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.ReferencedComment,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }
                    if (!def.Directives.Contains(DefinitionDirective.Token) &&
                        !def.Directives.Contains(DefinitionDirective.Subtoken) &&
                        !def.Directives.Contains(DefinitionDirective.Comment) &&
                        target.Directives.Contains(DefinitionDirective.Subtoken))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.NonTokenReferencesSubtoken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }
                    if (def.Directives.Contains(DefinitionDirective.Subtoken) &&
                        !target.Directives.Contains(DefinitionDirective.Token) &&
                        !target.Directives.Contains(DefinitionDirective.Subtoken) &&
                        !target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.SubtokenReferencesNonToken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }
                    if (def.Directives.Contains(DefinitionDirective.Token) &&
                        !target.Directives.Contains(DefinitionDirective.Token) &&
                        !target.Directives.Contains(DefinitionDirective.Subtoken) &&
                        !target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.TokenReferencesNonToken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }
                    if (def.Directives.Contains(DefinitionDirective.Subtoken) &&
                        target.Directives.Contains(DefinitionDirective.Token))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.SubtokenReferencesToken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }

                }
            }

            return errors;
        }

        IEnumerable<DefRefSubExpression> EnumerateDefRefs(Expression expr)
        {
            foreach (var item in expr.Items)
            {
                if (item is DefRefSubExpression)
                {
                    yield return (item as DefRefSubExpression);
                }
                else if (item is OrExpression)
                {
                    foreach (var expr2 in (item as OrExpression).Expressions)
                    {
                        foreach (var defref in EnumerateDefRefs(expr2))
                        {
                            yield return defref;
                        }
                    }
                }
            }
        }

        public List<ErrorInfo> CheckDefinitionInfosForSpanning(IEnumerable<DefinitionInfo> defs)
        {
            List<ErrorInfo> errors = CheckDefinitionInfos(defs);

            foreach (DefinitionInfo def in defs)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Subtoken) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.TokenizedDirectiveInNonTokenizedGrammar,
                        DefinitionInfo = def,
                    });
                }
            }

            return errors;
        }

        public virtual List<ErrorInfo> CheckDefinitionInfos(IEnumerable<DefinitionInfo> defs)
        {
            if (defs == null) throw new ArgumentNullException("defs");

            List<ErrorInfo> errors = new List<ErrorInfo>();

            // Any Expression or ExpressionItem object can be used in the 
            // trees only once. This precludes reference cycles as well as 
            // ADGs. ADGs are not much of a problem, but reference cycles 
            // would cause the checker to go into an infinite loop if we 
            // weren't expecting it.
            Set<Expression> visitedExprs = new Set<Expression>();
            Set<ExpressionItem> visitedItems = new Set<ExpressionItem>();
            Set<DefinitionInfo> visitedDefs = new Set<DefinitionInfo>();
            int index = -1;
            List<string> defNames = new List<string>();
            foreach (DefinitionInfo def in defs)
            {
                index++;
                if (def == null)
                {
                    throw new InvalidDefinitionException {
                        Error = Error.NullDefinition,
                        Index = index,
                    };
                }

                if (visitedDefs.Contains(def))
                {
                    throw new InvalidDefinitionException {
                        Error = Error.ReusedDefintion,
                        DefinitionInfo = def,
                        Index = index,
                    };
                }
                visitedDefs.Add(def);

                if (string.IsNullOrEmpty(def.Name))
                {
                    throw new InvalidDefinitionException {
                        Error = Error.NullOrEmptyDefinitionName,
                        DefinitionInfo = def,
                    };
                }
                defNames.Add(def.Name);
            }

            foreach (DefinitionInfo def in defs)
            {
                if (def.Expression == null)
                {
                    throw new InvalidExpressionException {
                        Error = Error.NullDefinitionExpression,
                        DefinitionInfo = def,
                    };
                }
                else
                {
                    CheckExpression(def, def.Expression, defNames, visitedExprs, visitedItems, errors);
                }
            }

            CheckForDuplicateNames(defs, errors);

            return errors;
        }

        protected virtual void CheckExpression(DefinitionInfo def,
                             Expression expr,
                             List<string> defNames,
                             Set<Expression> visitedExprs,
                             Set<ExpressionItem> visitedItems,
                             List<ErrorInfo> errors)
        {
            if (visitedExprs.Contains(expr))
            {
                throw new InvalidExpressionException {
                    Error = Error.ReusedExpressionOrItem,
                    Expression = expr,
                    DefinitionInfo = def,
                };
            }
            visitedExprs.Add(expr);

            if (expr.Items == null || expr.Items.Count < 1)
            {
                errors.Add(new ErrorInfo {
                    Error = Error.EmptyExpressionItems,
                    Expression = expr,
                    DefinitionInfo = def,
                });
            }
            else
            {
                bool skippable = true;

                int index = 0;
                foreach (ExpressionItem item in expr.Items)
                {
                    if (item == null)
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.NullExpressionItem,
                            Expression = expr,
                            Index = index,
                            DefinitionInfo = def
                        });
                    }
                    else
                    {
                        CheckExpressionItem(def, item, defNames, visitedExprs, visitedItems, errors);
                        skippable = skippable && item.IsSkippable;
                    }

                    index++;
                }

                if (skippable && expr.Items.Count > 0)
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.AllItemsSkippable,
                        Expression = expr,
                        DefinitionInfo = def,
                    });
                }
            }
        }

        protected virtual void CheckExpressionItem(DefinitionInfo def,
                                 ExpressionItem item,
                                 List<string> defNames,
                                 Set<Expression> visitedExprs,
                                 Set<ExpressionItem> visitedItems,
                                 List<ErrorInfo> errors)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (visitedItems.Contains(item))
            {
                throw new InvalidExpressionException {
                    Error = Error.ReusedExpressionOrItem,
                    ExpressionItem = item,
                    DefinitionInfo = def,
                };
            }
            visitedItems.Add(item);

            if (item is OrExpression)
            {
                OrExpression orexor = (OrExpression)item;
                if (orexor.Expressions.Count < 1)
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.EmptyOrexprExpressionList,
                        DefinitionInfo = def,
                        ExpressionItem = item
                    });
                }
                else
                {
                    int index = 0;
                    foreach (Expression expr in orexor.Expressions)
                    {
                        if (expr == null)
                        {
                            errors.Add(new ErrorInfo {
                                Error = Error.NullOrexprExpression,
                                ExpressionItem = item,
                                Index = index,
                                DefinitionInfo = def,
                            });
                        }
                        else
                        {
                            CheckExpression(def, expr, defNames, visitedExprs, visitedItems, errors);
                        }
                        index++;
                    }
                }
            }
            else if (item is SubExpression)
            {
                if ((item as SubExpression).Tag == null)
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.NullSubexprTag,
                        ExpressionItem = item,
                        DefinitionInfo = def,
                    });
                }

                if (item is LiteralSubExpression)
                {
                    if (string.IsNullOrEmpty((item as LiteralSubExpression).Value))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.NullOrEmptyLiteralValue,
                            ExpressionItem = item,
                            DefinitionInfo = def,
                        });
                    }
                }
                else if (item is CharClassSubExpression)
                {
                    CharClass cc = (item as CharClassSubExpression).CharClass;

                    if (cc == null || cc.GetAllCharsCount() < 1)
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.NullOrEmptyCharClass,
                            ExpressionItem = item,
                            DefinitionInfo = def
                        });
                    }
                }
                else if (item is DefRefSubExpression)
                {
                    string name = (item as DefRefSubExpression).DefinitionName;
                    if (string.IsNullOrEmpty(name))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.NullOrEmptyDefrefName,
                            ExpressionItem = item,
                            DefinitionInfo = def,
                        });
                    }
                    else if (!defNames.Contains(name))
                    {
                        errors.Add(new ErrorInfo {
                            Error = Error.DefRefNameNotFound,
                            ExpressionItem = item,
                            DefinitionInfo = def,
                        });
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                        "Unknown SubExpression sub-type: {0}", 
                        item.GetType()));
                }
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unknown ExpressionItem sub-type: {0}", 
                        item.GetType()));
            }
        }

        protected virtual void CheckForDuplicateNames(IEnumerable<DefinitionInfo> defs, List<ErrorInfo> errors)
        {
            DefinitionInfo[] defs2 = defs.ToArray();
            Dictionary<string, Set<int>> indexesByName = new Dictionary<string, Set<int>>();
            int i;
            for (i = 0; i < defs2.Length; i++)
            {
                string name = defs2[i].Name;
                if (!indexesByName.ContainsKey(name))
                {
                    indexesByName[name] = new Set<int>();
                }

                indexesByName[name].Add(i);
                if (indexesByName[name].Count > 1)
                {
                    errors.Add(new ErrorInfo {
                        Error = Error.DuplicateDefinitionName,
                        Index = i,
                        DefinitionInfo = defs2[i],
                    });
                }
            }
        }

        //Errors for both tokenized and non-tokenize grammars
        //
        // E0  DefinitionInfo[] array is null (argnull exception)
        // T1  Any Expression or ExpressionItem re-use (of which reference cycles are a subset)
        // T2  DefinitionInfo.Name is null or empty
        // T3  DefinitionInfo.Directives is null
        // T4  DefinitionInfo.Expression is null
        // T6  Expression.Items.Length less than one (empty)
        // T7  OrExpression.Expressions is empty
        // T8  SubExpression.Tag is null
        // T9  DefRefSubExpression.DefinitionName is null or empty
        // T10 LiteralSubExpression.Value is null or empty
        // T11 CharClassSubExpression.CharClass is null
        // T12 CharClassSubExpression.CharClass doesn't cover any characters or patterns
        // S2  More than one DefinitionInfo has a given Name
        // S3  All items in the expression are skippable
        // S4  Any expressions in the orexpr is skippable
        // S5  A defref's DefinitionName is not found in the containing DefinitionInfo[] array
        // S?  Leading reference cycle?

        //Errors for non-tokenized grammars
        //
        // S6  Any DefinitionInfo.Directives includes Token, Subtoken, or Comment

        //Errors for tokenized grammars
        //
        // S7  A non-token def references a subtoken def
        // S8  A subtoken def refs a non-token ref
        // S9  A token def refs a non-token def
        // S10 A subtoken def refs a token def
        // S11 A definition has both token and subtoken directives
        // S12 A definition has both comment and subtoken directives
        // S13 Any def refs a comment def
    }
}

