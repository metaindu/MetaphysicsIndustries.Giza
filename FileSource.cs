
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

using System.IO;

namespace MetaphysicsIndustries.Giza
{
    public class FileSource : IFileSource
    {
        public string GetFileContents(string filename)
        {
            return File.ReadAllText(path: filename);
        }

        public string CombinePath(string fromFile, string toFile)
        {
            var a1 = Path.GetFullPath(fromFile);
            var a2 = Path.GetDirectoryName(a1);
            var a3 = Path.Combine(a2, toFile);
            var a4 = Path.GetFullPath(a3);
            return a4;
        }
    }
}
