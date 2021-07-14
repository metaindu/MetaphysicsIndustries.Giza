
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

using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public partial class ExpressionChecker
    {
        bool IsTokenized(DefinitionExpression def)
        {
            return
                def.Directives.Contains(DefinitionDirective.Token) ||
                def.Directives.Contains(DefinitionDirective.Subtoken) ||
                def.Directives.Contains(DefinitionDirective.Comment);
        }

        public List<Error> CheckDefinitionForParsing(IEnumerable<DefinitionExpression> defs)
        {
            List<Error> errors = CheckDefinitions(defs);

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
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.MixedTokenizedDirectives,
                        Definition = def,
                    });
                }

                if (!IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.AtomicInNonTokenDefinition,
                        Definition = def,
                    });
                }

                if (def.Directives.Contains(DefinitionDirective.Token) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.AtomicInTokenDefinition,
                        Definition = def,
                    });
                }

                if (def.Directives.Contains(DefinitionDirective.Comment) &&
                    def.Directives.Contains(DefinitionDirective.Atomic))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.AtomicInCommentDefinition,
                        Definition = def,
                    });
                }

                if (!IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.MindWhitespace))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.MindWhitespaceInNonTokenDefinition,
                        Definition = def,
                    });
                }

                if (IsTokenized(def) &&
                    def.Directives.Contains(DefinitionDirective.MindWhitespace))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.MindWhitespaceInTokenizedDefinition,
                        Definition = def,
                    });
                }

                foreach (DefRefSubExpression defref in def.EnumerateDefRefs())
                {
                    if (!defsByName.ContainsKey(defref.DefinitionName)) continue;

                    DefinitionExpression target = defsByName[defref.DefinitionName];

                    if (!IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Subtoken))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NonTokenReferencesSubtoken,
                            ExpressionItem = defref,
                            Definition = def,
                        });
                    }

                    if (!IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NonTokenReferencesComment,
                            ExpressionItem = defref,
                            Definition = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Token))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.TokenizedReferencesToken,
                            ExpressionItem = defref,
                            Definition = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        target.Directives.Contains(DefinitionDirective.Comment))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.TokenizedReferencesComment,
                            ExpressionItem = defref,
                            Definition = def,
                        });
                    }

                    if (IsTokenized(def) &&
                        !IsTokenized(target))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.TokenizedReferencesNonToken,
                            ExpressionItem = defref,
                            Definition = def,
                        });
                    }
                }
            }

            return errors;
        }

        public List<Error> CheckDefinitionsForSpanning(IEnumerable<DefinitionExpression> defs)
        {
            List<Error> errors = CheckDefinitions(defs);

            foreach (var def in defs)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Subtoken) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.TokenizedDirectiveInNonTokenizedGrammar,
                        Definition = def,
                    });
                }
            }

            return errors;
        }

        public virtual List<Error> CheckDefinitions(IEnumerable<DefinitionExpression> defs)
        {
            if (defs == null) throw new ArgumentNullException("defs");

            List<Error> errors = new List<Error>();

            // Any Expression or ExpressionItem object can be used in the 
            // trees only once. This precludes reference cycles as well as 
            // ADGs. ADGs are not much of a problem, but reference cycles 
            // would cause the checker to go into an infinite loop if we 
            // weren't expecting it.
            HashSet<Expression> visitedExprs = new HashSet<Expression>();
            HashSet<ExpressionItem> visitedItems = new HashSet<ExpressionItem>();
            HashSet<DefinitionExpression> visitedDefs = new HashSet<DefinitionExpression>();
            int index = -1;
            List<string> defNames = new List<string>();
            foreach (DefinitionExpression def in defs)
            {
                index++;
                if (def == null)
                {
                    errors.Add(new ExpressionError {
                        ErrorType=ExpressionError.NullDefinition,
                        Index=index,
                    });
                    continue;
                }

                defNames.Add(def.Name);
            }

            var defnames2 = new HashSet<string>();
            index = -1;
            foreach (DefinitionExpression def in defs)
            {
                index++;
                if (def == null) continue;

                if (visitedDefs.Contains(def))
                {
                    errors.Add(new ExpressionError {
                        ErrorType=ExpressionError.ReusedDefintion,
                        Definition=def,
                        Index=index,
                    });
                    continue;
                }
                visitedDefs.Add(def);

                if (string.IsNullOrEmpty(def.Name))
                {
                    errors.Add(new ExpressionError {
                        ErrorType=ExpressionError.NullOrEmptyDefinitionName,
                        Definition=def,
                        Index=index,
                    });
                }
                else if (defnames2.Contains(def.Name))
                {
                    errors.Add(new ExpressionError {
                        ErrorType=ExpressionError.DuplicateDefinitionName,
                        Index=index,
                        Definition=def,
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
                             HashSet<Expression> visitedExprs,
                             HashSet<ExpressionItem> visitedItems,
                             List<Error> errors)
        {
            if (visitedExprs.Contains(expr))
            {
                errors.Add(new ExpressionError {
                    ErrorType=ExpressionError.ReusedExpression,
                    Expression=expr,
                    Definition=def,
                });
                return;
            }
            visitedExprs.Add(expr);

            if (expr.Items == null || expr.Items.Count < 1)
            {
                errors.Add(new ExpressionError {
                    ErrorType = ExpressionError.EmptyExpressionItems,
                    Expression = expr,
                    Definition = def,
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
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NullExpressionItem,
                            Expression = expr,
                            Index = index,
                            Definition = def
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
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.AllItemsSkippable,
                        Expression = expr,
                        Definition = def,
                    });
                }
            }
        }

        protected virtual void CheckExpressionItem(DefinitionExpression def,
                                 ExpressionItem item,
                                 List<string> defNames,
                                 HashSet<Expression> visitedExprs,
                                 HashSet<ExpressionItem> visitedItems,
                                 List<Error> errors)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (visitedItems.Contains(item))
            {
                errors.Add(new ExpressionError {
                    ErrorType=ExpressionError.ReusedExpressionItem,
                    ExpressionItem=item,
                    Definition=def,
                });
                return;
            }
            visitedItems.Add(item);

            if (item is OrExpression)
            {
                OrExpression orexor = (OrExpression)item;
                if (orexor.Expressions.Count < 1)
                {
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.EmptyOrexprExpressionList,
                        Definition = def,
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
                            errors.Add(new ExpressionError {
                                ErrorType = ExpressionError.NullOrexprExpression,
                                ExpressionItem = item,
                                Index = index,
                                Definition = def,
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
                    errors.Add(new ExpressionError {
                        ErrorType = ExpressionError.NullSubexprTag,
                        ExpressionItem = item,
                        Definition = def,
                    });
                }

                if (item is LiteralSubExpression)
                {
                    if (string.IsNullOrEmpty((item as LiteralSubExpression).Value))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NullOrEmptyLiteralValue,
                            ExpressionItem = item,
                            Definition = def,
                        });
                    }
                }
                else if (item is CharClassSubExpression)
                {
                    CharClass cc = (item as CharClassSubExpression).CharClass;

                    if (cc == null || cc.GetAllCharsCount() < 1)
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NullOrEmptyCharClass,
                            ExpressionItem = item,
                            Definition = def
                        });
                    }
                }
                else if (item is DefRefSubExpression)
                {
                    string name = (item as DefRefSubExpression).DefinitionName;
                    if (string.IsNullOrEmpty(name))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.NullOrEmptyDefrefName,
                            ExpressionItem = item,
                            Definition = def,
                        });
                    }
                    else if (!defNames.Contains(name))
                    {
                        errors.Add(new ExpressionError {
                            ErrorType = ExpressionError.DefRefNameNotFound,
                            ExpressionItem = item,
                            Definition = def,
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

