
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

        public HashSet<Node> _nextNodes = new HashSet<Node>();
        public HashSet<Node> NextNodes
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
