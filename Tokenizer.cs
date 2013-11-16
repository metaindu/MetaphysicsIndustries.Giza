using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class Tokenizer : ITokenSource
    {
        public Tokenizer(Grammar grammar, CharacterSource input)
        {
            if (grammar == null) throw new ArgumentNullException("grammar");
            if (input == null) throw new ArgumentNullException("input");

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
            _input = input;
        }

        Grammar _grammar;
        Spanner _spanner;
        Definition _tokenDef;
        CharacterSource _input;

        struct TokenizationByIndex
        {
            public TokenizationByIndex(int index,
                                       ICollection<Error> errors,
                                       bool endOfInput,
                                       ICollection<NodeMatch> matchTreeLeaves,
                                       InputPosition lastPosition)
            {
                Index = index;
                Errors = errors;
                EndOfInput = endOfInput;
                MatchTreeLeaves = matchTreeLeaves;
                LastPosition = lastPosition;
            }

            public int Index;
            public ICollection<Error> Errors;
            public bool EndOfInput;
            public ICollection<NodeMatch> MatchTreeLeaves;
            public InputPosition LastPosition;
        }

        public TokenizationInfo GetTokensAtLocation(int index)
        {
            Logger.WriteLine("Tokenizer: Getting tokens at index {0}, current input position is {1}", index, _input.CurrentPosition.Index);

            TokenizationInfo tinfo = new TokenizationInfo();

            var tokenizations = new Queue<TokenizationByIndex>();
            var startIndexes = new Queue<int>();
            startIndexes.Enqueue(index);

            while (startIndexes.Count > 0)
            {
                int startIndex = startIndexes.Dequeue();

                bool endOfInput2;
                var errors2 = new List<Error>();
                var tokenLeaves = new Set<NodeMatch>();
                InputPosition endOfInputPosition2;

                var leaves = _spanner.Match(_input, errors2,
                                            out endOfInput2,
                                            out endOfInputPosition2,
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
                            var stindex = tokenStart.StartPosition.Index;
                            var length = leaf.StartPosition.Index - tokenStart.StartPosition.Index + 1;

                            //skip the comment and start again
                            startIndexes.Enqueue(stindex + length);
                        }
                        else
                        {
                            tokenLeaves.Add(leaf);
                            endOfInput2 = false;
                        }
                    }
                }

                if (tokenLeaves.Count > 0 ||
                    endOfInput2 ||
                    errors2.Count > 0)
                {
                    var tokenization = new TokenizationByIndex(
                        startIndex, errors2, endOfInput2, tokenLeaves,
                        endOfInputPosition2);

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

            var tokens = new Set<Token>();
            tinfo.EndOfInput = false;
            tinfo.EndOfInputPosition = new InputPosition(-1);

            if (hasLeaves.Count > 0)
            {
                var matchTreeLeaves = new Set<NodeMatch>();

                foreach (var tok in hasLeaves)
                {
                    matchTreeLeaves.AddRange(tok.MatchTreeLeaves);
                }

                foreach (NodeMatch leaf in matchTreeLeaves)
                {
                    NodeMatch tokenEnd = leaf.Previous;
                    NodeMatch tokenStart = tokenEnd.StartDef;

                    tokens.Add(new Token(
                        definition: tokenEnd.Previous.Node.ParentDefinition,
                        startPosition: tokenStart.StartPosition,
                        value: CollectValue(tokenEnd)
                    ));
                }
            }

            if (hasEnd.Count > 0)
            {
                tinfo.EndOfInput = true;
                tinfo.EndOfInputPosition = hasEnd[0].LastPosition;
                foreach (var tok in hasEnd)
                {
                    if (tok.LastPosition.Index > tinfo.EndOfInputPosition.Index)
                    {
                        tinfo.EndOfInputPosition = tok.LastPosition;
                    }
                }
            }

            if (hasLeaves.Count > 0 || hasEnd.Count > 0)
            {
                tinfo.Tokens = tokens.ToArray();
                return tinfo;
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

                tinfo.Errors.AddRange(mintok.Errors);
                tinfo.Tokens = new Token[0];
                return tinfo;
            }
        }

        string CollectValue(NodeMatch tokenEnd)
        {
            if (tokenEnd == null) throw new ArgumentNullException("tokenEnd");
            if (tokenEnd.Transition != NodeMatch.TransitionType.EndDef) throw new ArgumentException("tokenDef must be an EndDef");
            if (tokenEnd.StartDef == null) throw new ArgumentException("tokenDef must have a corresponding StartDef");

            var chs = new List<char>();
            var cur = tokenEnd;

            while (cur != tokenEnd.StartDef)
            {
                if (cur.MatchedChar.Value != '\0') chs.Add(cur.MatchedChar.Value);

                cur = cur.Previous;
            }

            chs.Reverse();

            return new string(chs.ToArray());
        }
    }
}

