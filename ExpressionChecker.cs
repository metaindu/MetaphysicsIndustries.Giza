using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class ExpressionChecker
    {
        public class EcError : Error
        {
            public Expression Expression;
            public ExpressionItem ExpressionItem;
            public DefinitionExpression DefinitionInfo;
            public string DefinitionInfoName { get { return DefinitionInfo.Name; } }
            public int Index = -1;

            public override string Description
            {
                get
                {
                    if (ErrorType.DescriptionFormat != "{Name}")
                    {
                        return base.Description;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append(ErrorType.ToString());

                    if (Expression != null) sb.AppendFormat(", {0}", Expression.ToString());
                    if (ExpressionItem != null) sb.AppendFormat(", {0}", ExpressionItem.ToString());
                    if (DefinitionInfo != null) sb.AppendFormat(", {0} \"{1}\"", DefinitionInfo.ToString(), DefinitionInfo.Name);
                    if (Index > 0) sb.AppendFormat(", index {0}", Index);

                    if (ExpressionItem is DefRefSubExpression)
                    {
                        sb.AppendFormat(", defref \"{0}\"", (ExpressionItem as DefRefSubExpression).DefinitionName);
                    }

                    return sb.ToString();
                }
            }

            // all grammars
            public static readonly ErrorType ReusedDefintion =                          new ErrorType(name:"ReusedDefintion");
            public static readonly ErrorType NullDefinition =                           new ErrorType(name:"NullDefinition");
            public static readonly ErrorType ReusedExpressionOrItem =                   new ErrorType(name:"ReusedExpressionOrItem");
            public static readonly ErrorType NullOrEmptyDefinitionName =                new ErrorType(name:"NullOrEmptyDefinitionName");
            public static readonly ErrorType EmptyExpressionItems =                     new ErrorType(name:"EmptyExpressionItems");
            public static readonly ErrorType NullExpressionItem =                       new ErrorType(name:"NullExpressionItem");
            public static readonly ErrorType EmptyOrexprExpressionList =                new ErrorType(name:"EmptyOrexprExpressionList");
            public static readonly ErrorType NullOrexprExpression =                     new ErrorType(name:"NullOrexprExpression");
            public static readonly ErrorType NullSubexprTag =                           new ErrorType(name:"NullSubexprTag");
            public static readonly ErrorType NullOrEmptyDefrefName =                    new ErrorType(name:"NullOrEmptyDefrefName");
            public static readonly ErrorType NullOrEmptyLiteralValue =                  new ErrorType(name:"NullOrEmptyLiteralValue");
            public static readonly ErrorType NullOrEmptyCharClass =                     new ErrorType(name:"NullOrEmptyCharClass");
            public static readonly ErrorType DuplicateDefinitionName =                  new ErrorType(name:"DuplicateDefinitionName");
            public static readonly ErrorType AllItemsSkippable =                        new ErrorType(name:"AllItemsSkippable");
            public static readonly ErrorType SkippableOrexprExpressions =               new ErrorType(name:"SkippableOrexprExpressions ");
            public static readonly ErrorType DefRefNameNotFound =                       new ErrorType(name:"DefRefNameNotFound ");

            // spanned grammars
            public static readonly ErrorType TokenizedDirectiveInNonTokenizedGrammar =  new ErrorType(name:"TokenizedDirectiveInNonTokenizedGrammar",   isWarning:true );

            // parsed grammars
            public static readonly ErrorType MixedTokenizedDirectives =                 new ErrorType(name:"MixedTokenizedDirectives");
            public static readonly ErrorType NonTokenReferencesSubtoken =               new ErrorType(name:"NonTokenReferencesSubtoken");
            public static readonly ErrorType NonTokenReferencesComment =                new ErrorType(name:"NonTokenReferencesComment");
            public static readonly ErrorType TokenizedReferencesNonToken =              new ErrorType(name:"SubtokenReferencesNonToken");
            public static readonly ErrorType TokenizedReferencesToken =                 new ErrorType(name:"TokenReferencesNonToken");
            public static readonly ErrorType TokenizedReferencesComment =               new ErrorType(name:"SubtokenReferencesToken");
            public static readonly ErrorType AtomicInNonTokenDefinition =
                new ErrorType(
                    name:"AtomicInNonTokenDefinition",
                    descriptionFormat:"Directive 'atomic' applied to the non-token definition \"{DefinitionInfoName}\"",
                    isWarning:true);
            public static readonly ErrorType MindWhitespaceInNonTokenDefinition =       new ErrorType(name:"MindWhitespaceInNonTokenDefinition",        isWarning:true);
            public static readonly ErrorType AtomicInTokenDefinition =                  new ErrorType(name:"AtomicInTokenDefinition",                   isWarning:true);
            public static readonly ErrorType AtomicInCommentDefinition =                new ErrorType(name:"AtomicInCommentDefinition",                 isWarning:true);
            public static readonly ErrorType MindWhitespaceInTokenizedDefinition =      new ErrorType(name:"MindWhitespaceInTokenizedDefinition",       isWarning:true);
        }

        bool IsTokenized(DefinitionExpression def)
        {
            return
                def.Directives.Contains(DefinitionDirective.Token) ||
                def.Directives.Contains(DefinitionDirective.Subtoken) ||
                def.Directives.Contains(DefinitionDirective.Comment);
        }

        public List<Error> CheckDefinitionInfosForParsing(IEnumerable<DefinitionExpression> defs)
        {
            List<Error> errors = CheckDefinitionInfos(defs);

            Dictionary<string, DefinitionExpression> defsByName = new Dictionary<string, DefinitionExpression>();
            foreach (var def in defs)
            {
                defsByName[def.Name] = def;
            }
            foreach (var def in defs)
            {
                int numTokenizedDirectives = 0;

                if (def.Directives.Contains(DefinitionDirective.Token)) numTokenizedDirectives++;
                if (def.Directives.Contains(DefinitionDirective.Subtoken)) numTokenizedDirectives++;
                if (def.Directives.Contains(DefinitionDirective.Comment)) numTokenizedDirectives++;

                if (numTokenizedDirectives > 1)
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.MixedTokenizedDirectives,
                        DefinitionInfo = def,
                    });
                }

                if (!IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.AtomicInNonTokenDefinition,
                        DefinitionInfo = def,
                    });
                }

                if (def.Directives.Contains(DefinitionDirective.Token) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.AtomicInTokenDefinition,
                        DefinitionInfo = def,
                    });
                }

                if (def.Directives.Contains(DefinitionDirective.Comment) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.AtomicInCommentDefinition,
                        DefinitionInfo = def,
                    });
                }

                if (!IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.MindWhitespace))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.MindWhitespaceInNonTokenDefinition,
                        DefinitionInfo = def,
                    });
                }

                if (IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.MindWhitespace))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.MindWhitespaceInTokenizedDefinition,
                        DefinitionInfo = def,
                    });
                }

                foreach (DefRefSubExpression defref in def.EnumerateDefRefs())
                {
                    if (!defsByName.ContainsKey(defref.DefinitionName)) continue;

                    DefinitionExpression target = defsByName[defref.DefinitionName];

                    if (!IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Subtoken))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.NonTokenReferencesSubtoken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }

                    if (!IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.NonTokenReferencesComment,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Token))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.TokenizedReferencesToken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.TokenizedReferencesComment,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        !IsTokenized(target))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.TokenizedReferencesNonToken,
                            ExpressionItem = defref,
                            DefinitionInfo = def,
                        });
                    }
                }
            }

            return errors;
        }

        public List<Error> CheckDefinitionInfosForSpanning(IEnumerable<DefinitionExpression> defs)
        {
            List<Error> errors = CheckDefinitionInfos(defs);

            foreach (var def in defs)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Subtoken) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.TokenizedDirectiveInNonTokenizedGrammar,
                        DefinitionInfo = def,
                    });
                }
            }

            return errors;
        }

        public virtual List<Error> CheckDefinitionInfos(IEnumerable<DefinitionExpression> defs)
        {
            if (defs == null) throw new ArgumentNullException("defs");

            List<Error> errors = new List<Error>();

            // Any Expression or ExpressionItem object can be used in the 
            // trees only once. This precludes reference cycles as well as 
            // ADGs. ADGs are not much of a problem, but reference cycles 
            // would cause the checker to go into an infinite loop if we 
            // weren't expecting it.
            Set<Expression> visitedExprs = new Set<Expression>();
            Set<ExpressionItem> visitedItems = new Set<ExpressionItem>();
            Set<DefinitionExpression> visitedDefs = new Set<DefinitionExpression>();
            int index = -1;
            List<string> defNames = new List<string>();
            foreach (DefinitionExpression def in defs)
            {
                index++;
                if (def == null)
                {
                    errors.Add(new EcError {
                        ErrorType=EcError.NullDefinition,
                        Index=index,
                    });
                    continue;
                }

                defNames.Add(def.Name);
            }

            var defnames2 = new Set<string>();
            index = -1;
            foreach (DefinitionExpression def in defs)
            {
                index++;
                if (def == null) continue;

                if (visitedDefs.Contains(def))
                {
                    errors.Add(new EcError {
                        ErrorType=EcError.ReusedDefintion,
                        DefinitionInfo=def,
                        Index=index,
                    });
                    continue;
                }
                visitedDefs.Add(def);

                if (string.IsNullOrEmpty(def.Name))
                {
                    errors.Add(new EcError {
                        ErrorType=EcError.NullOrEmptyDefinitionName,
                        DefinitionInfo=def,
                        Index=index,
                    });
                }
                else if (defnames2.Contains(def.Name))
                {
                    errors.Add(new EcError {
                        ErrorType=EcError.DuplicateDefinitionName,
                        Index=index,
                        DefinitionInfo=def,
                    });
                }
                else
                {
                    defnames2.Add(def.Name);
                }

                CheckExpression(def, def, defNames, visitedExprs, visitedItems, errors);
            }

            return errors;
        }

        protected virtual void CheckExpression(DefinitionExpression def,
                             Expression expr,
                             List<string> defNames,
                             Set<Expression> visitedExprs,
                             Set<ExpressionItem> visitedItems,
                             List<Error> errors)
        {
            if (visitedExprs.Contains(expr))
            {
                errors.Add(new EcError {
                    ErrorType=EcError.ReusedExpressionOrItem,
                    Expression=expr,
                    DefinitionInfo=def,
                });
                return;
            }
            visitedExprs.Add(expr);

            if (expr.Items == null || expr.Items.Count < 1)
            {
                errors.Add(new EcError {
                    ErrorType = EcError.EmptyExpressionItems,
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
                        errors.Add(new EcError {
                            ErrorType = EcError.NullExpressionItem,
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
                    errors.Add(new EcError {
                        ErrorType = EcError.AllItemsSkippable,
                        Expression = expr,
                        DefinitionInfo = def,
                    });
                }
            }
        }

        protected virtual void CheckExpressionItem(DefinitionExpression def,
                                 ExpressionItem item,
                                 List<string> defNames,
                                 Set<Expression> visitedExprs,
                                 Set<ExpressionItem> visitedItems,
                                 List<Error> errors)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (visitedItems.Contains(item))
            {
                errors.Add(new EcError {
                    ErrorType=EcError.ReusedExpressionOrItem,
                    ExpressionItem=item,
                    DefinitionInfo=def,
                });
                return;
            }
            visitedItems.Add(item);

            if (item is OrExpression)
            {
                OrExpression orexor = (OrExpression)item;
                if (orexor.Expressions.Count < 1)
                {
                    errors.Add(new EcError {
                        ErrorType = EcError.EmptyOrexprExpressionList,
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
                            errors.Add(new EcError {
                                ErrorType = EcError.NullOrexprExpression,
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
                    errors.Add(new EcError {
                        ErrorType = EcError.NullSubexprTag,
                        ExpressionItem = item,
                        DefinitionInfo = def,
                    });
                }

                if (item is LiteralSubExpression)
                {
                    if (string.IsNullOrEmpty((item as LiteralSubExpression).Value))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.NullOrEmptyLiteralValue,
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
                        errors.Add(new EcError {
                            ErrorType = EcError.NullOrEmptyCharClass,
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
                        errors.Add(new EcError {
                            ErrorType = EcError.NullOrEmptyDefrefName,
                            ExpressionItem = item,
                            DefinitionInfo = def,
                        });
                    }
                    else if (!defNames.Contains(name))
                    {
                        errors.Add(new EcError {
                            ErrorType = EcError.DefRefNameNotFound,
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

