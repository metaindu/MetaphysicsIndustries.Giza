using System;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionChecker
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
        public DefinitionChecker()
        {
        }
    }
}

