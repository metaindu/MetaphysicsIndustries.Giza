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
