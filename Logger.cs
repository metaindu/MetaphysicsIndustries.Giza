using System;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public static class Logger
    {
        public static readonly StringBuilder Log = new StringBuilder();

        [Conditional("DEBUG")]
        public static void WriteLine(string str)
        {
            Log.AppendLine(str);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            Log.AppendFormat(format, args);
            Log.AppendLine();
        }
    }
}

