using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Definition
    {
        public static int __id = 0;
        public readonly int _id;

        public Definition()
            : this(string.Empty)
        {
        }
        public Definition(string name)
        {
            Name = name;

            Nodes = new DefinitionNodeOrderedParentChildrenCollection(this);

            _id = __id;
            __id++;
        }

        public string Name;
        public DefinitionNodeOrderedParentChildrenCollection Nodes;
        public Set<Node> StartingNodes = new Set<Node>();
        public Set<Node> EndingNodes = new Set<Node>();

        public IEnumerable<Node> GetStartingNodes()
        {
            Set<Node> startNodes = new Set<Node>();

            startNodes.AddRange(StartingNodes);

            return startNodes;
        }

        public IEnumerable<Node> GetEndingNodes()
        {
            Set<Node> endNodes = new Set<Node>();

            endNodes.AddRange(EndingNodes);

            return endNodes;
        }

        public bool IgnoreWhitespace = false;
        public bool IgnoreCase = false;
        public bool Contiguous = false;

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2} nodes", _id, Name, Nodes.Count);
        }

        private Grammar _parentGrammar;
        public Grammar ParentGrammar
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
    }
}
