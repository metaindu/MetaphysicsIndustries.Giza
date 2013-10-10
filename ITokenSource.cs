using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public interface ITokenSource
    {
        Token[] GetTokensAtLocation(string input, int index,
                                           List<Error> errors,
                                           out bool endOfInput,
                                           out int endOfInputIndex);
    }
}

