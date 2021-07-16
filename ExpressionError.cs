﻿
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
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class ExpressionError : Error
    {
        public Expression Expression;
        public ExpressionItem ExpressionItem;
        public Definition Definition;
        public string DefinitionName { get { return Definition.Name; } }
        public string ReferencedDefinitionName
        {
            get
            {
                if (ExpressionItem is DefRefSubExpression)
                {
                    return (ExpressionItem as DefRefSubExpression).DefinitionName;
                }
                else
                {
                    return null;
                }
            }
        }
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
                if (Definition != null) sb.AppendFormat(", {0} \"{1}\"", Definition.ToString(), Definition.Name);
                if (Index > 0) sb.AppendFormat(", index {0}", Index);

                if (ExpressionItem is DefRefSubExpression)
                {
                    sb.AppendFormat(", defref \"{0}\"", (ExpressionItem as DefRefSubExpression).DefinitionName);
                }

                return sb.ToString();
            }
        }

        // all grammars
        //  technical errors - can only occur when manually creating the
        //      object tree. SupergrammarSpanner does not generate expression
        //      trees with these errors.
        public static readonly ErrorType ReusedDefintion                            = new ErrorType(name:"ReusedDefintion",                                         descriptionFormat:"The Definition object for '{DefinitionName}' was re-used at index {Index}. ");
        public static readonly ErrorType NullDefinition                             = new ErrorType(name:"NullDefinition",                                          descriptionFormat:"A null was found in place of a Definition object, at index {Index}. ");
        public static readonly ErrorType ReusedExpression                           = new ErrorType(name:"ReusedExpression",                                        descriptionFormat:"An Expression object was re-used in definition '{DefinitionName}'.");
        public static readonly ErrorType ReusedExpressionItem                       = new ErrorType(name:"ReusedExpressionItem",                                    descriptionFormat:"An ExpressionItem object was re-used in definition '{DefinitionName}'.");
        public static readonly ErrorType NullOrEmptyDefinitionName                  = new ErrorType(name:"NullOrEmptyDefinitionName",                               descriptionFormat:"The definition's name was null or empty, at index {Index}. ");
        public static readonly ErrorType EmptyExpressionItems                       = new ErrorType(name:"EmptyExpressionItems",                                    descriptionFormat:"An Expression object's Items collection is empty in definition {DefinitionName}. ");
        public static readonly ErrorType NullExpressionItem                         = new ErrorType(name:"NullExpressionItem",                                      descriptionFormat:"A null reference was found in place of an ExpressionItem object, at index {Index}, in definition {DefinitionName}. ");
        public static readonly ErrorType EmptyOrexprExpressionList                  = new ErrorType(name:"EmptyOrexprExpressionList",                               descriptionFormat:"An OrExpression object's Expressions collection is empty, in definition {DefinitionName}. ");
        public static readonly ErrorType NullOrexprExpression                       = new ErrorType(name:"NullOrexprExpression",                                    descriptionFormat:"A null reference was found in place of an Expression object within an OrExpression object's Expressions collection, at index {Index}, in definition {DefinitionName}. ");
        public static readonly ErrorType NullSubexprTag                             = new ErrorType(name:"NullSubexprTag",                                          descriptionFormat:"A SubExpression object's Tag field is null, in definition {DefinitionName}. ");
        public static readonly ErrorType NullOrEmptyDefrefName                      = new ErrorType(name:"NullOrEmptyDefrefName",                                   descriptionFormat:"A DefRefSubExpression object's DefinitionName field is null or empty, in definition {DefinitionName}. ");
        public static readonly ErrorType NullOrEmptyLiteralValue                    = new ErrorType(name:"NullOrEmptyLiteralValue",                                 descriptionFormat:"A LiteralSubExpression object's Value field is null or empty, in definition {DefinitionName}. ");
        public static readonly ErrorType NullOrEmptyCharClass                       = new ErrorType(name:"NullOrEmptyCharClass",                                    descriptionFormat:"A CharClassSubExpression object's CharClass field is null or empty, in definition {DefinitionName}. ");

        //  grammar errors - SupergrammarSpanner can emit expression trees
        //      from grammar files with these errors.
        public static readonly ErrorType DuplicateDefinitionName                    = new ErrorType(name:"DuplicateDefinitionName",                                 descriptionFormat:"The definition '{DefinitionName}' has already been defined.");
        public static readonly ErrorType AllItemsSkippable                          = new ErrorType(name:"AllItemsSkippable",                                       descriptionFormat:"All Items within an expression are skippable, in definition '{DefinitionName}'.");
        public static readonly ErrorType SkippableOrexprExpressions                 = new ErrorType(name:"SkippableOrexprExpressions",                              descriptionFormat:"One of the expressions within an OrExpression in ");
        public static readonly ErrorType DefRefNameNotFound                         = new ErrorType(name:"DefRefNameNotFound",                                      descriptionFormat:"Definition '{DefinitionName}' references a definition '{ReferencedDefinitionName}' which is not defined.");

        // spanned grammars
        public static readonly ErrorType TokenizedDirectiveInNonTokenizedGrammar    = new ErrorType(name:"TokenizedDirectiveInNonTokenizedGrammar", isWarning:true, descriptionFormat:"One of directives 'token', 'subtoken', or 'comment' was applied to definition '{DefinitionName}', which is in a spanned grammar. The directive will be ignored.");

        // parsed grammars
        public static readonly ErrorType MixedTokenizedDirectives                   = new ErrorType(name:"MixedTokenizedDirectives",                                descriptionFormat:"The definition \"{DefinitionName}\" contains more than one of the 'token', 'subtoken', or 'comment' directives.");
        public static readonly ErrorType NonTokenReferencesSubtoken                 = new ErrorType(name:"NonTokenReferencesSubtoken",                              descriptionFormat:"The non-tokenized definition '{DefinitionName}' references the subtoken definition '{ReferencedDefinitionName}'.");
        public static readonly ErrorType NonTokenReferencesComment                  = new ErrorType(name:"NonTokenReferencesComment",                               descriptionFormat:"The non-tokenized definition '{DefinitionName}' references the comment definition '{ReferencedDefinitionName}'.");
        public static readonly ErrorType TokenizedReferencesNonToken                = new ErrorType(name:"TokenizedReferencesNonToken",                             descriptionFormat:"The tokenized definition '{DefinitionName}' references the non-tokenized definition '{ReferencedDefinitionName}'.");
        public static readonly ErrorType TokenizedReferencesToken                   = new ErrorType(name:"TokenizedReferencesToken",                                descriptionFormat:"The tokenized definition '{DefinitionName}' references the token definition '{ReferencedDefinitionName}'.");
        public static readonly ErrorType TokenizedReferencesComment                 = new ErrorType(name:"TokenizedReferencesComment",                              descriptionFormat:"The tokenized definition '{DefinitionName}' references the comment definition '{ReferencedDefinitionName}'.");
        public static readonly ErrorType AtomicInNonTokenDefinition                 = new ErrorType(name:"AtomicInNonTokenDefinition",              isWarning:true, descriptionFormat:"Directive 'atomic' applied to the non-tokenized definition '{DefinitionName}' is ignored.");
        public static readonly ErrorType MindWhitespaceInNonTokenDefinition         = new ErrorType(name:"MindWhitespaceInNonTokenDefinition",      isWarning:true, descriptionFormat:"Directive 'mind whitespace' applied to the non-tokenized definition '{DefinitionName}' is ignored.");
        public static readonly ErrorType AtomicInTokenDefinition                    = new ErrorType(name:"AtomicInTokenDefinition",                 isWarning:true, descriptionFormat:"Directive 'atomic' applied to the token definition '{DefinitionName}' is redundant.");
        public static readonly ErrorType AtomicInCommentDefinition                  = new ErrorType(name:"AtomicInCommentDefinition",               isWarning:true, descriptionFormat:"Directive 'atomic' applied to the comment definition '{DefinitionName}' is redundant.");
        public static readonly ErrorType MindWhitespaceInTokenizedDefinition        = new ErrorType(name:"MindWhitespaceInTokenizedDefinition",     isWarning:true, descriptionFormat:"Directive 'mind whitespace' applied to the token, subtoken, or comment definition '{DefinitionName}' is redundant.");
    }
}

