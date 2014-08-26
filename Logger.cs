using System;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public static class Logger
    {
        public static readonly StringBuilder Log = new StringBuilder();

        public static bool WriteToInternalLog = true;
        public static bool WriteToStderr = false;

        [Conditional("DEBUG")]
        public static void WriteLine(string str)
        {
            if (WriteToInternalLog)
            {
                Log.AppendLine(str);
            }
            if (WriteToStderr)
            {
                Console.Error.WriteLine(str);
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
    }
}

