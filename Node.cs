using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    [DebuggerDisplay("'{CharClass}', {Tag}")]
    public class CharNode : Node
    {
        public CharNode(char ch)
            : this(new CharClass(ch))
        {
        }
        public CharNode(char ch, string tag)
            : this(new CharClass(ch), tag)
        {
        }
        public CharNode(CharClass cc)
            : this(cc, cc.ToString())
        {
        }
        public CharNode(CharClass cc, string tag)
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
        public static CharNode[] FromString(string text)
        {
            return FromString(text, text);
        }

        public static CharNode[] FromString(string text, string tag)
        {
            List<CharNode> list = new List<CharNode>();

            int i = 0;
            foreach (char ch in text)
            {
                list.Add(new CharNode(ch, tag));// + "_" + i.ToString()));
                i++;
            }
            for (i = 1; i < list.Count; i++)
            {
                list[i - 1].NextNodes.Add(list[i]);
            }

            return list.ToArray();
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
        public DefRefNode(Definition def)
            : this(def, def == null ? string.Empty : def.Name)
        {
        }
        public DefRefNode(Definition def, string tag)
            : base(tag)
        {
            if (def == null) { throw new ArgumentNullException("def"); }

            _defRef = def;
        }

        Definition _defRef;
        public Definition DefRef
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

        public bool IsAnEndOf(Definition def)
        {
            return NextNodes.Contains(def.end);
        }

        public static Node FromSimpleNode(SimpleNode unconNode, Definition[] defs)
        {
            if (unconNode.Text.Length < 1) throw new ArgumentException("unconNode");

            switch (unconNode.Type)
            {
                case NodeType.start: return new StartNode(unconNode.Text);
                case NodeType.end: return new EndNode();
                case NodeType.charclass: 
                    return new CharNode(
                        CharClass.FromUndelimitedCharClassText(unconNode.Text), 
                        unconNode.Tag);

                case NodeType.literal:
                    if (unconNode.Text.Length > 1)
                    {
                        return CharNode.FromString(unconNode.Text, unconNode.Tag)[0];
                    }
                    else
                    {
                        return new CharNode(unconNode.Text[0], unconNode.Tag);
                    }

                case NodeType.defref:
                    foreach (Definition def in defs)
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
