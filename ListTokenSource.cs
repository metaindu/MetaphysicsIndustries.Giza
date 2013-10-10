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
                _tokens.Add(new Token { Value = null });
            }
        }

        Grammar _grammar;
        Spanner _spanner;
        Definition _tokenDef;
        List<string> _intokens;
        List<Token> _tokens = new List<Token>();

        public IEnumerable<Token> GetTokensAtLocation(int index,
                                                      List<Error> errors,
                                                      out bool endOfInput,
                                                      out int endOfInputIndex)
        {
            if (index >= _intokens.Count)
            {
                endOfInput = true;
                endOfInputIndex = _intokens.Count;
                return new Token[0];
            }

            endOfInput = false;
            endOfInputIndex = -1;

            if (_tokens[index].Value != null)
            {
                return new Token[] { _tokens[index] };
            }

            var nodeMatches = _spanner.Match(_intokens[index], errors);
            var leaf = nodeMatches[0];

            NodeMatch tokenEnd = leaf.Previous;
            NodeMatch tokenStart = tokenEnd.StartDef;

            int length = leaf.Index - tokenStart.Index + 1;
            var token = new Token{
                Definition = tokenEnd.Previous.Node.ParentDefinition,
                StartIndex = tokenStart.Index,
                Length = length,
                Value = _intokens[index].Substring(tokenStart.Index, length),
            };

            _tokens[index] = token;

            return new Token[] { _tokens[index] };
        }
    }
}

