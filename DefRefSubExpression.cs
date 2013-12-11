using System;

namespace MetaphysicsIndustries.Giza
{
    public class DefRefSubExpression : SubExpression
    {
        public DefRefSubExpression(string definitionName="",
                                   string tag="",
                                   bool isSkippable=false,
                                   bool isRepeatable=false)
            : base(tag: tag, isSkippable: isSkippable, isRepeatable: isRepeatable)
        {
            DefinitionName = definitionName;
        }

        public string DefinitionName;
    }
}

