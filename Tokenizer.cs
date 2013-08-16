using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class Tokenizer
    {
        public Tokenizer(Grammar grammar)
        {
            _grammar = grammar.Clone();
            _tokenDef = new Definition("$token");
            _grammar.Definitions.Add(_tokenDef);

            foreach (Definition def in grammar.Definitions)
            {
                if (def.Directives.Contains(DefinitionDirective.Token) ||
                    def.Directives.Contains(DefinitionDirective.Comment))
                {
                    DefRefNode node = new DefRefNode(def);
                    _tokenDef.Nodes.Add(node);
                    _tokenDef.StartNodes.Add(node);
                    _tokenDef.EndNodes.Add(node);
                }
            }
        }

        Grammar _grammar;
        Spanner _spanner = new Spanner();
        Definition _tokenDef;

        public Token[] GetTokensAtLocation(string input, int index, List<Error> errors)
        {
            Set<NodeMatch> matchTreeLeaves = new Set<NodeMatch>();

            Queue<int> startIndexes = new Queue<int>();
            startIndexes.Enqueue(index);
            while (startIndexes.Count > 0)
            {
                int startIndex = startIndexes.Dequeue();
                var leaves = _spanner.Match(_tokenDef, input, errors,
                                            mustUseAllInput:false,
                                            startIndex:startIndex);
                if (errors.Count > 0)
                {
                    return null;
                }

                foreach (var leaf in leaves)
                {
                    var tokenEnd = leaf.Previous;
                    if (tokenEnd.DefRef.Directives.Contains(DefinitionDirective.Comment))
                    {
                        var tokenStart = tokenEnd.StartDef;
                        var stindex = tokenStart.Index;
                        var length = leaf.Index - tokenStart.Index + 1;

                        //skip the comment and start again
                        startIndexes.Enqueue(stindex + length);
                    }
                    else
                    {
                        matchTreeLeaves.Add(leaf);
                    }
                }
            }

            List<Token> tokens = new List<Token>();
            foreach (NodeMatch leaf in matchTreeLeaves)
            {
                NodeMatch tokenEnd = leaf.Previous;
                NodeMatch tokenStart = tokenEnd.StartDef;

                tokens.Add(new Token{
                    Definition = tokenEnd.Previous.Node.ParentDefinition,
                    StartIndex = tokenStart.Index,
                    Length = leaf.Index - tokenStart.Index + 1
                });
            }

            return tokens.ToArray();
        }
    }
}

