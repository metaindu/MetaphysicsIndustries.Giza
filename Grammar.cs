using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Grammar
    {
        public List<Definition> Definitions;
        public List<ImportStatement> ImportStatements;

        public Definition FindDefinitionByName(string name)
        {
            return Definitions.FirstOrDefault(def => def.Name == name);
        }
    }
}
