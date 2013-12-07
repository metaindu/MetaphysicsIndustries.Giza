using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public interface ITokenSource
    {
        InputElementSet<Token> GetInputAtLocation(int index);
    }
}

