
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;

using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionChecker
    {
        /* NDefinitions are graphs of interconnected nodes. The graphs of
           nodes that result from using the supergrammar are not all of the 
           possible graphs that Spanner can deal with. That is, there are
           other graphs that could be constructed manually, or by some system
           other than GrammarCompiler, that would still work. The purpose of
           the DefinitionChecker then, is to confirm that a given NDefinition
           can be used by Spanner (a looser requirement), and NOT to confirm
           that the NDefinition conforms to what the supergrammar can output
           (a narrower requirement). */

        public IEnumerable<Error> CheckDefinitions(
            IEnumerable<NDefinition> defs)
        {
            if (defs == null) throw new ArgumentNullException("defs");

            var errors = new List<Error>();
            CheckDefinitions(defs, errors);
            return errors;
        }
        public void CheckDefinitions(IEnumerable<NDefinition> defs, 
            ICollection<Error> errors)
        {
            if (defs == null) throw new ArgumentNullException("defs");
            if (errors == null) throw new ArgumentNullException("errors");

            bool foundErrors = false;
            var defs1 = defs.ToList();
            foreach (var def in defs1)
            {
                var defErrors = CheckDefinition(def);
                if (defErrors.Count() > 0)
                {
                    errors.AddRange(defErrors);
                    foundErrors = true;
                }
            }

            if (!foundErrors)
            {
                // check for leading cycles
                var leaders = 
                    new Dictionary<NDefinition, HashSet<NDefinition>>();
                foreach (var def in defs1)
                {
                    var s = new HashSet<NDefinition>();
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
                    foreach (var def in leaders.Keys.ToArray())
                    {
                        if (leaders[def].Count < 1)
                        {
                            leaders.Remove(def);
                            c = true;
                        }
                        else
                        {
                            foreach (var leader in leaders[def].ToArray())
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
                    var start = leaders.Keys.First();
                    var leadingCycle = new List<NDefinition>();
                    var current = leaders[start].First();
                    leadingCycle.Add(start);
                    leadingCycle.Add(current);
                    while (current != start)
                    {
                        current = leaders[current].First();
                        leadingCycle.Add(current);
                    }

                    errors.Add(new DefinitionError {
                        ErrorType=DefinitionError.LeadingReferenceCycle,
                        Cycle=leadingCycle,
                    });
                }
            }
        }

        public IEnumerable<Error> CheckDefinition(NDefinition def)
        {
            if (def == null) throw new ArgumentNullException("def");

            var errors = new List<Error>();
            CheckDefinition(def, errors);
            return errors;
        }
        public void CheckDefinition(NDefinition def, ICollection<Error> errors)
        {
            if (def == null) throw new ArgumentNullException("def");
            if (errors == null) throw new ArgumentNullException("errors");

            bool checkPaths = true;

            // check that all NextNodes are in the same definition
            foreach (Node node in def.Nodes)
            {
                foreach (Node next in node.NextNodes)
                {
                    if (next.ParentDefinition != def)
                    {
                        errors.Add(new DefinitionError {
                            ErrorType=DefinitionError.NextNodeLinksOutsideOfDefinition,
                            Node = node,
                        });
                        checkPaths = false;
                    }
                }
            }

            //check start and end
            foreach (Node node in def.StartNodes)
            {
                if (node.ParentDefinition != def)
                {
                    errors.Add(new DefinitionError {
                        ErrorType=DefinitionError.StartNodeHasWrongParentDefinition,
                        Node=node,
                    });
                    checkPaths = false;
                }
            }
            foreach (Node node in def.EndNodes)
            {
                if (node.ParentDefinition != def)
                {
                    errors.Add(new DefinitionError {
                        ErrorType=DefinitionError.EndNodeHasWrongParentDefinition,
                        Node=node,
                    });
                    checkPaths = false;
                }
            }

            if (checkPaths)
            {
                // check path-from-start
                if (true)
                {
                    var knownPathFromStart = new HashSet<Node>();
                    var remaining = new HashSet<Node>();
                    var nexts = new HashSet<Node>();
                    remaining.UnionWith(def.Nodes);
                    knownPathFromStart.UnionWith(def.StartNodes);
                    remaining.ExceptWith(knownPathFromStart);

                    while (remaining.Count > 0)
                    {
                        nexts.Clear();
                        foreach (var node in knownPathFromStart)
                        {
                            nexts.UnionWith(node.NextNodes);
                        }
                        nexts.ExceptWith(knownPathFromStart);

                        if (nexts.Count < 1) break;

                        knownPathFromStart.UnionWith(nexts);
                        remaining.ExceptWith(nexts);
                    }

                    foreach (var node in remaining)
                    {
                        errors.Add(new DefinitionError {
                            ErrorType = DefinitionError.NodeHasNoPathFromStart,
                            Node = node,
                        });
                    }
                }

                // check path-to-end
                if (true)
                {
                    var previousNodes = new Dictionary<Node, HashSet<Node>>();
                    foreach (var node in def.Nodes)
                    {
                        previousNodes[node] = new HashSet<Node>();
                    }
                    foreach (var node in def.Nodes)
                    {
                        foreach (var next in node.NextNodes)
                        {
                            previousNodes[next].Add(node);
                        }
                    }

                    var knownPathToEnd = new HashSet<Node>();
                    var remaining = new HashSet<Node>(def.Nodes);
                    var prevs = new HashSet<Node>();

                    knownPathToEnd.UnionWith(def.EndNodes);
                    remaining.ExceptWith(knownPathToEnd);

                    while (remaining.Count > 0)
                    {
                        prevs.Clear();
                        foreach (var node in knownPathToEnd)
                        {
                            prevs.UnionWith(previousNodes[node]);
                        }

                        prevs.ExceptWith(knownPathToEnd);

                        if (prevs.Count < 1) break;

                        knownPathToEnd.UnionWith(prevs);
                        remaining.ExceptWith(prevs);
                    }

                    foreach (var node in remaining)
                    {
                        errors.Add(new DefinitionError {
                            ErrorType = DefinitionError.NodeHasNoPathToEnd,
                            Node = node,
                        });
                    }
                }
            }
        }
    }
}

