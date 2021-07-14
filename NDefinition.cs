
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
using System.Text;

using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class NDefinition
    {
        public NDefinition(
            string name="",
            Node[] nodes=null,
            int[] nexts=null,
            int[] startNodes=null,
            int[] endNodes=null,
            DefinitionDirective[] directives=null)
        {
            Init(name, nodes, nexts, startNodes, endNodes, directives);
        }

        public void Init(
            string name="",
            Node[] nodes=null,
            int[] nexts=null,
            int[] startNodes=null,
            int[] endNodes=null,
            DefinitionDirective[] directives=null)
        {
            Nodes = new DefinitionNodeOrderedParentChildrenCollection(this);

            Name = name;

            if (nodes != null)
            {
                Nodes.AddRange(nodes);

                if (nexts != null)
                {
                    int i;
                    for (i = 0; i < nexts.Length; i += 2)
                    {
                        int from = nexts[i];
                        int to = nexts[i + 1];

                        if (from < nodes.Length &&
                            to < nodes.Length)
                        {
                            nodes[from].NextNodes.Add(nodes[to]);
                        }
                    }
                }

                if (startNodes != null)
                {
                    foreach (var i in startNodes)
                    {
                        if (i < nodes.Length)
                        {
                            StartNodes.Add(nodes[i]);
                        }
                    }
                }

                if (endNodes != null)
                {
                    foreach (var i in endNodes)
                    {
                        if (i < nodes.Length)
                        {
                            EndNodes.Add(nodes[i]);
                        }
                    }
                }
            }

            if (directives != null)
            {
                Directives.UnionWith(directives);
            }
        }

        public string Name;
        public DefinitionNodeOrderedParentChildrenCollection Nodes;
        public HashSet<Node> StartNodes = new HashSet<Node>();
        public HashSet<Node> EndNodes = new HashSet<Node>();

        public bool IsImported { get; set; } = false;

        public readonly HashSet<DefinitionDirective> Directives = new HashSet<DefinitionDirective>();
        public bool MindWhitespace { get { return Directives.Contains(DefinitionDirective.MindWhitespace); } }
        public bool IgnoreCase { get { return Directives.Contains(DefinitionDirective.IgnoreCase); } }
        public bool Atomic { get { return Directives.Contains(DefinitionDirective.Atomic); } }
        public bool IsTokenized
        {
            get
            {
                return
                    Directives.Contains(DefinitionDirective.Token) ||
                    Directives.Contains(DefinitionDirective.Subtoken) ||
                    Directives.Contains(DefinitionDirective.Comment);
            }
        }
        public bool IsComment { get { return Directives.Contains(DefinitionDirective.Comment); } }

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2} nodes", ID, Name, Nodes.Count);
        }

        private NGrammar _parentGrammar;
        public NGrammar ParentGrammar
        {
            get { return _parentGrammar; }
            set
            {
                if (value != _parentGrammar)
                {
                    if (_parentGrammar != null)
                    {
                        _parentGrammar.Definitions.Remove(this);
                    }

                    _parentGrammar = value;

                    if (_parentGrammar != null)
                    {
                        _parentGrammar.Definitions.Add(this);
                    }
                }
            }
        }

        public int ID
        {
            get
            {
                if (ParentGrammar == null)
                {
                    return -1;
                }
                else
                {
                    return ParentGrammar.Definitions.IndexOf(this); 
                }
            }
        }
    }
}
