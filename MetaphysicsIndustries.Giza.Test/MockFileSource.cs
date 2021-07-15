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
