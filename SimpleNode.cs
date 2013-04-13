using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("{Type} '{Text}', '{Tag}', {NextNodes.Count} Nodes")]
    public class SimpleNode
    {
        public SimpleNode(string text, NodeType type, string tag)
        {
            Text = text;
            Type = type;
            Tag = tag;
        }

        //if it's a start node, the text is the name of the definition
        //if it's a defref, the text is the unescaped name of the target definition
        //if it's a literal, the text is the unescaped content of the literal
        //if is's a char class, the text is the escaped content of the char class;
        //      "\l", "\d", and "\s" refer to letters, digits, and whitespace, respectively; 
        //      all other characters preceded by "\" are treated as literal;
        //      the delimiting brackets are removed
        public string Text = string.Empty;
        public NodeType Type = NodeType.charclass;

        //the tag is a string used to identify the node. typically, if it isn't
        //specified in the node source (grammar, state graph, etc.), then the
        //tag is just a copy of the text.
        //there are no restrictions on contents.
        public string Tag = string.Empty;

        public Set<SimpleNode> NextNodes = new Set<SimpleNode>();
    }
}
