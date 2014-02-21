using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("{Value}")]
    public class Span
    {
        public Span(int start, int length, string input, params Span[] subspans)
        {
            if (start < 0) { throw new ArgumentOutOfRangeException("start"); }
            if (length < 0) { throw new ArgumentOutOfRangeException("length"); }

            Start = start;
            Length = length;
            Input = input ?? string.Empty;
            Subspans = subspans ?? new Span[0];
        }

        public string Definition;

        public string Tag = string.Empty;

        public string Value
        {
            get { return GetText(Input); }
        }

        public int Start;
        public int Length;
        public string Input;
        public int End
        {
            get { return Start + Length - 1; }
        }
        public string GetText(string input)
        {
            return input.Substring(Start, Length);
        }

        public Span[] Subspans;
    }
}
