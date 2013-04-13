using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionBuilder
    {
        public SimpleDefinitionNode[] BuildDefinitions(Span span)
        {
            if (span.Tag != "grammar") { throw new ArgumentException("span"); }

            List<SimpleDefinitionNode> defs = new List<SimpleDefinitionNode>();
            foreach (Span subspan in span.Subspans)
            {
                if (subspan.Tag == "comment") continue;

                SimpleDefinitionNode def = BuildSingleDefinition(subspan);
                defs.Add(def);
            }
            return defs.ToArray();
        }

        SimpleDefinitionNode BuildSingleDefinition(Span span)
        {
            if (span.Tag != "definition") { throw new ArgumentException("span"); }

            SimpleDefinitionNode def = new SimpleDefinitionNode();
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

            def.Nodes.AddRange(SpannerServices.GatherSimpleNodes(start));

            return def;
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
                text = SpannerServices.UnescapeForLiteralNode(main.Value);
                tagString = (tag == null ? text : tag.Value);
                node = new SimpleNode(text, type, tagString);
//                node2 = new LiteralNode(
            }
            else if (main.Tag == "charclass")
            {
                type = NodeType.charclass;
                text = SpannerServices.UndelimitForCharClass(main.Value);
                tagString = (tag == null ? text : tag.Value);
                node = new SimpleNode(text, type, tagString);
            }
            else
            {
                throw new InvalidOperationException();
            }

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
    }
}
