using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Text.RegularExpressions;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionBuilder
    {
        public Definition[] BuildDefinitions(Span span)
        {
            if (span.Tag != "grammar") { throw new ArgumentException("span"); }

            List<SimpleDefinition> defs = new List<SimpleDefinition>();
            foreach (Span subspan in span.Subspans)
            {
                if (subspan.Tag == "comment") continue;

                SimpleDefinition def = new SimpleDefinition();
                bool found = false;
                foreach (Span sub2 in subspan.Subspans)
                {
                    if (sub2.Tag == "identifier")
                    {
                        def.Name = sub2.Value;
                        found = true;
                        break;
                    }
                }
                if (!found) throw new InvalidOperationException("Couldn't find a name for the definition.");

                BuildSingleDefinition(subspan, def);
                defs.Add(def);
            }

            CheckDefRefs(defs);

            List<Definition> defs2 = new List<Definition>();
            Dictionary<SimpleDefinition, Definition> defmatchup = new Dictionary<SimpleDefinition, Definition>();

            //create the empty defintion objects
            foreach (SimpleDefinition def in defs)
            {
                Definition def2 = new Definition();
                def2.Name = def.Name;

                defmatchup[def]=def2;
                defs2.Add(def2);
            }

            //populate the new definitions with nodes
            foreach (SimpleDefinition def in defs)
            {
                Definition def2 = defmatchup[def];
                def2.IgnoreWhitespace = def.IgnoreWhitespace;
                def2.IgnoreCase = def.IgnoreCase;

                //create the new nodes
                Dictionary<SimpleNode, Node> nodeMatchup = new Dictionary<SimpleNode, Node>();
                foreach (SimpleNode node in def.Nodes)
                {
                    if (node.Text == "*")
                    {
                    }

                    Node node2 = Node.FromSimpleNode(node, defs2.ToArray());
                    nodeMatchup[node] = node2;

                    def2.Nodes.AddRange(GatherNodes(node2));

                    if (node.Type == NodeType.start)
                    {
                        def2.start = node2;
                    }
                    else if (node.Type == NodeType.end)
                    {
                        def2.end = node2;
                    }
                }

                //connect nodes to each other
                foreach (SimpleNode node in def.Nodes)
                {
                    Node node2 = nodeMatchup[node];

                    //FromSimpleNode converts a multi-char literal into a list of single-char nodes
                    //we have to go to the end of the list to tell that one where to go next
                    if (node.Type == NodeType.literal && node.Text.Length > 1)
                    {
                        CharNode literalnode = (CharNode)node2;
                        while (literalnode.NextNodes.Count > 0)
                        {
                            literalnode = (CharNode)literalnode.NextNodes.GetFirst();
                        }
                        node2 = literalnode;
                    }

                    foreach (SimpleNode next in node.NextNodes)
                    {
                        node2.NextNodes.Add(nodeMatchup[next]);
                    }
                }
            }

            foreach (Definition def2 in defs2)
            {
                Console.WriteLine(def2.ToString());
                foreach (Node node in def2.Nodes)
                {
                    Console.Write("  ");
                    Console.WriteLine(node.ToString());

                    foreach(Node next in node.NextNodes)
                    {
                        Console.Write("    ");
                        Console.WriteLine(next.ToString());
                    }
                }
            }

            return defs2.ToArray();
        }

        void BuildSingleDefinition(Span span, SimpleDefinition def)
        {
            if (span.Tag != "definition") { throw new ArgumentException("span"); }

            def.IgnoreWhitespace = true;

            int i;
            bool ignoreWhitespace = true;
            bool ignoreCase = false;
            for (i = 0; i < span.Subspans.Length; i++)
            {
                if (span.Subspans[i].Tag != "defmod") break;

                Span defmod = span.Subspans[i];
                foreach (Span item in defmod.Subspans)
                {
                    if (item.Tag == "defmod-item")
                    {
                        if (item.Value == "whitespace")
                        {
                            ignoreWhitespace = false;
                        }
                        else if (
                            item.Subspans.Length > 0 && 
                            item.Subspans[0].Value == "ignore" && 
                            (
                                item.Subspans.Length ==2 && 
                                item.Subspans[1].Value == "case" 
                            )
                            ||
                            (
                                item.Subspans.Length ==3 && 
                                item.Subspans[1].Value == "-" && 
                                item.Subspans[2].Value == "case" 
                            ))
                        {
                            ignoreCase = true;
                        }
                    }
                }
            }

            def.IgnoreWhitespace = ignoreWhitespace;
            def.IgnoreCase = ignoreCase;

            if (span.Subspans[i].Tag != "identifier") throw new NotImplementedException();
            def.Name = span.Subspans[i].Value;

            if (span.Subspans[i + 1].Value != "=") throw new NotImplementedException();
            if (span.Subspans[i + 2].Tag != "expr") throw new NotImplementedException();

            SimpleNode start = new SimpleNode(def.Name, NodeType.start, def.Name);
            StartNode start2 = new StartNode(def.Name);
            def.start = start;

            SimpleNode end = new SimpleNode("end", NodeType.end, "end");
            EndNode end2 = new EndNode();
            def.end = end;

            SimpleNode[] frontNodes;
            SimpleNode[] backNodes;
            bool skipAll;
            ProcessExpr(span.Subspans[i + 2], out frontNodes, out backNodes, out skipAll);

            start.NextNodes.AddRange(frontNodes);
            foreach (SimpleNode node in backNodes)
            {
                node.NextNodes.Add(end);
            }
            //if (skipAll)
            //{
            //    //should we allow this?
            //    start.NextNodes.Add(end);
            //}

            def.Nodes.AddRange(GatherSimpleNodes(start));
        }

        private void ProcessExpr(Span expr, out SimpleNode[] frontNodes, out SimpleNode[] backNodes, out bool skipAll)
        {
            if (expr.Tag != "expr") throw new ArgumentException("expr");

            List<SimpleNode[]> fronts = new List<SimpleNode[]>();
            List<SimpleNode[]> backs = new List<SimpleNode[]>();
            List<bool> skips = new List<bool>();

            foreach (Span sub in expr.Subspans)
            {
                if (sub.Tag == "comment") continue;

                SimpleNode[] frontNodes2;
                SimpleNode[] backNodes2;
                Span modifier;
                if (sub.Tag == "subexpr")
                {
                    ProcessSubExpr(sub, out frontNodes2, out backNodes2, out modifier);
                }
                else if (sub.Tag == "orexpr")
                {
                    ProcessOrExpr(sub, out frontNodes2, out backNodes2, out modifier);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                bool skip = false;
                if (modifier != null)
                {
                    if (modifier.Value == "*" ||
                        modifier.Value == "?")
                    {
                        skip = true;
                    }
                    if (modifier.Value == "*" ||
                        modifier.Value == "+")
                    {
                    }
                }

                skips.Add(skip);
                fronts.Add(frontNodes2);
                backs.Add(backNodes2);
            }

            int i;
            for (i = 1; i < fronts.Count; i++)
            {
                foreach (SimpleNode b in backs[i - 1])
                {
                    b.NextNodes.AddRange(fronts[i]);
                }
            }

            for (i = 0; i < skips.Count; i++)
            {
                int j;
                for (j = i + 1; j < skips.Count-1; j++)
                {
                    if (skips[j])
                    {
                        foreach (SimpleNode b in backs[i])
                        {
                            b.NextNodes.AddRange(fronts[j + 1]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Set<SimpleNode> frontNodes3 = new Set<SimpleNode>();
            Set<SimpleNode> backNodes3 = new Set<SimpleNode>();

            skipAll = skips[0];
            for (i = 0; i < skips.Count-1; i++)
            {
                if (skips[i])
                {
                    frontNodes3.AddRange(fronts[i + 1]);
                }
                else
                {
                    skipAll = false;
                    break;
                }
            }
            for (i = 1; i < skips.Count; i++)
            {
                if (skips[i])
                {
                    backNodes3.AddRange(backs[i - 1]);
                }
                else
                {
                    backNodes3.Clear();
                }
            }

            frontNodes3.AddRange(fronts[0]);
            backNodes3.AddRange(backs[backs.Count - 1]);

            frontNodes = frontNodes3.ToArray();
            backNodes = backNodes3.ToArray();
        }

        private void ProcessSubExpr(Span subexpr, out SimpleNode[] frontNodes, out SimpleNode[] backNodes, out Span modifier)
        {
            if (subexpr.Tag != "subexpr") throw new ArgumentException("subexpr");

            Span main = null;
            Span tag = null;

            modifier = null;

            foreach (Span subspan in subexpr.Subspans)
            {
                if (subspan.Tag == "modifier")
                {
                    modifier = subspan;
                }
                else if (subspan.Tag == "tag")
                {
                    tag = subspan;
                }
                else if (subspan.Tag == "identifier" ||
                        subspan.Tag == "literal" ||
                        subspan.Tag == "charclass")
                {
                    main = subspan;
                }
            }

            if (main == null) throw new InvalidOperationException();

            string text;
            NodeType type;
            string tagString;
            SimpleNode node;
            Node node2;

            List<SimpleNode> snodes = new List<SimpleNode>();

            if (main.Tag == "identifier")
            {
                type = NodeType.defref;
                text = main.Value;
                tagString = (tag == null ? text : tag.Value);
                node = new SimpleNode(text, type, tagString);
//                node2 = new DefRefNode(def, tagString);
            }
            else if (main.Tag == "literal")
            {
                type = NodeType.literal;
                text = UnescapeForLiteralNode(main.Value);
                tagString = (tag == null ? text : tag.Value);
                node = new SimpleNode(text, type, tagString);
                SimpleNode prev = null;
                foreach (char ch in text)
                {
                    SimpleNode n = new SimpleNode(ch.ToString(), type, tagString);
                    snodes.Add(n);
                    if (prev != null)
                    {
                        prev.NextNodes.Add(n);
                    }
                    prev = n;
                }

//                node2 = new LiteralNode(
            }
            else if (main.Tag == "charclass")
            {
                type = NodeType.charclass;
                text = UndelimitForCharClass(main.Value);
                tagString = (tag == null ? text : tag.Value);
                node = new SimpleNode(text, type, tagString);
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (main.Tag != "literal")
            {
                if (modifier != null)
                {
                    if (modifier.Value == "*" ||
                        modifier.Value == "+")
                    {
                        node.NextNodes.Add(node);
                    }
                }

                frontNodes = backNodes = new SimpleNode[] { node };
            }
            else
            {
                frontNodes = new SimpleNode[]{snodes[0]};
                backNodes = new SimpleNode[]{snodes[snodes.Count-1]};
                if (modifier != null &&
                    (modifier.Value == "*" ||
                     modifier.Value == "+"))
                {
                    backNodes[0].NextNodes.Add(frontNodes[0]);
                }
            }
        }

        private void ProcessOrExpr(Span orexpr, out SimpleNode[] frontNodes, out SimpleNode[] backNodes, out Span modifier)
        {
            //orexpr = '(' expr:exprs[] ( '|' expr:exprs[] )* ')' modifier?;
            if (orexpr.Tag != "orexpr") throw new ArgumentException("orexpr");

            if (orexpr.Subspans[0].Value != "(") throw new InvalidOperationException();

            List<SimpleNode[]> fronts = new List<SimpleNode[]>();
            List<SimpleNode[]> backs = new List<SimpleNode[]>();
            int i = 1;
            for (i=1;i<orexpr.Subspans.Length;i++)
            {
                if (orexpr.Subspans[i].Tag != "expr") throw new InvalidOperationException();
                SimpleNode[] frontNodes2;
                SimpleNode[] backNodes2;
                bool skipAll;
                ProcessExpr(orexpr.Subspans[i], out frontNodes2, out backNodes2, out skipAll);
                fronts.Add(frontNodes2);
                backs.Add(backNodes2);
                //ignore skipAll for now
                i++;

                if (orexpr.Subspans[i].Value == ")")
                {
                    break;
                }
                if (orexpr.Subspans[i].Value != "|") throw new InvalidOperationException();
            }

            Set<SimpleNode> frontNodes3 = new Set<SimpleNode>();
            foreach (SimpleNode[] nodes in fronts)
            {
                frontNodes3.AddRange(nodes);
            }
            Set<SimpleNode> backNodes3 = new Set<SimpleNode>();
            foreach (SimpleNode[] nodes in backs)
            {
                backNodes3.AddRange(nodes);
            }

            if (orexpr.Subspans[i].Value != ")") throw new InvalidOperationException();
            i++;
            modifier = null;
            for (; i < orexpr.Subspans.Length; i++)
            {
                if (orexpr.Subspans[i].Tag == "modifier")
                {
                    modifier = orexpr.Subspans[i];
                    break;
                }
            }

            if (modifier != null)
            {
                if (modifier.Value == "*" ||
                    modifier.Value == "+")
                {
                    foreach (SimpleNode b in backNodes3)
                    {
                        b.NextNodes.AddRange(frontNodes3);
                    }
                }
            }

            frontNodes = frontNodes3.ToArray();
            backNodes = backNodes3.ToArray();
        }

        private static void CheckDefRefs(IEnumerable<SimpleDefinition> defs)
        {
            Set<string> defnames = new Set<string>();
            Set<string> duplicateNames = new Set<string>();
            Set<string> defrefs = new Set<string>();
            foreach (SimpleDefinition def in defs)
            {
                if (defnames.Contains(def.Name))
                {
                    duplicateNames.Add(def.Name);
                }
                else
                {
                    defnames.Add(def.Name);
                }
                SimpleNode[] nodes = GatherSimpleNodes(def.start);
                foreach (SimpleNode node in nodes)
                {
                    if (node.Type == NodeType.defref)
                    {
                        defrefs.Add(node.Text);
                    }
                }
            }

            string[] unknowndefs = Set<string>.Difference(defrefs.ToArray(), defnames.ToArray());

            if (duplicateNames.Count > 0 || unknowndefs.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                if (duplicateNames.Count > 0)
                {
                    sb.AppendFormat("There {0} {1} duplicate definition name{2}:",
                                    duplicateNames.Count == 1 ? "was" : "were",
                                    duplicateNames.Count,
                                    duplicateNames.Count == 1 ? "" : "s");
                    sb.AppendLine();
                    foreach (string name in duplicateNames)
                    {
                        sb.AppendFormat("    {0}", name);
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                }
                if (unknowndefs.Length > 0)
                {
                    sb.AppendFormat("There {0} {1} unknown definition name{2}:",
                                    unknowndefs.Length == 1 ? "was" : "were",
                                    unknowndefs.Length,
                                    unknowndefs.Length == 1 ? "" : "s");
                    sb.AppendLine();
                    foreach (string name in unknowndefs)
                    {
                        sb.AppendFormat("    {0}", name);
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                }

                throw new ApplicationException(sb.ToString());
            }
        }

        public static string[] MakeTagsUnique(string[] tags)
        {
            Dictionary<string, int> tagCounts = new Dictionary<string, int>();
            foreach (string tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    if (tagCounts.ContainsKey(tag))
                    {
                        tagCounts[tag]++;
                    }
                    else
                    {
                        tagCounts[tag] = 1;
                    }
                }
            }

            foreach (string tag in Collection.ToArray(tagCounts.Keys))
            {
                if (tagCounts[tag] < 2)
                {
                    tagCounts.Remove(tag);
                }
            }
            foreach (string tag in Collection.ToArray(tagCounts.Keys))
            {
                tagCounts[tag] = 0;
            }

            string[] newtags = new string[tags.Length];
            int i;
            for (i=0;i<tags.Length;i++)
            {
                string tag = tags[i];
                if (!string.IsNullOrEmpty(tag))
                {
                    if (tagCounts.ContainsKey(tag))
                    {
                        string tag2 = tag;
                        newtags[i] = tag2 + "_" + tagCounts[tag2].ToString();
                        tagCounts[tag2]++;
                    }
                    else
                    {
                        newtags[i] = tag;
                    }
                }
                else
                {
                    newtags[i] = string.Empty;
                }
            }

            return newtags;
        }

        public static Node[] GatherNodes(Node[] nodes)
        {
            Set<Node> set = new Set<Node>();
            foreach (Node node in nodes)
            {
                set.AddRange(GatherNodes(node));
            }
            return set.ToArray();
        }
        public static Node[] GatherNodes(Node start)
        {
            return Collection.Gather(start, GatherNodes_GetNextNodes);
        }
        public static Node[] GatherNodes_GetNextNodes(Node node)
        {
            return node.NextNodes.ToArray();
        }

        public static SimpleNode[] GatherSimpleNodes(SimpleNode[] nodes)
        {
            Set<SimpleNode> set = new Set<SimpleNode>();
            foreach (SimpleNode node in nodes)
            {
                set.AddRange(GatherSimpleNodes(node));
            }
            return set.ToArray();
        }
        public static SimpleNode[] GatherSimpleNodes(SimpleNode start)
        {
            return Collection.Gather(start, GatherSimpleNodes_GetNextNodes);
        }
        public static SimpleNode[] GatherSimpleNodes_GetNextNodes(SimpleNode node)
        {
            return node.NextNodes.ToArray();
        }

        public static string UnescapeForLiteralNode(string value)
        {
            value = value.Trim();
            List<char> chars = new List<char>(value.Length);
            int i;
            bool start = false;
            for (i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (start)
                {
                    if (ch == '\'')
                    {
                        break;
                    }
                    else if (ch == '\\')
                    {
                        i++;
                        chars.Add(value[i]);
                    }
                    else
                    {
                        chars.Add(ch);
                    }
                }
                else
                {
                    if (ch == '\'')
                    {
                        start = true;
                    }
                }
            }

            return new string(chars.ToArray());
        }

        public static string UndelimitForCharClass(string value)
        {
            if (value[0] != '[') { throw new ArgumentException("value must begin with an open bracket"); }

            List<char> chars = new List<char>(value.Length);
            int i;
            for (i = 1; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch == ']')
                {
                    break;
                }

                chars.Add(ch);

                if (ch == '\\' && i < value.Length - 1)
                {
                    i++;
                    chars.Add(value[i]);
                }
            }

            return new string(chars.ToArray());
        }

        public static string CleanTag(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "empty";
            }

            int n = text.Length - 1;
            if (n > 1 && text[0] == text[n] &&
                (text[0] == '\'' || text[0] == '"'))
            {
                return CleanTag(text.Substring(1, n - 1));
            }
            if (text[0] == '[' && text[n] == ']')
            {
                text = text.Substring(1, n - 1);
                text = text.Replace("\\l", "letter");
                text = text.Replace("\\w", "letter");
                text = text.Replace("\\d", "digit");
                text = text.Replace("\\s", "whitespace");
                return CleanTag(text);
            }

            if (text.Length == 1)
            {
                return TagFromChar(text[0]);
            }

            if (text == "\\l" || text == "\\w")
            {
                return "class_letter";
            }
            if (text == "\\d")
            {
                return "class_digit";
            }
            if (text == "\\s")
            {
                return "class_whitespace";
            }

            if (text.Length <= 3)
            {
                bool symbols = false;
                foreach (char ch in text)
                {
                    if (IsSymbol(ch))
                    {
                        symbols = true;
                        break;
                    }
                }
                if (symbols)
                {
                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    foreach (char ch in text)
                    {
                        if (first) first = false; else sb.Append("_");
                        sb.Append(TagFromChar(ch));
                    }
                    return sb.ToString();
                }
            }

            text = text.Replace('-', '_');
            text = Regex.Replace(text, @"[^\w\d_]", string.Empty);
            if (text.Length == 1)
            {
                return TagFromChar(text[0]);
            }
            if (string.IsNullOrEmpty(text))
            {
                return "symbols";
            }

            if (char.IsDigit(text[0]))
            {
                text = "_" + text;
            }
            return text;
        }

        public static bool IsSymbol(char ch)
        {
            switch (ch)
            {
                case '~': return true;
                case '`': return true;
                case '!': return true;
                case '@': return true;
                case '#': return true;
                case '$': return true;
                case '%': return true;
                case '^': return true;
                case '&': return true;
                case '*': return true;
                case '(': return true;
                case ')': return true;
                case '-': return true;
                case '_': return true;
                case '=': return true;
                case '+': return true;
                case '[': return true;
                case ']': return true;
                case '{': return true;
                case '}': return true;
                case '\\': return true;
                case '|': return true;
                case ';': return true;
                case '\'': return true;
                case ':': return true;
                case '"': return true;
                case ',': return true;
                case '.': return true;
                case '<': return true;
                case '>': return true;
                case '/': return true;
                case '?': return true;
            }

            return false;
        }

        public static string TagFromChar(char ch)
        {
            if (char.IsDigit(ch)) { return "number_" + ch.ToString(); }
            if (char.IsLetter(ch)) { return "letter_" + ch.ToString(); }
            switch (ch)
            {
                case '\t': return "tab";
                case '\r': return "carreturn";
                case '\n': return "newline";
                case ' ': return "space";
                case '~': return "tilde";
                case '`': return "tick";
                case '!': return "exclamation";
                case '@': return "at";
                case '#': return "hash";
                case '$': return "dollar";
                case '%': return "percent";
                case '^': return "chevron";
                case '&': return "amp";
                case '*': return "star";
                case '(': return "oparen";
                case ')': return "cparen";
                case '-': return "hyphen";
                case '_': return "score";
                case '=': return "equal";
                case '+': return "plus";
                case '[': return "obracket";
                case ']': return "cbracket";
                case '{': return "obrace";
                case '}': return "cbrace";
                case '\\': return "bslash";
                case '|': return "pipe";
                case ';': return "semi";
                case '\'': return "quote";
                case ':': return "colon";
                case '"': return "dquote";
                case ',': return "comma";
                case '.': return "period";
                case '<': return "less";
                case '>': return "greater";
                case '/': return "slash";
                case '?': return "question";
            }

            short sh = (short)ch;
            return "bits" + sh.ToString("X4");
        }
    }
}
