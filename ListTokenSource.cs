using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class ListTokenSource : ITokenSource
    {
        public ListTokenSource(Grammar grammar, params string[] intokens)
            : this(grammar, (IEnumerable<string>)intokens)
        {
        }
        public ListTokenSource(Grammar grammar, IEnumerable<string> intokens)
        {
            _grammar = grammar.Clone();
            _tokenDef = new Definition("$token");
            _grammar.Definitions.Add(_tokenDef);

            foreach (Definition def in grammar.Definitions)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    var node = new DefRefNode(def);
                    _tokenDef.Nodes.Add(node);
                    _tokenDef.StartNodes.Add(node);
                    _tokenDef.EndNodes.Add(node);
                }
            }

            _spanner = new Spanner(_tokenDef);
            _intokens = new List<string>(intokens);

            foreach (var intoken in intokens)
            {
                _tokens.Add(new Token(value: null));
            }
        }

        Grammar _grammar;
        Spanner _spanner;
        Definition _tokenDef;
        List<string> _intokens;
        List<Token> _tokens = new List<Token>();

        public TokenizationInfo GetTokensAtLocation(int index)
        {
            TokenizationInfo tinfo = new TokenizationInfo();

            if (index >= _intokens.Count)
            {
                tinfo.EndOfInput = true;
                tinfo.EndOfInputPosition = new InputPosition(_intokens.Count);
                tinfo.Tokens = new Token[0];
                return tinfo;
            }

            tinfo.EndOfInput = false;
            tinfo.EndOfInputPosition = new InputPosition(-1);

            if (_tokens[index].Value != null)
            {
                tinfo.Tokens = new Token[] { _tokens[index] };
                return tinfo;
            }

            var nodeMatches = _spanner.Match(_intokens[index], tinfo.Errors);
            var leaf = nodeMatches[0];

            NodeMatch tokenEnd = leaf.Previous;
            NodeMatch tokenStart = tokenEnd.StartDef;

            int length = leaf.StartPosition.Index - tokenStart.StartPosition.Index + 1;
            var token = new Token(
                definition: tokenEnd.Previous.Node.ParentDefinition,
                startPosition: tokenStart.StartPosition,
                value: _intokens[index].Substring(tokenStart.StartPosition.Index, length)
            );

            _tokens[index] = token;

            tinfo.Tokens = new Token[] { _tokens[index] };
            return tinfo;
        }
    }
}

