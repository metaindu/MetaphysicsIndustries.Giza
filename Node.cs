using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public abstract class Node
    {
        protected Node()
            : this(string.Empty)
        {
        }
        protected Node(string tag)
        {
            if (tag == null) tag = string.Empty;

            _tag = tag;
        }

        public int ID
        {
            get
            {
                if (ParentDefinition == null)
                {
                    return -1;
                }
                else
                {
                    return ParentDefinition.Nodes.IndexOf(this); 
                }
            }
        }

        //the tag is a string used to identify the node. typically, if it isn't
        //specified in the node source (grammar, state graph, etc.), then the
        //tag is just a copy of the text.
        //there are no restrictions on contents.
        string _tag = string.Empty;
        public virtual string Tag
        {
            get { return _tag; }
        }
        public void SetTag(string tag)
        {
            _tag = tag;
        }

        public Set<Node> _nextNodes = new Set<Node>();
        public Set<Node> NextNodes
        {
            get { return _nextNodes; }
        }

        public bool IsEndNode
        {
            get { return ParentDefinition.EndNodes.Contains(this); }
        }

        private Definition _parentDefinition;
        public Definition ParentDefinition
        {
            get { return _parentDefinition; }
            set
            {
                if (value != _parentDefinition)
                {
                    if (_parentDefinition != null)
                    {
                        _parentDefinition.Nodes.Remove(this);
                    }

                    _parentDefinition = value;

                    if (_parentDefinition != null)
                    {
                        _parentDefinition.Nodes.Add(this);
                    }
                }
            }
        }
    }
}
