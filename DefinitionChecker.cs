using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;

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

        public class DcError : Error
        {
            public static readonly ErrorType NextNodeLinksOutsideOfDefinition =     new ErrorType(name:"NextNodeLinksOutsideOfDefinition",  descriptionFormat:"NextNodeLinksOutsideOfDefinition"  );
            public static readonly ErrorType StartNodeHasWrongParentDefinition =    new ErrorType(name:"StartNodeHasWrongParentDefinition", descriptionFormat:"StartNodeHasWrongParentDefinition" );
            public static readonly ErrorType EndNodeHasWrongParentDefinition =      new ErrorType(name:"EndNodeHasWrongParentDefinition",   descriptionFormat:"EndNodeHasWrongParentDefinition"   );
            public static readonly ErrorType LeadingReferenceCycle =                new ErrorType(name:"LeadingReferenceCycle",             descriptionFormat:"LeadingReferenceCycle"             );
            public static readonly ErrorType NodehasNoPathFromStart =               new ErrorType(name:"NodehasNoPathFromStart");
            public static readonly ErrorType NodehasNoPathToEnd =                   new ErrorType(name:"NodehasNoPathToEnd");

            public Node Node;
            public List<Definition> Cycle;
        }

        public IEnumerable<Error> CheckDefinitions(IEnumerable<Definition> defs)
        {
            foreach (Definition def in defs)
            {
                foreach (DcError ei in CheckDefinition(def))
                {
                    yield return ei;
                }
            }

            // check for leading cycles
            Dictionary<Definition, Set<Definition>> leaders = new Dictionary<Definition, Set<Definition>>();
            foreach (Definition def in defs)
            {
                Set<Definition> s = new Set<Definition>();
                leaders[def] = s;

                foreach (Node start in def.StartNodes)
                {
                    if (start is DefRefNode)
                    {
                        s.Add((start as DefRefNode).DefRef);
                    }
                }
            }

            bool c = true;
            while (c)
            {
                c = false;
                foreach (Definition def in leaders.Keys.ToArray())
                {
                    if (leaders[def].Count < 1)
                    {
                        leaders.Remove(def);
                        c = true;
                    }
                    else
                    {
                        foreach (Definition leader in leaders[def].ToArray())
                        {
                            if (!leaders.ContainsKey(leader))
                            {
                                leaders[def].Remove(leader);
                                c = true;
                            }
                        }
                    }
                }
            }

            if (leaders.Count > 0)
            {
                Definition start = leaders.Keys.First();
                List<Definition> leadingCycle = new List<Definition>();
                Definition current = leaders[start].First();
                leadingCycle.Add(start);
                leadingCycle.Add(current);
                while (current != start)
                {
                    current = leaders[current].First();
                    leadingCycle.Add(current);
                }

                yield return new DcError {
                    ErrorType=DcError.LeadingReferenceCycle,
                    Cycle=leadingCycle,
                };
            }
        }

        public IEnumerable<Error> CheckDefinition(Definition def)
        {
            // check that all NextNodes are in the same definition
            foreach (Node node in def.Nodes)
            {
                foreach (Node next in node.NextNodes)
                {
                    if (next.ParentDefinition != def)
                    {
                        yield return new DcError {
                            ErrorType=DcError.NextNodeLinksOutsideOfDefinition,
                            Node = next,
                        };
                    }
                }
            }

            //check start and end
            foreach (Node node in def.StartNodes)
            {
                if (node.ParentDefinition != def)
                {
                    yield return new DcError {
                        ErrorType=DcError.StartNodeHasWrongParentDefinition,
                        Node=node,
                    };
                }
            }
            foreach (Node node in def.EndNodes)
            {
                if (node.ParentDefinition != def)
                {
                    yield return new DcError {
                        ErrorType=DcError.EndNodeHasWrongParentDefinition,
                        Node=node,
                    };
                }
            }
        }
    }
}

