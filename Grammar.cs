using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public Grammar(IEnumerable<Definition> definitions = null,
            IEnumerable<ImportStatement> importStatements = null,
            string source = null)
        {
            if (definitions != null) Definitions.AddRange(definitions);
            if (importStatements != null)
                ImportStatements.AddRange(importStatements);
            Source = source;
        }

        public readonly List<Definition> Definitions = new List<Definition>();
        public readonly List<ImportStatement> ImportStatements =
            new List<ImportStatement>();
        public string Source;

        public Definition FindDefinitionByName(string name)
        {
            return Definitions.FirstOrDefault(def => def.Name == name);
        }

        public Grammar Clone(IEnumerable<Definition> newDefinitions=null,
            IEnumerable<ImportStatement> newImportStatements=null,
            string newSource=null)
        {
            if (newDefinitions == null) newDefinitions = Definitions;
            if (newImportStatements == null)
                newImportStatements = ImportStatements;
            if (newSource == null) newSource = Source;

            return new Grammar(newDefinitions, newImportStatements, newSource);
        }
    }
}
