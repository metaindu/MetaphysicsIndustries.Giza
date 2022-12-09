
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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

namespace MetaphysicsIndustries.Giza
{
    public class ListTokenSource : IInputSource<Token>
    {
        public ListTokenSource(NGrammar grammar, params string[] intokens)
            : this(grammar, (IEnumerable<string>)intokens)
        {
        }
        public ListTokenSource(NGrammar grammar, IEnumerable<string> intokens)
        {
            _grammar = grammar.Clone();
            _tokenDef = new NDefinition("$token");
            _grammar.Definitions.Add(_tokenDef);

            foreach (var def in grammar.Definitions)
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

        NGrammar _grammar;
        Spanner _spanner;
        NDefinition _tokenDef;
        List<string> _intokens;
        List<Token> _tokens = new List<Token>();

        public InputElementSet<Token> GetInputAtLocation(int index)
        {
            var tinfo = new InputElementSet<Token>();

            if (index >= _intokens.Count)
            {
                tinfo.EndOfInput = true;
                tinfo.EndOfInputPosition = new InputPosition(_intokens.Count);
                tinfo.InputElements = new Token[0];
                return tinfo;
            }

            tinfo.EndOfInput = false;
            tinfo.EndOfInputPosition = new InputPosition(-1);

            if (_tokens[index].Value != null)
            {
                tinfo.InputElements = new Token[] { _tokens[index] };
                return tinfo;
            }

            var nodeMatches = _spanner.Match(_intokens[index].ToCharacterSource(), tinfo.Errors);
            var leaf = nodeMatches[0];

            var tokenEnd = leaf.Previous;
            var tokenStart = tokenEnd.StartDef;

            int length = leaf.StartPosition.Index - tokenStart.StartPosition.Index + 1;
            var token = new Token(
                definition: tokenEnd.Previous.Node.ParentDefinition,
                startPosition: tokenStart.StartPosition,
                value: _intokens[index].Substring(tokenStart.StartPosition.Index, length)
            );

            _tokens[index] = token;

            tinfo.InputElements = new Token[] { _tokens[index] };
            return tinfo;
        }

        public InputPosition CurrentPosition
        {
            get { return new InputPosition(_tokens.Count - 1); }
        }

        public InputPosition GetPosition(int index)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentIndex(int index)
        {
            throw new NotImplementedException();
        }

        public InputElementSet<Token> Peek()
        {
            throw new NotImplementedException();
        }

        public InputElementSet<Token> GetNextValue()
        {
            throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
        }
    }
}

