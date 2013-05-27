using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MetaphysicsIndustries.Giza
{
    public class DefinitionBuilder
    {
        enum DefmodItem
        {
            MindWhitespace,
            IgnoreCase,
            Atomic,
        }

        public Definition[] BuildDefinitions(Supergrammar grammar, Span span)
        {
            if (!(span.Node is DefRefNode) ||
                (span.Node as DefRefNode).DefRef != grammar.def_0_grammar) 
            {
                throw new InvalidOperationException();
            }

            SpanChecker sc = new SpanChecker();
            List<SpanChecker.Error> errors = sc.CheckSpan(span, grammar);
            if (errors.Count > 0) throw new InvalidOperationException();

            List<Definition> defs = new List<Definition>();
            Dictionary<Definition, Span> matchup = new Dictionary<Definition, Span>();
            Dictionary<string, Definition> defsByName = new Dictionary<string, Definition>();

            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_grammar_0_definition)
                {
                    Definition def = new Definition();
                    defs.Add(def);
                    matchup[def] = sub;

                    foreach (Span sub2 in sub.Subspans)
                    {
                        if (sub2.Node == grammar.node_definition_1_identifier)
                        {
                            def.Name = GetIdentifier(grammar, sub2);
                            break;
                        }
                    }
                    defsByName[def.Name] = def;
                }
            }

            foreach (Definition def in defs)
            {
                Span defspan = matchup[def];
                Set<DefmodItem> defmodItems = new Set<DefmodItem>();
                foreach (Span sub in defspan.Subspans)
                {
                    if (sub.Node == grammar.node_definition_0_defmod)
                    {
                        defmodItems.AddRange(GetDefMods(grammar, sub));
                    }
                    else if (sub.Node == grammar.node_definition_3_expr)
                    {
                        NodeBundle bundle = GetNodesFromExpr(grammar, sub, defsByName);

                        if (bundle.IsSkippable) throw new InvalidOperationException();

                        def.StartNodes.AddRange(bundle.StartNodes);

                        def.Nodes.AddRange(bundle.Nodes);

                        def.EndNodes.AddRange(bundle.EndNodes);
                    }
                }

                def.IgnoreWhitespace = true;
                def.IgnoreCase = false;
                def.Atomic = false;
                foreach (DefmodItem item in defmodItems)
                {
                    switch (item)
                    {
                    case DefmodItem.MindWhitespace: def.IgnoreWhitespace = false; break;
                    case DefmodItem.IgnoreCase: def.IgnoreCase = true; break;
                    case DefmodItem.Atomic: def.Atomic = true; break;
                    }
                }
            }

            return defs.ToArray();
        }

        IEnumerable<DefmodItem> GetDefMods(Supergrammar grammar, Span span)
        {
            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_defmod_1_defmod_002D_item ||
                    sub.Node == grammar.node_defmod_3_defmod_002D_item)
                {
                    yield return GetDefModItem(grammar, sub);
                }
            }
        }

        DefmodItem GetDefModItem(Supergrammar grammar, Span span)
        {
            if (span.Subspans[0].Node == grammar.node_defmod_item_0_id_002D_whitespace)
            {
                return DefmodItem.MindWhitespace;
            }
            if (span.Subspans[0].Node == grammar.node_defmod_item_1_id_002D_ignore)
            {
                if (span.Subspans[1].Node == grammar.node_defmod_item_3_id_002D_case ||
                    span.Subspans[2].Node == grammar.node_defmod_item_3_id_002D_case)
                {
                    return DefmodItem.IgnoreCase;
                }
            }
            if (span.Subspans[0].Node == grammar.node_defmod_item_4_id_002D_atomic)
            {
                return DefmodItem.Atomic;
            }

            throw new InvalidOperationException();
        }

        class NodeBundle
        {
            public Set<Node> StartNodes;
            public Set<Node> EndNodes;
            public List<Node> Nodes;
            public bool IsSkippable = false;
        }

        NodeBundle GetNodesFromExpr(Supergrammar grammar, Span exprSpan, Dictionary<string, Definition> defsByName)
        {
            NodeBundle first = null;
            NodeBundle last = null;
            List<NodeBundle> bundles = new List<NodeBundle>();
            foreach (Span sub in exprSpan.Subspans)
            {
                NodeBundle bundle = null;
                if (sub.Node == grammar.node_expr_0_subexpr)
                {
                    bundle = GetNodesFromSubExpr(grammar, sub, defsByName);
                }
                else if (sub.Node == grammar.node_expr_1_orexpr)
                {
                    bundle = GetNodesFromOrExpr(grammar, sub, defsByName);
                }

                if (bundle != null)
                {
                    if (first == null)
                    {
                        first = bundle;
                    }

                    last = bundle;

                    bundles.Add(bundle);
                }
            }

            Set<Node> starts = new Set<Node>();
            Set<Node> ends = new Set<Node>();
            List<Node> nodes = new List<Node>();

            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
            }
            starts.AddRange(first.StartNodes);
            ends.AddRange(last.EndNodes);

            // connect the nodes
            int i;
            // inter-bundle connections
            for (i = 1; i < bundles.Count; i++)
            {
                foreach (Node prev in bundles[i-1].EndNodes)
                {
                    prev.NextNodes.AddRange(bundles[i].StartNodes);
                }
            }

            // inter-bundle skips
            for (i = 2; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    foreach (Node prev in bundles[i-2].EndNodes)
                    {
                        prev.NextNodes.AddRange(bundles[i].StartNodes);
                    }
                }
            }

            // skip from start to inner bundle
            bool skippable = true;
            for (i = 1; i < bundles.Count; i++)
            {
                if (bundles[i - 1].IsSkippable)
                {
                    starts.AddRange(bundles[i].StartNodes);
                }
                else
                {
                    skippable = false;
                    break;
                }
            }
            if (skippable)
            {
                if (bundles[bundles.Count - 1].IsSkippable)
                {
                }
                else
                {
                    skippable = false;
                }
            }

            // skip from inner bundle to end
            for (i = bundles.Count - 1; i > 0; i--)
            {
                if (bundles[i].IsSkippable)
                {
                    ends.AddRange(bundles[i - 1].EndNodes);
                }
                else
                {
                    break;
                }
            }

            return new NodeBundle{StartNodes = starts,
                                  EndNodes = ends,
                                  Nodes = nodes,
                                  IsSkippable = skippable};
        }

        NodeBundle GetNodesFromSubExpr(Supergrammar grammar, Span span, Dictionary<string, Definition> defsByName)
        {
            bool skippable = false;
            bool loop = false;
            string tag = null;
            List<Node> nodes = null;

            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_subexpr_0_identifier)
                {
                    Node node = GetNodeFromIdentifier(grammar, sub, defsByName);
                    nodes = new List<Node>{node};
                }
                else if (sub.Node == grammar.node_subexpr_1_literal)
                {
                    nodes = GetNodesFromLiteral(grammar, sub);
                }
                else if (sub.Node == grammar.node_subexpr_2_charclass)
                {
                    Node node = GetNodeFromCharClass(grammar, sub);
                    nodes = new List<Node>{node};
                }
                else if (sub.Node == grammar.node_subexpr_3_modifier)
                {
                    char mod = GetModifier(grammar, sub);
                    switch (mod)
                    {
                    case '?': skippable = true; loop = false; break;
                    case '+': skippable = false; loop = true; break;
                    case '*': skippable = true; loop = true; break;
                    }
                }
                else if (sub.Node == grammar.node_subexpr_4__003A_) // ':'
                {
                    //skip it
                    continue;
                }
                else if (sub.Node == grammar.node_subexpr_5_tag)
                {
                    tag = GetIdentifier(grammar, sub);
                }
            }

            if (tag != null)
            {
                foreach (Node node in nodes)
                {
                    node.SetTag(tag);
                }
            }

            int i;
            for (i = 1; i < nodes.Count; i++)
            {
                nodes[i-1].NextNodes.Add(nodes[i]);
            }

            Node start = nodes[0];
            Node end = nodes[nodes.Count - 1];
            if (loop)
            {
                end.NextNodes.Add(start);
            }

            return
                new NodeBundle {
                    StartNodes = new Set<Node>{ start },
                    EndNodes = new Set<Node>{ end },
                    Nodes = nodes,
                    IsSkippable = skippable};
        }

        Node GetNodeFromIdentifier(Supergrammar grammar, Span span, Dictionary<string, Definition> defsByName)
        {
            string ident = GetIdentifier(grammar, span);
            return new DefRefNode(defsByName[ident], ident);
        }

        List<Node> GetNodesFromLiteral(Supergrammar grammar, Span span)
        {
            List<Node> nodes = new List<Node>();
            List<char> tagNodes = new List<char>();

            int i;
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];
                if (sub.Node == grammar.node_literal_0__0027_) continue;
                if (sub.Node == grammar.node_literal_5__0027_) continue;

                char ch = ' ';
                if (sub.Node == grammar.node_literal_4_unicodechar)
                {
                    ch = GetUnicodeChar(grammar, sub);
                }
                else if (sub.Node == grammar.node_literal_1__005E__005C__005C__0027_) // [^\\']
                {
                    ch = sub.Value[0];
                }
                else if (sub.Node == grammar.node_literal_2__005C_) // '\\'
                {
                    Span sub2 = span.Subspans[i+1];

                    if (sub2.Value == "r") ch = '\r';
                    else if (sub2.Value == "n") ch = '\n';
                    else if (sub2.Value == "t") ch = '\t';
                    else if (sub2.Value == "\\") ch = '\\';
                    else if (sub2.Value == "'") ch = '\'';

                    i++;
                }
                nodes.Add(new CharNode(ch));
                tagNodes.Add(ch);
            }

            string tag = new string(tagNodes.ToArray());
            foreach (Node node in nodes)
            {
                node.SetTag(tag);
            }

            return nodes;
        }

        char GetUnicodeChar(Supergrammar grammar, Span span)
        {
            string s =
                span.Subspans[2].Value +
                span.Subspans[3].Value +
                span.Subspans[4].Value +
                span.Subspans[5].Value;

            int i;
            if (!int.TryParse(s, NumberStyles.HexNumber, null, out i)) throw new InvalidOperationException();

            return (char)i;
        }

        Node GetNodeFromCharClass(Supergrammar grammar, Span span)
        {
            int i;
            List<char> items = new List<char>();
            for (i = 1; i < span.Subspans.Count - 1; i++)
            {
                Span sub = span.Subspans[i];

                if (sub.Node == grammar.node_charclass_1__005E__005C__005C__005C__005B__005C__005D_) // [^\\\[\]]
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_2__005C_) // '\\'
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_) // [wldsrnt\\\[\]]
                {
                    items.Add(sub.Value[0]);
                }
                else if (sub.Node == grammar.node_charclass_4_unicodechar)
                {
                    items.Add(GetUnicodeChar(grammar, sub));
                }
            }

            string charClassText = new string(items.ToArray());

            return new CharNode(CharClass.FromUndelimitedCharClassText(charClassText));
        }

        char GetModifier(Supergrammar grammar, Span span)
        {
            return span.Subspans[0].Value[0];
        }

        string GetIdentifier(Supergrammar grammar, Span span)
        {
            List<char> chs = new List<char>();
            foreach (Span sub in span.Subspans)
            {
                chs.Add(sub.Value[0]);
            }
            return new string(chs.ToArray());
        }

        NodeBundle GetNodesFromOrExpr(Supergrammar grammar, Span span, Dictionary<string, Definition> defsByName)
        {
            List<NodeBundle> bundles = new List<NodeBundle>();
            bool skippable = false;
            bool loopable = false;

            foreach (Span sub in span.Subspans)
            {
                if (sub.Node == grammar.node_orexpr_1_expr ||
                    sub.Node == grammar.node_orexpr_3_expr)
                {
                    bundles.Add(GetNodesFromExpr(grammar, sub, defsByName));
                }
                else if (sub.Node == grammar.node_orexpr_5_modifier)
                {
                    switch (GetModifier(grammar, sub))
                    {
                    case '*': loopable = true; skippable = true; break;
                    case '?': skippable = true; break;
                    case '+': loopable = true; break;
                    }
                }
            }

            Set<Node> starts = new Set<Node>();
            Set<Node> ends = new Set<Node>();
            List<Node> nodes = new List<Node>();
            foreach (NodeBundle bundle in bundles)
            {
                nodes.AddRange(bundle.Nodes);
                starts.AddRange(bundle.StartNodes);
                ends.AddRange(bundle.EndNodes);
                if (bundle.IsSkippable)
                {
                    skippable = true;
                }
            }

            if (loopable)
            {
                foreach (Node end in ends)
                {
                    end.NextNodes.AddRange(starts);
                }
            }

            return new NodeBundle{
                StartNodes=starts,
                EndNodes=ends,
                Nodes=nodes,
                IsSkippable=skippable
            };
        }
    }
}
