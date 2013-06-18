using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class GrammarComparer
    {
//        public bool AreEquivalent(Grammar a, Grammar b)
//        {
//            if (a.Definitions.Count != b.Definitions.Count) return false;
//
//            Dictionary<Definition, Definition> defMatchup = new Dictionary<Definition, Definition>();
//            int i;
//            for (i = 0; i < a.Definitions.Count; i++)
//            {
//                defMatchup[a.Definitions[i]] = b.Definitions[i];
//                defMatchup[b.Definitions[i]] = a.Definitions[i];
//            }
//
//            for (i = 0; i < a.Definitions.Count; i++)
//            {
//                if (!AreEquivalent(a.Definitions[i], b.Definitions[i], defMatchup)) return false;
//            }
//
//            return true;
//        }

        public bool AreEquivalent(Definition a, Definition b, Dictionary<Definition, Definition> defmatch)
        {
            if (a.Name != b.Name) return false;

            if (a.Directives.Count != b.Directives.Count) return false;
            foreach (DefinitionDirective dir in a.Directives)
            {
                if (!b.Directives.Contains(dir)) return false;
            }

            if (a.Nodes.Count != b.Nodes.Count) return false;
            int i;
            Dictionary<Node, Node> nodematchup = new Dictionary<Node, Node>();
            for (i = 0; i < a.Nodes.Count; i++)
            {
                nodematchup[a.Nodes[i]] = b.Nodes[i];
                nodematchup[b.Nodes[i]] = a.Nodes[i];
            }
            for (i = 0; i < a.Nodes.Count; i++)
            {
                if (!AreEquivalent(a.Nodes[i], b.Nodes[i], defmatch, nodematchup)) return false;
            }

            if (a.StartNodes.Count != b.StartNodes.Count) return false;
            foreach (Node n in a.StartNodes)
            {
                if (!b.StartNodes.Contains(nodematchup[n])) return false;
            }

            if (a.EndNodes.Count != b.EndNodes.Count) return false;
            foreach (Node n in a.EndNodes)
            {
                if (!b.EndNodes.Contains(nodematchup[n])) return false;
            }

            return true;
        }

        bool AreEquivalent(Node a, Node b, Dictionary<Definition, Definition> defmatchup, Dictionary<Node, Node> nodematchup)
        {
            if (a.GetType() != b.GetType()) return false;

            if (a is CharNode)
            {
                CharNode aa = (CharNode)a;
                CharNode bb = (CharNode)b;

                if (aa.CharClass.ToUndelimitedString() != bb.CharClass.ToUndelimitedString()) return false;
            }
            else // (a is DefRefNode)
            {
                DefRefNode aa = (DefRefNode)a;
                DefRefNode bb = (DefRefNode)b;

                if (aa.DefRef != defmatchup[bb.DefRef]) return false;
            }

//            if (a.Tag != b.Tag) return false;

            if (a.NextNodes.Count != b.NextNodes.Count) return false;
            foreach (Node next in a.NextNodes)
            {
                if (!b.NextNodes.Contains(nodematchup[next])) return false;
            }

            return true;
        }
    }
}

