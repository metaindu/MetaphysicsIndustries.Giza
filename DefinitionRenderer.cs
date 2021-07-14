
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
using System.Text.RegularExpressions;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionRenderer
    {
        public string RenderDefinitionsAsCSharpClass(
            string className, IEnumerable<NDefinition> defs, string ns=null,
            bool singleton=false, string baseClassName="NGrammar",
            IEnumerable<string> usings=null, bool skipImported=false)
        {
            var defsSorted = defs.ToList();
            defsSorted.Sort((a, b) =>
                string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            IEnumerable<NDefinition> defsToRender;
            if (skipImported)
                defsToRender = defsSorted.Where(d => !d.IsImported).ToList();
            else
                defsToRender = defsSorted;

            var defnames = new Dictionary<NDefinition, string>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");

            HashSet<string> usings2 = new HashSet<string>();
            if (usings != null)
                usings2.AddRange(usings);
            if (ns != "MetaphysicsIndustries.Giza")
                usings2.Add("MetaphysicsIndustries.Giza");
            var usings3 = usings2.ToList();
            usings3.Sort();
            foreach (var u in usings3)
                sb.AppendLine($"using {u};");
            sb.AppendLine();

            string indent;
            if (string.IsNullOrEmpty(ns))
            {
                indent = string.Empty;
            }
            else
            {
                indent = "    ";
                sb.AppendFormat("namespace {0}", ns);
                sb.AppendLine();
                sb.AppendLine("{");
            }

            sb.Append(indent);
            sb.AppendFormat("public class {0} : {1}",
                RenderIdentifier(className), baseClassName);
            sb.AppendLine();
            sb.Append(indent);
            sb.AppendLine("{");

            if (singleton)
            {
                sb.Append(indent);
                sb.AppendFormat("    public static readonly {0} Instance = new {0}();", RenderIdentifier(className));
                sb.AppendLine();
                sb.AppendLine();
            }

            foreach (var def in defsSorted)
            {
                string name = string.Format("def_{0}", RenderIdentifier(def.Name));
                defnames[def] = name;
            }
            foreach (var def in defsToRender)
            {
                var name = defnames[def];

                sb.Append(indent);
                sb.AppendFormat(
                    "    public NDefinition {0} = new NDefinition({1});",
                    name,
                    RenderString(def.Name));
                sb.AppendLine();
            }

            sb.AppendLine();

            Dictionary<Node, string> nodenames = new Dictionary<Node, string>();

            foreach (var def in defsToRender)
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

                    sb.Append(indent);
                    sb.AppendFormat("    public {0} {1};", type, name);
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            sb.Append(indent);
            sb.AppendFormat("    public {0}()", RenderIdentifier(className));
            sb.AppendLine();
            sb.Append(indent);
            sb.AppendLine("    {");

            string indent2 = indent + "        ";
            foreach (var def in defsToRender)
            {
                sb.Append(indent2);
                sb.AppendFormat("Definitions.Add({0});", defnames[def]);
                sb.AppendLine();
            }

            sb.AppendLine();

            foreach (var def in defsToRender)
            {
                foreach (DefinitionDirective dd in def.Directives)
                {
                    sb.Append(indent2);
                    sb.AppendFormat("{0}.Directives.Add(DefinitionDirective.{1});", defnames[def], dd);
                    sb.AppendLine();
                }

                foreach (Node node in def.Nodes)
                {
                    string name = nodenames[node];

                    sb.Append(indent2);
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
                    sb.Append(indent2);
                    sb.AppendFormat("{0}.Nodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }

                foreach (Node node in def.StartNodes)
                {
                    sb.Append(indent2);
                    sb.AppendFormat("{0}.StartNodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }
                foreach (Node node in def.EndNodes)
                {
                    sb.Append(indent2);
                    sb.AppendFormat("{0}.EndNodes.Add({1});", defnames[def], nodenames[node]);
                    sb.AppendLine();
                }

                foreach (Node node in def.Nodes)
                {
                    foreach (Node next in node.NextNodes)
                    {
                        sb.Append(indent2);
                        sb.AppendFormat("{0}.NextNodes.Add({1});", nodenames[node], nodenames[next]);
                        sb.AppendLine();
                    }
                }
                sb.AppendLine();
            }

            sb.Append(indent);
            sb.AppendLine("    }");
            sb.Append(indent);
            sb.AppendLine("}");

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("}");
            }

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

        public string RenderDefinitionExprsAsGrammarText(IEnumerable<DefinitionExpression> defs, int? maxLineWidth=null)
        {
            var sb = new StringBuilder();
            var noSpaceTokens = new string[] { ";", ":", "*", "?", "+", ">", "," };

            foreach (var def in defs)
            {
                int p = 0;
                string prevtoken = string.Empty;

                foreach (var token in ProcessDefinitionExpression(def))
                {
                    if (maxLineWidth.HasValue &&
                        p + 1 + token.Length > maxLineWidth.Value)
                    {
                        sb.AppendLine();
                        p = 0;
                    }
                    else if (p != 0 && 
                        !noSpaceTokens.Contains(token) && 
                        prevtoken != ":" &&
                        prevtoken != "<")
                    {
                        sb.Append(" ");
                        p++;
                    }

                    sb.Append(token);
                    p += token.Length;

                    prevtoken = token;
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public IEnumerable<string> ProcessDefinitionExpression(DefinitionExpression defexpr)
        {
            if (defexpr.Directives.Count > 0)
            {
                yield return "<";

                bool first = true;
                foreach (var dir in defexpr.Directives)
                {
                    if (!first)
                    {
                        yield return ",";
                    }
                    first = false;

                    switch (dir)
                    {
                    case DefinitionDirective.MindWhitespace:
                        yield return "mind";
                        yield return "whitespace";
                        break;
                    case DefinitionDirective.IgnoreCase:
                        yield return "ignore";
                        yield return "case";
                        break;
                    case DefinitionDirective.Atomic:
                        yield return "atomic";
                        break;
                    case DefinitionDirective.Token:
                        yield return "token";
                        break;
                    case DefinitionDirective.Subtoken:
                        yield return "subtoken";
                        break;
                    case DefinitionDirective.Comment:
                        yield return "comment";
                        break;
                    }
                }

                yield return ">";
            }

            yield return defexpr.Name;
            yield return "=";
            foreach (var token in ProcessExpression(defexpr.Expr))
            {
                yield return token;
            }
            yield return ";";
        }
        public IEnumerable<string> ProcessExpression(Expression expr)
        {
            return expr.Items.SelectMany(item => ProcessExpressionItem(item));
        }
        public IEnumerable<string> ProcessExpressionItem(ExpressionItem item)
        {
            var subexpr = item as SubExpression;
            if (subexpr != null)
            {
                return ProcessSubExpression(subexpr);
            }

            var orexpr = item as OrExpression;
            if (orexpr != null)
            {
                return ProcessOrExpression(orexpr);
            }

            throw new InvalidOperationException(string.Format("Unknown type: {0}", item.GetType().ToString()));
        }
        public IEnumerable<string> ProcessSubExpression(SubExpression subexpr)
        {
            string token = null;
            string defaultTag = null;
            var defref = subexpr as DefRefSubExpression;
            var literal = subexpr as LiteralSubExpression;
            var cc = subexpr as CharClassSubExpression;
            if (defref != null)
            {
                token = defref.DefinitionName;
                defaultTag = token;
            }
            else if (literal != null)
            {
                token = literal.ToDelimitedString();
                defaultTag = literal.Value;
            }
            else if (cc != null)
            {
                token = cc.CharClass.ToString();
                defaultTag = cc.CharClass.ToUndelimitedString();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown type: {0}", subexpr.GetType().ToString()));
            }

            yield return token;

            if (subexpr.IsSkippable && subexpr.IsRepeatable)
            {
                yield return "*";
            }
            else if (subexpr.IsSkippable)
            {
                yield return "?";
            }
            else if (subexpr.IsRepeatable)
            {
                yield return "+";
            }

            if (!string.IsNullOrEmpty(subexpr.Tag) && subexpr.Tag != defaultTag)
            {
                yield return ":";
                yield return subexpr.Tag;
            }
        }
        public IEnumerable<string> ProcessOrExpression(OrExpression orexpr)
        {
            bool first = true;
            foreach (var expr in orexpr.Expressions)
            {
                if (first)
                {
                    yield return "(";
                    first = false;
                }
                else
                {
                    yield return "|";
                }

                foreach (var token in ProcessExpression(expr))
                {
                    yield return token;
                }
            }

            yield return ")";

            if (orexpr.IsSkippable && orexpr.IsRepeatable)
            {
                yield return "*";
            }
            else if (orexpr.IsSkippable)
            {
                yield return "?";
            }
            else if (orexpr.IsRepeatable)
            {
                yield return "+";
            }
        }
    }
}

