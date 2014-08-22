using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MetaphysicsIndustries.Giza
{
    public enum DefinitionDirective
    {
        MindWhitespace,
        IgnoreCase,
        Atomic,
        Token,
        Subtoken,
        Comment,
    }
}