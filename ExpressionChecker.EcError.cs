using System;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public partial class ExpressionChecker
    {
        public class EcError : Error
        {
            public Expression Expression;
            public ExpressionItem ExpressionItem;
            public DefinitionExpression DefinitionInfo;
            public string DefinitionInfoName { get { return DefinitionInfo.Name; } }
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
                    descriptionFormat:"Directive 'atomic' applied to the non-token definition \"{DefinitionInfoName}\" ",
                    isWarning:true);
            public static readonly ErrorType MindWhitespaceInNonTokenDefinition =       new ErrorType(name:"MindWhitespaceInNonTokenDefinition",        isWarning:true);
            public static readonly ErrorType AtomicInTokenDefinition =                  new ErrorType(name:"AtomicInTokenDefinition",                   isWarning:true);
            public static readonly ErrorType AtomicInCommentDefinition =                new ErrorType(name:"AtomicInCommentDefinition",                 isWarning:true);
            public static readonly ErrorType MindWhitespaceInTokenizedDefinition =      new ErrorType(name:"MindWhitespaceInTokenizedDefinition",       isWarning:true);
        }

    }
}

