namespace MetaphysicsIndustries.Giza
{
    public struct ImportStatement
    {
        // TODO: module imports
        public bool IsModuleImport;
        public string ModuleOrFile;
        public ImportRef[] ImportRefs;
        public bool ImportAll;
    }
}
