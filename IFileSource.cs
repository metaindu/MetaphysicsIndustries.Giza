namespace MetaphysicsIndustries.Giza
{
    public interface IFileSource
    {
        string GetFileContents(string filename);
        string CombinePath(string fromFile, string toFile);
    }
}
