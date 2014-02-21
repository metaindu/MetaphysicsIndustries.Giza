using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("'{Char}', {Tag}")]
    public class LiteralNode : Node
    {
        public LiteralNode(char ch)
            : this(ch, string.Empty)
        {
        }
        public LiteralNode(char ch, string tag)
            : base(tag)
        {
            _char = ch;
        }

        public static LiteralNode[] FromString(string text)
        {
            return FromString(text, text);
        }
        public static LiteralNode[] FromString(string text, string tag)
        {
            List<LiteralNode> list = new List<LiteralNode>();

            int i = 0;
            foreach (char ch in text)
            {
                list.Add(new LiteralNode(ch, tag));// + "_" + i.ToString()));
                i++;
            }
            for (i = 1; i < list.Count; i++)
            {
                list[i - 1].NextNodes.Add(list[i]);
            }

            return list.ToArray();
        }

        char _char;
        public char Char
        {
            get { return _char; }
        }

        public override NodeType Type
        {
            get { return NodeType.literal; }
        }
        public override bool Matches(char ch)
        {
            return (ch == Char);
        }
    }

    [DebuggerDisplay("'{CharClass}', {Tag}")]
    public class CharClassNode : Node
    {
        public CharClassNode(string text)
            : this(CharClass.FromUndelimitedCharClassText(text), text)
        {
        }
        public CharClassNode(string text, string tag)
            : this(CharClass.FromUndelimitedCharClassText(text), tag)
        {
        }
        public CharClassNode(CharClass cc)
            : this(cc, cc.ToString())
        {
        }
        public CharClassNode(CharClass cc, string tag)
            : base(tag)
        {
            if (cc == null) throw new ArgumentNullException("cc");
            _charClass = cc;
        }

        CharClass _charClass;
        public CharClass CharClass
        {
            get { return _charClass; }
        }

        public override NodeType Type
        {
            get { return NodeType.charclass; }
        }
        public override bool Matches(char ch)
        {
            return CharClass.Matches(ch);
        }
    }

    [DebuggerDisplay("start")]
    public class StartNode : Node
    {
        public StartNode(string defname)
        {
            //throw new NotImplementedException();
        }

        public override NodeType Type
        {
            get { return NodeType.start; }
        }
        public override bool Matches(char ch)
        {
            return false;
        }
        public override string Tag
        {
            get { return "start"; }
        }
    }

    [DebuggerDisplay("end")]
    public class EndNode : Node
    {
        public override NodeType Type
        {
            get { return NodeType.end; }
        }
        public override bool Matches(char ch)
        {
            return false;
        }
        public override string Tag
        {
            get { return "end"; }
        }
    }

    [DebuggerDisplay("DefRef {DefRef.Name}, {Tag}")]
    public class DefRefNode : Node
    {
        public DefRefNode(DefinitionNode def)
            : this(def, def == null ? string.Empty : def.Name)
        {
        }
        public DefRefNode(DefinitionNode def, string tag)
            : base(tag)
        {
            if (def == null) { throw new ArgumentNullException("def"); }

            _defRef = def;
        }

        DefinitionNode _defRef;
        public DefinitionNode DefRef
        {
            get { return _defRef; }
        }

        public override NodeType Type
        {
            get { return NodeType.defref; }
        }
        public override bool Matches(char ch)
        {
            foreach (Node node in DefRef.start.NextNodes)
            {
                if (node.Matches(ch))
                {
                    return true;
                }
            }

            return false;
        }
    }

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

        public abstract NodeType Type
        {
            get;
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

        public override string ToString()
        {
            throw new NotImplementedException();
            //return Text;
        }

        public static Node FromSimpleNode(SimpleNode unconNode, DefinitionNode[] defs)
        {
            if (unconNode.Text.Length < 1) throw new ArgumentException("unconNode");

            switch (unconNode.Type)
            {
                case NodeType.start: return new StartNode(unconNode.Text);
                case NodeType.end: return new EndNode();
                case NodeType.charclass: return new CharClassNode(unconNode.Text, unconNode.Tag);
                case NodeType.literal:
                    if (unconNode.Text.Length > 1)
                    {
                        return LiteralNode.FromString(unconNode.Text, unconNode.Tag)[0];
                    }
                    else
                    {
                        return new LiteralNode(unconNode.Text[0], unconNode.Tag);
                    }

                case NodeType.defref:
                    foreach (DefinitionNode def in defs)
                    {
                        if (def.Name == unconNode.Text)
                        {
                            return new DefRefNode(def, unconNode.Tag);
                        }
                    }
                    throw new KeyNotFoundException();

                default:
                    throw new InvalidOperationException("Unknown node type: " + unconNode.Type.ToString());
            }
        }

        public abstract bool Matches(char ch);
    }
}
