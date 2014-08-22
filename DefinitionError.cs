using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    /* Definitions are graphs of interconnected nodes. The graphs of nodes
       that result from using the supergrammar are not all of the possible
       graphs that Spanner can deal with. That is, there are other graphs
       that could be constructed manually, or by some system other than
       DefintionBuilder, that would still work. The purpose of the
       DefinitionChecker then, is to confirm that a given Definition can be
       used by Spanner (a looser requirement), and NOT to confirm that the
       Definition conforms to what the supergrammar can output (a narrower
       requirement). */

    public class DefinitionError : Error
    {
        public static readonly ErrorType NextNodeLinksOutsideOfDefinition =     new ErrorType(name:"NextNodeLinksOutsideOfDefinition",  descriptionFormat:"NextNodeLinksOutsideOfDefinition"  );
        public static readonly ErrorType StartNodeHasWrongParentDefinition =    new ErrorType(name:"StartNodeHasWrongParentDefinition", descriptionFormat:"StartNodeHasWrongParentDefinition" );
        public static readonly ErrorType EndNodeHasWrongParentDefinition =      new ErrorType(name:"EndNodeHasWrongParentDefinition",   descriptionFormat:"EndNodeHasWrongParentDefinition"   );
        public static readonly ErrorType LeadingReferenceCycle =                new ErrorType(name:"LeadingReferenceCycle",             descriptionFormat:"LeadingReferenceCycle"             );
        public static readonly ErrorType NodeHasNoPathFromStart =               new ErrorType(name:"NodeHasNoPathFromStart");
        public static readonly ErrorType NodeHasNoPathToEnd =                   new ErrorType(name:"NodeHasNoPathToEnd");

        public Node Node;
        public List<Definition> Cycle;
    }
}

