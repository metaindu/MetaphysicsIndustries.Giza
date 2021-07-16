
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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
using System.IO;

namespace MetaphysicsIndustries.Giza.Test
{
    public class MockFileSource : IFileSource
    {
        public MockFileSource(Func<string, string> getFileContents=null,
            string pwd=null)
        {
            if (getFileContents == null) getFileContents = s => "";

            GetFileContentsP = getFileContents;
            Pwd = pwd;
        }

        public Func<string, string> GetFileContentsP { get; set; }
        public string Pwd;

        public string GetFileContents(string filename)
        {
            if (GetFileContentsP == null)
                throw new FileNotFoundException(
                    "Could not find file \"{0}\"", filename);
            return GetFileContentsP(filename);
        }

        public string CombinePath(string fromFile, string toFile)
        {
            if (Pwd == null) return toFile;

            var a1 = fromFile;
            var a2 = Path.GetDirectoryName(a1);
            var a3 = Path.Combine(a2, toFile);
            var a4 = Path.GetFullPath(a3);
            return a4;
        }
    }
}
