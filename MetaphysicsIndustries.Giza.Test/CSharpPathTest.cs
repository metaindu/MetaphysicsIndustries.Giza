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
            Assert.AreEqual("/path/to/another/file2.giza", a3);
        }
    }
}
