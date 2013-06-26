using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class TokenizedGrammarBuilder
    {
        public Grammar BuildTokenizedGrammar(DefinitionInfo[] defs)
        {
            ExpressionChecker ec = new ExpressionChecker();
            List<ExpressionChecker.ErrorInfo> errors = ec.CheckDefinitionInfosForParsing(defs);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException("Errors in expressions.");
            }

            throw new NotImplementedException();
        }
    }
}

