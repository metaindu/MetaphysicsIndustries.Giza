
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

using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class Tokenizer : IInputSource<Token>
    {
        public Tokenizer(Grammar grammar, IInputSource<InputChar> input)
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
        IInputSource<InputChar> _input;
        readonly Dictionary<int, InputElementSet<Token>> _tokenizationsByIndex = new Dictionary<int, InputElementSet<Token>>();
        readonly HashSet<int> _startIndexes = new HashSet<int>();

        struct TokenizationByIndex
        {
            public TokenizationByIndex(int index,
                                       ICollection<Error> errors,
                                       bool endOfInput,
                                       ICollection<NodeMatch<InputChar>> matchTreeLeaves,
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
            public ICollection<NodeMatch<InputChar>> MatchTreeLeaves;
            public InputPosition LastPosition;
        }

        public InputElementSet<Token> GetInputAtLocation(int index)
        {
            Logger.WriteLine("Tokenizer: Getting tokens at index {0}, current input position is {1}", index, _input.CurrentPosition.Index);

            if (_tokenizationsByIndex.ContainsKey(index))
            {
                return _tokenizationsByIndex[index];
            }

            var tinfo = new InputElementSet<Token>();
            _tokenizationsByIndex[index] = tinfo;

            var tokenizations = new Queue<TokenizationByIndex>();
            var startIndexes = new Queue<int>();
            startIndexes.Enqueue(index);

            while (startIndexes.Count > 0)
            {
                int startIndex = startIndexes.Dequeue();

                bool endOfInput2;
                var errors2 = new List<Error>();
                var tokenLeaves = new HashSet<NodeMatch<InputChar>>();
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

            var tokens = new HashSet<Token>();
            tinfo.EndOfInput = false;
            tinfo.EndOfInputPosition = new InputPosition(-1);

            if (hasLeaves.Count > 0)
            {
                var matchTreeLeaves = new HashSet<NodeMatch<InputChar>>();

                foreach (var tok in hasLeaves)
                {
                    matchTreeLeaves.UnionWith(tok.MatchTreeLeaves);
                }

                foreach (var leaf in matchTreeLeaves)
                {
                    var tokenEnd = leaf.Previous;
                    var tokenStart = tokenEnd.StartDef;

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
                tinfo.InputElements = tokens.ToArray();
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
                tinfo.InputElements = new Token[0];
                return tinfo;
            }
        }

        public InputPosition CurrentPosition
        {
            get { return _input.CurrentPosition; }
        }

        public InputPosition GetPosition(int index)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentIndex(int index)
        {
            _input.SetCurrentIndex(index);
        }

        public InputElementSet<Token> Peek()
        {
            return GetInputAtLocation(CurrentPosition.Index);
        }

        public InputElementSet<Token> GetNextValue()
        {
            var ies = Peek();
            if (!ies.EndOfInput &&
                !ies.Errors.Any())
            {
                _startIndexes.UnionWith(ies.InputElements.Select(x => x.IndexOfNextTokenization));
                var nextIndex = _startIndexes.Min();
                _startIndexes.Remove(nextIndex);
                SetCurrentIndex(nextIndex);
            }
            return ies;
        }

        public int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAtEnd
        {
            get
            {
                return _input.IsAtEnd;
            }
        }

        string CollectValue(NodeMatch<InputChar> tokenEnd)
        {
            if (tokenEnd == null) throw new ArgumentNullException("tokenEnd");
            if (tokenEnd.Transition != TransitionType.EndDef) throw new ArgumentException("tokenDef must be an EndDef");
            if (tokenEnd.StartDef == null) throw new ArgumentException("tokenDef must have a corresponding StartDef");

            var chs = new List<char>();
            var cur = tokenEnd;

            while (cur != tokenEnd.StartDef)
            {
                if (cur.InputElement.Value != '\0') chs.Add(cur.InputElement.Value);

                cur = cur.Previous;
            }

            chs.Reverse();

            return new string(chs.ToArray());
        }
    }
}

