using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionRenderer
    {
        public string RenderDefinitionsAsCSharpClass(string className, IEnumerable<Definition> defs)
        {
            Dictionary<Definition, string> defnames = new Dictionary<Definition, string>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace MetaphysicsIndustries.Giza");
            sb.AppendLine("{");
            sb.AppendFormat("    public class {0} : Grammar", RenderIdentifier(className));
            sb.AppendLine();
            sb.AppendLine("    {");

            foreach (Definition def in defs)
            {
                string name = string.Format("def_{0}_{1}", def.ID, RenderIdentifier(def.Name));

                defnames[def] = name;

                sb.AppendFormat(
                    "        public Definition {0} = new Definition({1});",
                    name,
                    RenderString(def.Name));
                sb.AppendLine();
            }

            sb.AppendLine();

            Dictionary<Node, string> nodenames = new Dictionary<Node, string>();

            foreach (Definition def in defs)
            {
                foreach (Node node in def.Nodes)
                {
                    string name =
                        string.Format(
                            "node_{0}_{1}_{2}",
                            RenderIdentifier(def.Name),
                            node.ID,
                            RenderIdentifier(node.Tag));

                    nodenames[node] = name;

                    string type = "";
                    if (node is CharNode)
                    {
                        type = "CharNode";
                    }
                    else // (node is DefRefNode)
                    {
                        type = "DefRefNode";
                    }

                    sb.AppendFormat("        public {0} {1};", type, name);
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            sb.AppendFormat("        public {0}()", RenderIdentifier(className));
            sb.AppendLine();
            sb.AppendLine("        {");

            string indent = "            ";
            foreach (Definition def in defs)
            {
                sb.Append(indent);
                sb.AppendFormat("Definitions.Add({0});", defnames[def]);
                sb.AppendLine();
            }

            sb.AppendLine();

            foreach (Definition def in defs)
            {
                foreach (DefinitionDirective dd in def.Directives)
                {
                    sb.Append(indent);
                    sb.AppendFormat("{0}.Directives.Add(DefinitionDirective.{1});", defnames[def], dd);
                    sb.AppendLine();
                }

                foreach (Node node in def.Nodes)
                {
                    string name = nodenames[node];

                    sb.Append(indent);
                    if (node is CharNode)
                    {
                        sb.AppendFormat(
                            "{0} = new CharNode(CharClass.FromUndelimitedCharClassText({1}), {2});",
                            name,
                            RenderString((node as CharNode).CharClass.ToUndelimitedString()),
                            RenderString(node.Tag));
                    }
                    else // (node is DefRefNode)
                    {
                        sb.AppendFormat(
                            "{0} = new DefRefNode({1}, {2});",
                            name,
                            defnames[(node as DefRefNode).DefRef],
                            RenderString(node.Tag));
                    }
                    sb.AppendLine();
                }
                foreach (Node node in def.Nodes)
                {
                    sb.Append(indent);
                    sb.AppendFormat("{0}.Nodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }

                foreach (Node node in def.StartNodes)
                {
                    sb.Append(indent);
                    sb.AppendFormat("{0}.StartNodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }
                foreach (Node node in def.EndNodes)
                {
                    sb.Append(indent);
                    sb.AppendFormat("{0}.EndNodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }

                foreach (Node node in def.Nodes)
                {
                    foreach (Node next in node.NextNodes)
                    {
                        sb.Append(indent);
                        sb.AppendFormat("{0}.NextNodes.Add({1});", nodenames[node], nodenames[next]);
                        sb.AppendLine();
                    }
                }
                sb.AppendLine();
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        static string RenderIdentifier(string name)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char ch in name)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(ch.ToString());
                }
                else
                {
                    sb.Append("_");
                    sb.Append(Convert.ToInt32(ch).ToString("X4"));
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }

        static string RenderString(string s)
        {
            string s2 = s;

            s2 = s2.Replace("\\", "\\\\");
            s2 = s2.Replace("\"", "\\\"");
            s2 = s2.Replace("\r", "\\r");
            s2 = s2.Replace("\n", "\\n");
            s2 = s2.Replace("\t", "\\t");

            return "\"" + s2 + "\"";
        }
    }
}

