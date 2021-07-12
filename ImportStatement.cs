namespace MetaphysicsIndustries.Giza
{
    public struct ImportStatement
    {
        public bool IsModuleImport;
        public string ModuleOrFileName;
        public ImportRef[] ImportRefs;
    }
}
