
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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

/*****************************************************************************
 *                                                                           *
 *  SomethingParser.cs                                                       *
 *  10 March 2010                                                            *
 *  Project: ParserBuilder                                                   *
 *  Written by: Richard Sartor                                               *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;


namespace ParserBuilder
{
    public class SomethingParser
    {
        enum grammarState
        {
            end,
            space_0,
            definition,
            space_1,
            start,
        }
        enum spaceState
        {
            start,
            whitespace,
            end,
        }
        enum definitionState
        {
            start,
            name,
            tag,
            space_0,
            equal,
            space_1,
            expr,
            space_2,
            semi,
            end,
        }
        enum orexprState
        {
            start,
            oparen,
            space_0,
            expr,
            pipe,
            space_1,
            cparen,
            modifier,
            tag,
            end,
        }
        enum modifierState
        {
            start,
            obracket,
            symbols,
            space_0,
            end,
            count,
            cbracket,
            space_1,
        }
        enum identifierState
        {
            start,
            letter_,
            letterdigit_,
            end,
        }
        enum tagState
        {
            start,
            colon,
            identifier,
            empty_0,
            empty_1,
            end,
        }
        enum literalState
        {
            start,
            symbols_0,
            symbols_1,
            letterdigitt _,
            rnt,
            symbols_2,
            end,
        }
        enum exprState
        {
            start,
            andexpr,
            orexpr,
            end,
        }
        enum andexprState
        {
            start,
            subexpr,
            space,
            end,
        }
        enum subexprState
        {
            start,
            literal,
            identifier,
            modifier,
            tag,
            end,
        }
        public object GetGrammar(string input, ref int i)
        {
            grammarState state = grammarState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case grammarState.end:
                    break;

                case grammarState.space_0:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        state = grammarState.end;
                        break;
                    }
                    obj = GetDefinition(input, ref i);
                    state = grammarState.definition;
                    break;

                case grammarState.definition:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        state = grammarState.end;
                        break;
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = grammarState.space_0;
                    }
                    else
                    {
                        obj = GetDefinition(input, ref i);
                        state = grammarState.definition;
                    }
                    break;

                case grammarState.space_1:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetDefinition(input, ref i);
                    state = grammarState.definition;
                    break;

                case grammarState.start:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = grammarState.space_1;
                    }
                    else
                    {
                        obj = GetDefinition(input, ref i);
                        state = grammarState.definition;
                    }
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == grammarState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetSpace(string input, ref int i)
        {
            spaceState state = spaceState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case spaceState.start:
                    if (!(ch == 's'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = spaceState.whitespace;
                    break;

                case spaceState.whitespace:
                    if (!(ch == 's'))
                    {
                        state = spaceState.end;
                        break;
                    }
                    state = spaceState.whitespace;
                    break;

                case spaceState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == spaceState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetDefinition(string input, ref int i)
        {
            definitionState state = definitionState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case definitionState.start:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetIdentifier(input, ref i);
                    state = definitionState.name;
                    break;

                case definitionState.name:
                    if (!(ch == ':' || ch == 's' || ch == '='))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == ':')
                    {
                        obj = GetTag(input, ref i);
                        state = definitionState.tag;
                    }
                    else if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = definitionState.space_0;
                    }
                    else
                    {
                        state = definitionState.equal;
                    }
                    break;

                case definitionState.tag:
                    if (!(ch == 's' || ch == '='))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = definitionState.space_0;
                    }
                    else
                    {
                        state = definitionState.equal;
                    }
                    break;

                case definitionState.space_0:
                    if (!(ch == '='))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = definitionState.equal;
                    break;

                case definitionState.equal:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = definitionState.space_1;
                    }
                    else
                    {
                        obj = GetExpr(input, ref i);
                        state = definitionState.expr;
                    }
                    break;

                case definitionState.space_1:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetExpr(input, ref i);
                    state = definitionState.expr;
                    break;

                case definitionState.expr:
                    if (!(ch == 's' || ch == ';'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = definitionState.space_2;
                    }
                    else
                    {
                        state = definitionState.semi;
                    }
                    break;

                case definitionState.space_2:
                    if (!(ch == ';'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = definitionState.semi;
                    break;

                case definitionState.semi:
                    state = definitionState.end;
                    break;

                case definitionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == definitionState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetOrexpr(string input, ref int i)
        {
            orexprState state = orexprState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case orexprState.start:
                    if (!(ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = orexprState.oparen;
                    break;

                case orexprState.oparen:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = orexprState.space_0;
                    }
                    else
                    {
                        obj = GetExpr(input, ref i);
                        state = orexprState.expr;
                    }
                    break;

                case orexprState.space_0:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetExpr(input, ref i);
                    state = orexprState.expr;
                    break;

                case orexprState.expr:
                    if (!(ch == 's' || ch == '|' || ch == ')'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == 's')
                    {
                        obj = GetSpace(input, ref i);
                        state = orexprState.space_1;
                    }
                    else if (ch == '|')
                    {
                        state = orexprState.pipe;
                    }
                    else
                    {
                        state = orexprState.cparen;
                    }
                    break;

                case orexprState.pipe:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '(')
                    {
                        obj = GetExpr(input, ref i);
                        state = orexprState.expr;
                    }
                    else
                    {
                        obj = GetSpace(input, ref i);
                        state = orexprState.space_0;
                    }
                    break;

                case orexprState.space_1:
                    if (!(ch == ')' || ch == '|'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == ')')
                    {
                        state = orexprState.cparen;
                    }
                    else
                    {
                        state = orexprState.pipe;
                    }
                    break;

                case orexprState.cparen:
                    if (!(ch == '[' || ch == '?' || ch == '*' || ch == '+' || ch == ':'))
                    {
                        state = orexprState.end;
                        break;
                    }
                    if (ch == '[' || ch == '?' || ch == '*' || ch == '+')
                    {
                        obj = GetModifier(input, ref i);
                        state = orexprState.modifier;
                    }
                    else
                    {
                        obj = GetTag(input, ref i);
                        state = orexprState.tag;
                    }
                    break;

                case orexprState.modifier:
                    if (!(ch == ':'))
                    {
                        state = orexprState.end;
                        break;
                    }
                    obj = GetTag(input, ref i);
                    state = orexprState.tag;
                    break;

                case orexprState.tag:
                    state = orexprState.end;
                    break;

                case orexprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == orexprState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetModifier(string input, ref int i)
        {
            modifierState state = modifierState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case modifierState.start:
                    if (!(ch == '[' || ch == '?' || ch == '*' || ch == '+'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == '[')
                    {
                        state = modifierState.obracket;
                    }
                    else
                    {
                        state = modifierState.symbols;
                    }
                    break;

                case modifierState.obracket:
                    if (!(ch == 's'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetSpace(input, ref i);
                    state = modifierState.space_0;
                    break;

                case modifierState.symbols:
                    state = modifierState.end;
                    break;

                case modifierState.space_0:
                    if (!char.IsDigit(ch))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = modifierState.count;
                    break;

                case modifierState.end:
                    break;

                case modifierState.count:
                    if (!(ch == 's'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetSpace(input, ref i);
                    state = modifierState.space_1;
                    break;

                case modifierState.cbracket:
                    state = modifierState.end;
                    break;

                case modifierState.space_1:
                    if (!(ch == ']'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = modifierState.cbracket;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == modifierState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetIdentifier(string input, ref int i)
        {
            identifierState state = identifierState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case identifierState.start:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = identifierState.letter_;
                    break;

                case identifierState.letter_:
                    if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = identifierState.letterdigit_;
                    break;

                case identifierState.letterdigit_:
                    if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                    {
                        state = identifierState.end;
                        break;
                    }
                    state = identifierState.letterdigit_;
                    break;

                case identifierState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == identifierState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetTag(string input, ref int i)
        {
            tagState state = tagState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case tagState.start:
                    if (!(ch == ':'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = tagState.colon;
                    break;

                case tagState.colon:
                    if (!(char.IsLetter(ch) || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetIdentifier(input, ref i);
                    state = tagState.identifier;
                    break;

                case tagState.identifier:
                    if (!(ch == '['))
                    {
                        state = tagState.end;
                        break;
                    }
                    state = tagState.empty_0;
                    break;

                case tagState.empty_0:
                    if (!(ch == ']'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = tagState.empty_1;
                    break;

                case tagState.empty_1:
                    state = tagState.end;
                    break;

                case tagState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == tagState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetLiteral(string input, ref int i)
        {
            literalState state = literalState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case literalState.start:
                    if (!(ch == '\''))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = literalState.symbols_0;
                    break;

                case literalState.symbols_0:
                    if (!(char.IsLetterOrDigit(ch) || ch == '\\' || ch == '\t' || ch == ' ' || ch == '`' || ch == '~' || ch == '!' || ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '^' || ch == '&' || ch == '*' || ch == '(' || ch == ')' || ch == '-' || ch == '_' || ch == '=' || ch == '+' || ch == '|' || ch == '[' || ch == ']' || ch == '{' || ch == '}' || ch == ';' || ch == ':' || ch == '"' || ch == ',' || ch == '<' || ch == '.' || ch == '>' || ch == '/' || ch == '?'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == '\\')
                    {
                        state = literalState.symbols_1;
                    }
                    else
                    {
                        state = literalState.letterdigitt _;
                    }
                    break;

                case literalState.symbols_1:
                    if (!(ch == '\\' || ch == '\'' || ch == 'r' || ch == 'n' || ch == 't'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    state = literalState.rnt;
                    break;

                case literalState.letterdigitt _:
                    if (!(char.IsLetterOrDigit(ch) || ch == '\\' || ch == '\t' || ch == ' ' || ch == '`' || ch == '~' || ch == '!' || ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '^' || ch == '&' || ch == '*' || ch == '(' || ch == ')' || ch == '-' || ch == '_' || ch == '=' || ch == '+' || ch == '|' || ch == '[' || ch == ']' || ch == '{' || ch == '}' || ch == ';' || ch == ':' || ch == '"' || ch == ',' || ch == '<' || ch == '.' || ch == '>' || ch == '/' || ch == '?' || ch == '\''))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == '\\')
                    {
                        state = literalState.symbols_1;
                    }
                    else if (char.IsLetterOrDigit(ch) || ch == '\t' || ch == ' ' || ch == '`' || ch == '~' || ch == '!' || ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '^' || ch == '&' || ch == '*' || ch == '(' || ch == ')' || ch == '-' || ch == '_' || ch == '=' || ch == '+' || ch == '\\' || ch == '|' || ch == '[' || ch == ']' || ch == '{' || ch == '}' || ch == ';' || ch == ':' || ch == '"' || ch == ',' || ch == '<' || ch == '.' || ch == '>' || ch == '/' || ch == '?')
                    {
                        state = literalState.letterdigitt _;
                    }
                    else
                    {
                        state = literalState.symbols_2;
                    }
                    break;

                case literalState.rnt:
                    if (!(char.IsLetterOrDigit(ch) || ch == '\'' || ch == '\\' || ch == '\t' || ch == ' ' || ch == '`' || ch == '~' || ch == '!' || ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '^' || ch == '&' || ch == '*' || ch == '(' || ch == ')' || ch == '-' || ch == '_' || ch == '=' || ch == '+' || ch == '|' || ch == '[' || ch == ']' || ch == '{' || ch == '}' || ch == ';' || ch == ':' || ch == '"' || ch == ',' || ch == '<' || ch == '.' || ch == '>' || ch == '/' || ch == '?'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == '\'')
                    {
                        state = literalState.symbols_2;
                    }
                    else if (ch == '\\')
                    {
                        state = literalState.symbols_1;
                    }
                    else
                    {
                        state = literalState.letterdigitt _;
                    }
                    break;

                case literalState.symbols_2:
                    state = literalState.end;
                    break;

                case literalState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == literalState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetExpr(string input, ref int i)
        {
            exprState state = exprState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case exprState.start:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_' || ch == '('))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (char.IsLetter(ch) || ch == '\'' || ch == '_')
                    {
                        obj = GetAndexpr(input, ref i);
                        state = exprState.andexpr;
                    }
                    else
                    {
                        obj = GetOrexpr(input, ref i);
                        state = exprState.orexpr;
                    }
                    break;

                case exprState.andexpr:
                    state = exprState.end;
                    break;

                case exprState.orexpr:
                    state = exprState.end;
                    break;

                case exprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == exprState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetAndexpr(string input, ref int i)
        {
            andexprState state = andexprState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case andexprState.start:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetSubexpr(input, ref i);
                    state = andexprState.subexpr;
                    break;

                case andexprState.subexpr:
                    if (!(ch == 's'))
                    {
                        state = andexprState.end;
                        break;
                    }
                    obj = GetSpace(input, ref i);
                    state = andexprState.space;
                    break;

                case andexprState.space:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    obj = GetSubexpr(input, ref i);
                    state = andexprState.subexpr;
                    break;

                case andexprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == andexprState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetSubexpr(string input, ref int i)
        {
            subexprState state = subexprState.start;
            char ch;
            object obj;

            for (; i < input.Length; i++)
            {
                ch = input[i];
                switch (state)
                {
                case subexprState.start:
                    if (!(char.IsLetter(ch) || ch == '\'' || ch == '_'))
                    {
                        throw new System.InvalidOperationException("Invalid character");
                    }
                    if (ch == '\'')
                    {
                        obj = GetLiteral(input, ref i);
                        state = subexprState.literal;
                    }
                    else
                    {
                        obj = GetIdentifier(input, ref i);
                        state = subexprState.identifier;
                    }
                    break;

                case subexprState.literal:
                    if (!(ch == '[' || ch == '?' || ch == '*' || ch == '+' || ch == ':'))
                    {
                        state = subexprState.end;
                        break;
                    }
                    if (ch == '[' || ch == '?' || ch == '*' || ch == '+')
                    {
                        obj = GetModifier(input, ref i);
                        state = subexprState.modifier;
                    }
                    else
                    {
                        obj = GetTag(input, ref i);
                        state = subexprState.tag;
                    }
                    break;

                case subexprState.identifier:
                    if (!(ch == '[' || ch == '?' || ch == '*' || ch == '+' || ch == ':'))
                    {
                        state = subexprState.end;
                        break;
                    }
                    if (ch == '[' || ch == '?' || ch == '*' || ch == '+')
                    {
                        obj = GetModifier(input, ref i);
                        state = subexprState.modifier;
                    }
                    else
                    {
                        obj = GetTag(input, ref i);
                        state = subexprState.tag;
                    }
                    break;

                case subexprState.modifier:
                    if (!(ch == ':'))
                    {
                        state = subexprState.end;
                        break;
                    }
                    obj = GetTag(input, ref i);
                    state = subexprState.tag;
                    break;

                case subexprState.tag:
                    state = subexprState.end;
                    break;

                case subexprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
                }
                if (state == subexprState.end)
                {
                    break;
                }
            }
            i--;
            return null;
        }

        public object GetGrammar(string input)
        {
            int i = 0;

            return GetGrammar(input, ref i);
        }
    }
}


