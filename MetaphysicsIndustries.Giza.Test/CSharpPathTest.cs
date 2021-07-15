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
    }
}
