using System;

namespace giza
{
    public enum SpanPrintingOptions
    {
        None,
        One,
        All,
    }

    public static class SpanPrintingOptionsHelper
    {
        public static SpanPrintingOptions FromBools(bool verbose=false, bool showAllParses=false)
        {
            if (showAllParses)
            {
                return SpanPrintingOptions.All;
            }
            if (verbose)
            {
                return SpanPrintingOptions.One;
            }
            return SpanPrintingOptions.None;
        }
    }

}

