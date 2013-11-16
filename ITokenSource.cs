using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public interface ITokenSource
    {
        TokenizationInfo GetTokensAtLocation(int index);
    }
}

