using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Text.RegularExpressions;

namespace MetaphysicsIndustries.Giza
{
    public static class SpannerServices
    {
        public static Definition[] PrepareDefinitions(SimpleDefinition[] defs)
        {
            //converts SimpleNodes to Nodes and SimpleDefinitionNodes to DefinitionNodes,
            //makes connections from defref nodes to the appropriate definitions

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
                    if (node2.Type == NodeType.literal)
                    {
                        LiteralNode literalnode = (LiteralNode)node2;
                        while (literalnode.NextNodes.Count == 1 &&
                            literalnode.NextNodes.GetFirst().Type == NodeType.literal)
                        {
                            literalnode = (LiteralNode)literalnode.NextNodes.GetFirst();
                        }
                        node2 = literalnode;
                    }
                    foreach (SimpleNode next in node.NextNodes)
                    {
                        node2.NextNodes.Add(nodeMatchup[next]);
                    }
                }
            }


            return defs2.ToArray();
        }

        private static void CheckDefRefs(SimpleDefinition[] defs)
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
                SimpleNode[] nodes = SpannerServices.GatherSimpleNodes(def.start);
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
            return Collection.Gather(start, SpannerServices.GatherNodes_GetNextNodes);
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
            return Collection.Gather(start, SpannerServices.GatherSimpleNodes_GetNextNodes);
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
