using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class PreGrammar
    {
        public List<DefinitionExpression> Definitions;

        public DefinitionExpression FindDefinitionByName(string name)
        {
            return Definitions.FirstOrDefault(def => def.Name == name);
        }
    }
}
