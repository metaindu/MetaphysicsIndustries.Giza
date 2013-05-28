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
        public Set<Node> StartNodes = new Set<Node>();
        public Set<Node> EndNodes = new Set<Node>();

        public readonly Set<DefinitionDirective> Directives = new Set<DefinitionDirective>();
        public bool IncludeWhitespace { get { return Directives.Contains(DefinitionDirective.IncludeWhitespace); } }
        public bool IgnoreCase { get { return Directives.Contains(DefinitionDirective.IgnoreCase); } }
        public bool Atomic { get { return Directives.Contains(DefinitionDirective.Atomic); } }

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
