using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Definition
    {
        public Definition()
            : this(string.Empty)
        {
        }
        public Definition(string name)
        {
            Name = name;

            Nodes = new DefinitionNodeOrderedParentChildrenCollection(this);
        }

        public string Name;
        public DefinitionNodeOrderedParentChildrenCollection Nodes;
        public Set<Node> StartNodes = new Set<Node>();
        public Set<Node> EndNodes = new Set<Node>();

        public readonly Set<DefinitionDirective> Directives = new Set<DefinitionDirective>();
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
        public bool IsComment { get { return Directives.Contains(DefinitionDirective.Atomic); } }

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2} nodes", ID, Name, Nodes.Count);
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
