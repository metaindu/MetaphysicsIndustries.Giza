using System.IO;

namespace MetaphysicsIndustries.Giza
{
    public class FileSource : IFileSource
    {
        public string GetFileContents(string filename)
        {
            return File.ReadAllText(path: filename);
        }
    }
}
