using System;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public static class Logger
    {
        public static readonly StringBuilder Log = new StringBuilder();

        public static void WriteLine(string str)
        {
            Log.AppendLine(str);
        }

        public static void WriteLine(string format, params object[] args)
        {
            Log.AppendFormat(format, args);
            Log.AppendLine();
        }
    }
}

