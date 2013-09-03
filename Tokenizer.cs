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

            _spanner = new Spanner(_tokenDef);
        }

        Grammar _grammar;
        Spanner _spanner;
        Definition _tokenDef;

        struct TokenizationByIndex
        {
            public TokenizationByIndex(int index,
                                       ICollection<Error> errors,
                                       bool endOfInput,
                                       ICollection<NodeMatch> matchTreeLeaves,
                                       int lastIndex)
            {
                Index = index;
                Errors = errors;
                EndOfInput = endOfInput;
                MatchTreeLeaves = matchTreeLeaves;
                LastIndex = lastIndex;
            }

            public int Index;
            public ICollection<Error> Errors;
            public bool EndOfInput;
            public ICollection<NodeMatch> MatchTreeLeaves;
            public int LastIndex;
        }

        public Token[] GetTokensAtLocation(string input, int index, 
                                           List<Error> errors, 
                                           out bool endOfInput, 
                                           out int endOfInputIndex)
        {
            var tokenizations = new Queue<TokenizationByIndex>();
            var startIndexes = new Queue<int>();
            startIndexes.Enqueue(index);

            while (startIndexes.Count > 0)
            {
                int startIndex = startIndexes.Dequeue();

                bool endOfInput2;
                var errors2 = new List<Error>();
                var tokenLeaves = new Set<NodeMatch>();
                int endOfInputIndex2;

                var leaves = _spanner.Match(input, errors2,
                                            out endOfInput2,
                                            out endOfInputIndex2,
                                            mustUseAllInput:false,
                                            startIndex:startIndex);

                if (!errors2.ContainsNonWarnings())
                {
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
                            tokenLeaves.Add(leaf);
                            endOfInput = false;
                        }
                    }
                }

                if (tokenLeaves.Count > 0 ||
                    endOfInput2 ||
                    errors.Count > 0)
                {
                    var tokenization = new TokenizationByIndex(
                        startIndex, errors2, endOfInput2, tokenLeaves,
                        endOfInputIndex2);

                    tokenizations.Enqueue(tokenization);
                }
            }

            // if any has leaves, then all available leaves will be returned,
            // no errors will be returned, and endOfInput will be false
            //
            // otherwise, if any has EndOfInput=true, then no leaves will be
            // returned, no errors will be returned, and endOfInput will be
            // true
            //
            // otherwise, no leaves will be returned, the errors of the first
            // tokenization (by StartIndex) will be returned, and endOfInput
            // will be false

            var hasLeaves = new List<TokenizationByIndex>();
            var hasEnd = new List<TokenizationByIndex>();
            var hasErrors = new List<TokenizationByIndex>();

            foreach (var tok in tokenizations)
            {
                if (tok.MatchTreeLeaves.Count > 0 &&
                    !tok.Errors.ContainsNonWarnings() &&
                    !tok.EndOfInput)
                {
                    hasLeaves.Add(tok);
                }
                else if (tok.EndOfInput &&
                         !tok.Errors.ContainsNonWarnings())
                {
                    hasEnd.Add(tok);
                }
                else if (tok.Errors.ContainsNonWarnings())
                {
                    hasErrors.Add(tok);
                }
                else
                {
                    // no tokens, no end, no errors?
                    throw new InvalidOperationException();
                }
            }

            if (hasLeaves.Count > 0)
            {
                var matchTreeLeaves = new Set<NodeMatch>();

                foreach (var tok in hasLeaves)
                {
                    matchTreeLeaves.AddRange(tok.MatchTreeLeaves);
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

                endOfInput = false;
                endOfInputIndex = -1;
                return tokens.ToArray();
            }
            else if (hasEnd.Count > 0)
            {
                endOfInput = true;
                endOfInputIndex = index;
                foreach (var tok in hasEnd)
                {
                    endOfInputIndex = Math.Max(endOfInputIndex, tok.LastIndex);
                }
                return new Token[0];
            }
            else
            {
                var minIndex = hasErrors[0].Index;
                var mintok = hasErrors[0];

                foreach (var tok in tokenizations)
                {
                    if (tok.Index < minIndex)
                    {
                        minIndex = tok.Index;
                        mintok = tok;
                    }
                }

                errors.AddRange(mintok.Errors);

                endOfInput = false;
                endOfInputIndex = -1;
                return new Token[0];
            }



        }
    }
}

