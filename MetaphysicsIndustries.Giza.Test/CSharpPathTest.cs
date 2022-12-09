
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
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class CSharpPathTest
    {
        [Test]
        public void PosixLeadingSlashIsAbsolute()
        {
            // given
            const string path = "/path/to/file";
            // expect
            Assert.IsTrue(Path.IsPathRooted(path));
        }

        [Test]
        public void PosixNoLeadingSlashIsRelative()
        {
            // given
            const string path = "path/to/file";
            // expect
            Assert.IsFalse(Path.IsPathRooted(path));
        }

        [Test]
        public void PosixLeadingDotIsRelative()
        {
            // given
            const string path = "./path/to/file";
            // expect
            Assert.IsFalse(Path.IsPathRooted(path));
        }

        [Test]
        public void PosixLeadingDotsIsRelative()
        {
            // given
            const string path = "../path/to/file";
            // expect
            Assert.IsFalse(Path.IsPathRooted(path));
        }

        [Test]
        public void PosixCombineAbsoluteAndRelative()
        {
            // given
            const string fromPath = "/path/to/file1.giza";
            const string toPath = "../another/file2.giza";
            // when
            var a1 = Path.GetFullPath(fromPath);
            var a2 = Path.Combine(a1, toPath);
            var a3 = Path.GetFullPath(a2);
            // then
            Assert.That(a3, Is.EqualTo("/path/to/another/file2.giza"));
        }
    }
}
