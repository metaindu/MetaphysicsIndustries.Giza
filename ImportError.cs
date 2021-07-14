namespace MetaphysicsIndustries.Giza
{
    public class ImportError : Error
    {
        public static readonly ErrorType DefinitionNotFound = new ErrorType("DefinitionNotFound");

        public string DefinitionName;
    }
}
