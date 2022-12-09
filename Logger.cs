
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

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

