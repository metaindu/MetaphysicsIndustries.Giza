using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class Tokenizer
    {
        public Tokenizer(Grammar grammar)
        {
            _tokenDef = new Definition("$token");

            foreach (Definition def in grammar.Definitions)
            {
                if (def.Directives.Contains(DefinitionDirective.Token))
                {
                    DefRefNode node = new DefRefNode(def);
                    _tokenDef.Nodes.Add(node);
                    _tokenDef.StartNodes.Add(node);
                    _tokenDef.EndNodes.Add(node);
                }
            }
        }

        Spanner _spanner = new Spanner();
        Definition _tokenDef;

        public Token[] GetTokensAtLocation(Grammar grammar, string input, int index)
        {
            string error;
            Spanner.NodeMatch[] matchTreeLeaves = _spanner.Match(_tokenDef, input, out error);

            throw new NotImplementedException();
        }
    }
}

