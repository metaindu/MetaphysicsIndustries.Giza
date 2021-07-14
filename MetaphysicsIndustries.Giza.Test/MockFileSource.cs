using System;
using System.IO;

namespace MetaphysicsIndustries.Giza.Test
{
    public class MockFileSource : IFileSource
    {
        public MockFileSource()
            : this((s)=>"")
        {
        }

        public MockFileSource(Func<string, string> getFileContents)
        {
            GetFileContentsP = getFileContents;
        }

        public Func<string, string> GetFileContentsP { get; set; }

        public string GetFileContents(string filename)
        {
            if (GetFileContentsP == null)
                throw new FileNotFoundException(
                    "Could not find file \"{0}\"", filename);
            return GetFileContentsP(filename);
        }
    }
}
