
/*****************************************************************************
 *                                                                           *
 *  SupergrammarSpanner.cs                                                   *
 *  6 July 2010                                                              *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2010 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class SupergrammarSpanner : MetaphysicsIndustries.Giza.BaseSpanner
    {
        #region states
        enum grammarState
        {
            start,
            definition,
            comment,
            end,
        }
        enum definitionState
        {
            start,
            identifier,
            defmod,
            equal,
            expr,
            semi,
            end,
        }
        enum defmodState
        {
            start,
            less,
            defmod_item_0,
            comma,
            greater,
            defmod_item_1,
            end,
        }
        enum defmoditemState
        {
            start,
            id_whitespace,
            id_ignore,
            end,
            hyphen,
            id_case,
        }
        enum idwhitespaceState
        {
            start,
            whitespace_0,
            whitespace_1,
            whitespace_2,
            whitespace_3,
            whitespace_4,
            whitespace_5,
            whitespace_6,
            whitespace_7,
            whitespace_8,
            whitespace_9,
            end,
        }
        enum idignoreState
        {
            start,
            ignore_0,
            ignore_1,
            ignore_2,
            ignore_3,
            ignore_4,
            ignore_5,
            end,
        }
        enum idcaseState
        {
            start,
            case_0,
            case_1,
            case_2,
            case_3,
            end,
        }
        enum exprState
        {
            start,
            subexpr,
            orexpr,
            comment,
            end,
        }
        enum orexprState
        {
            start,
            oparen,
            expr_0,
            pipe,
            cparen,
            expr_1,
            modifier,
            end,
        }
        enum subexprState
        {
            start,
            identifier,
            literal,
            charclass,
            modifier,
            colon,
            end,
            tag,
        }
        enum modifierState
        {
            start,
            star_plus_question,
            end,
        }
        enum numberState
        {
            start,
            class_digit,
            end,
        }
        enum identifierState
        {
            start,
            w__,
            w__d,
            end,
        }
        enum literalState
        {
            start,
            quote_0,
            litchar,
            quote_1,
            end,
        }
        enum litcharState
        {
            start,
            symbols,
            bslash,
            unicodechar,
            end,
            wldsrnt,
        }
        enum charclassState
        {
            start,
            obracket,
            chevron,
            charclasschar,
            cbracket,
            end,
        }
        enum charclasscharState
        {
            start,
            symbols,
            bslash,
            unicodechar,
            end,
            wldsrnt,
        }
        enum unicodecharState
        {
            start,
            bslash_letter_x_0,
            bslash_letter_x_1,
            dabcdef_0,
            dabcdef_1,
            dabcdef_2,
            dabcdef_3,
            end,
        }
        enum commentState
        {
            start,
            slash_star_0,
            slash_star_1,
            slash_slash_0,
            slash_slash_1,
            chevron_star,
            star_0,
            star_1,
            rn_0,
            rn_1,
            chevron_star_slash,
            slash,
            end,
        }
        public MetaphysicsIndustries.Giza.Span Getgrammar(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.Span span = Getgrammar(input, ref i);

            if (span != null)
            {
                span.Tag = "grammar";
            }
            return span;
        }
        #endregion

        public MetaphysicsIndustries.Giza.Span Getnumber(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.Span span = Getnumber(input, ref i);

            if (span != null)
            {
                span.Tag = "number";
            }
            return span;
        }

        public MetaphysicsIndustries.Giza.Span Getgrammar(string input, ref int i)
        {
            return GetItem(input, ref i, true, "grammar", (int)(grammarState.start), (int)(grammarState.end), this.GetValidNextStates_grammar, this.GetStateTag_grammar, this.GetSubSpan_grammar);
        }

        public Int32[] GetValidNextStates_grammar(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((grammarState)(currentState))
            {
                case grammarState.start:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '<')
                    {
                        validNextStates.Add((int)(grammarState.definition));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(grammarState.comment));
                    }
                    break;

                case grammarState.definition:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '<')
                    {
                        validNextStates.Add((int)(grammarState.definition));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(grammarState.comment));
                    }
                    linksToEnd = true;
                    break;

                case grammarState.comment:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '<')
                    {
                        validNextStates.Add((int)(grammarState.definition));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(grammarState.comment));
                    }
                    linksToEnd = true;
                    break;

                case grammarState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_grammar(string input, ref int i, int nextState)
        {
            switch ((grammarState)(nextState))
            {
                case grammarState.definition:
                    return Getdefinition(input, ref i);

                case grammarState.comment:
                    return Getcomment(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_grammar(int state)
        {
            switch ((grammarState)(state))
            {
                case grammarState.start:
                    return "start";

                case grammarState.definition:
                    return "definition";

                case grammarState.comment:
                    return "comment";

                case grammarState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getdefinition(string input, ref int i)
        {
            return GetItem(input, ref i, true, "definition", (int)(definitionState.start), (int)(definitionState.end), this.GetValidNextStates_definition, this.GetStateTag_definition, this.GetSubSpan_definition);
        }

        public Int32[] GetValidNextStates_definition(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((definitionState)(currentState))
            {
                case definitionState.start:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(definitionState.identifier));
                    }
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(definitionState.defmod));
                    }
                    break;

                case definitionState.identifier:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(definitionState.equal));
                    }
                    break;

                case definitionState.defmod:
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(definitionState.defmod));
                    }
                    if (char.IsLetter(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(definitionState.identifier));
                    }
                    break;

                case definitionState.equal:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[' || ch == '(' || ch == '/')
                    {
                        validNextStates.Add((int)(definitionState.expr));
                    }
                    break;

                case definitionState.expr:
                    if (ch == ';')
                    {
                        validNextStates.Add((int)(definitionState.semi));
                    }
                    break;

                case definitionState.semi:
                    linksToEnd = true;
                    break;

                case definitionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_definition(string input, ref int i, int nextState)
        {
            switch ((definitionState)(nextState))
            {
                case definitionState.identifier:
                    return Getidentifier(input, ref i);

                case definitionState.defmod:
                    return Getdefmod(input, ref i);

                case definitionState.expr:
                    return Getexpr(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_definition(int state)
        {
            switch ((definitionState)(state))
            {
                case definitionState.start:
                    return "start";

                case definitionState.identifier:
                    return "identifier";

                case definitionState.defmod:
                    return "defmod";

                case definitionState.equal:
                    return "=";

                case definitionState.expr:
                    return "expr";

                case definitionState.semi:
                    return ";";

                case definitionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getdefmod(string input, ref int i)
        {
            return GetItem(input, ref i, true, "defmod", (int)(defmodState.start), (int)(defmodState.end), this.GetValidNextStates_defmod, this.GetStateTag_defmod, this.GetSubSpan_defmod);
        }

        public Int32[] GetValidNextStates_defmod(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((defmodState)(currentState))
            {
                case defmodState.start:
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(defmodState.less));
                    }
                    break;

                case defmodState.less:
                    if (ch == 'w' || ch == 'W' || ch == 'i' || ch == 'I')
                    {
                        validNextStates.Add((int)(defmodState.defmod_item_0));
                    }
                    break;

                case defmodState.defmod_item_0:
                    if (ch == ',')
                    {
                        validNextStates.Add((int)(defmodState.comma));
                    }
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(defmodState.greater));
                    }
                    break;

                case defmodState.comma:
                    if (ch == 'w' || ch == 'W' || ch == 'i' || ch == 'I')
                    {
                        validNextStates.Add((int)(defmodState.defmod_item_1));
                    }
                    break;

                case defmodState.greater:
                    linksToEnd = true;
                    break;

                case defmodState.defmod_item_1:
                    if (ch == ',')
                    {
                        validNextStates.Add((int)(defmodState.comma));
                    }
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(defmodState.greater));
                    }
                    break;

                case defmodState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_defmod(string input, ref int i, int nextState)
        {
            switch ((defmodState)(nextState))
            {
                case defmodState.defmod_item_0:
                    return Getdefmod_item(input, ref i);

                case defmodState.defmod_item_1:
                    return Getdefmod_item(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_defmod(int state)
        {
            switch ((defmodState)(state))
            {
                case defmodState.start:
                    return "start";

                case defmodState.less:
                    return "<";

                case defmodState.defmod_item_0:
                    return "defmod-item";

                case defmodState.comma:
                    return ",";

                case defmodState.greater:
                    return ">";

                case defmodState.defmod_item_1:
                    return "defmod-item";

                case defmodState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getdefmod_item(string input, ref int i)
        {
            return GetItem(input, ref i, true, "defmod-item", (int)(defmoditemState.start), (int)(defmoditemState.end), this.GetValidNextStates_defmod_item, this.GetStateTag_defmod_item, this.GetSubSpan_defmod_item);
        }

        public Int32[] GetValidNextStates_defmod_item(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((defmoditemState)(currentState))
            {
                case defmoditemState.start:
                    if (ch == 'w' || ch == 'W')
                    {
                        validNextStates.Add((int)(defmoditemState.id_whitespace));
                    }
                    if (ch == 'i' || ch == 'I')
                    {
                        validNextStates.Add((int)(defmoditemState.id_ignore));
                    }
                    break;

                case defmoditemState.id_whitespace:
                    linksToEnd = true;
                    break;

                case defmoditemState.id_ignore:
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(defmoditemState.hyphen));
                    }
                    if (ch == 'c' || ch == 'C')
                    {
                        validNextStates.Add((int)(defmoditemState.id_case));
                    }
                    break;

                case defmoditemState.end:
                    break;

                case defmoditemState.hyphen:
                    if (ch == 'c' || ch == 'C')
                    {
                        validNextStates.Add((int)(defmoditemState.id_case));
                    }
                    break;

                case defmoditemState.id_case:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_defmod_item(string input, ref int i, int nextState)
        {
            switch ((defmoditemState)(nextState))
            {
                case defmoditemState.id_whitespace:
                    return Getid_whitespace(input, ref i);

                case defmoditemState.id_ignore:
                    return Getid_ignore(input, ref i);

                case defmoditemState.id_case:
                    return Getid_case(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_defmod_item(int state)
        {
            switch ((defmoditemState)(state))
            {
                case defmoditemState.start:
                    return "start";

                case defmoditemState.id_whitespace:
                    return "id-whitespace";

                case defmoditemState.id_ignore:
                    return "id-ignore";

                case defmoditemState.end:
                    return "end";

                case defmoditemState.hyphen:
                    return "-";

                case defmoditemState.id_case:
                    return "id-case";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getid_whitespace(string input, ref int i)
        {
            return GetItem(input, ref i, false, "id-whitespace", (int)(idwhitespaceState.start), (int)(idwhitespaceState.end), this.GetValidNextStates_id_whitespace, this.GetStateTag_id_whitespace, this.GetSubSpan_id_whitespace);
        }

        public Int32[] GetValidNextStates_id_whitespace(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((idwhitespaceState)(currentState))
            {
                case idwhitespaceState.start:
                    if (ch == 'w' || ch == 'W')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_0));
                    }
                    break;

                case idwhitespaceState.whitespace_0:
                    if (ch == 'h' || ch == 'H')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_1));
                    }
                    break;

                case idwhitespaceState.whitespace_1:
                    if (ch == 'i' || ch == 'I')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_2));
                    }
                    break;

                case idwhitespaceState.whitespace_2:
                    if (ch == 't' || ch == 'T')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_3));
                    }
                    break;

                case idwhitespaceState.whitespace_3:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_4));
                    }
                    break;

                case idwhitespaceState.whitespace_4:
                    if (ch == 's' || ch == 'S')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_5));
                    }
                    break;

                case idwhitespaceState.whitespace_5:
                    if (ch == 'p' || ch == 'P')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_6));
                    }
                    break;

                case idwhitespaceState.whitespace_6:
                    if (ch == 'a' || ch == 'A')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_7));
                    }
                    break;

                case idwhitespaceState.whitespace_7:
                    if (ch == 'c' || ch == 'C')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_8));
                    }
                    break;

                case idwhitespaceState.whitespace_8:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(idwhitespaceState.whitespace_9));
                    }
                    break;

                case idwhitespaceState.whitespace_9:
                    linksToEnd = true;
                    break;

                case idwhitespaceState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_id_whitespace(string input, ref int i, int nextState)
        {
            switch ((idwhitespaceState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_id_whitespace(int state)
        {
            switch ((idwhitespaceState)(state))
            {
                case idwhitespaceState.start:
                    return "start";

                case idwhitespaceState.whitespace_0:
                    return "whitespace";

                case idwhitespaceState.whitespace_1:
                    return "whitespace";

                case idwhitespaceState.whitespace_2:
                    return "whitespace";

                case idwhitespaceState.whitespace_3:
                    return "whitespace";

                case idwhitespaceState.whitespace_4:
                    return "whitespace";

                case idwhitespaceState.whitespace_5:
                    return "whitespace";

                case idwhitespaceState.whitespace_6:
                    return "whitespace";

                case idwhitespaceState.whitespace_7:
                    return "whitespace";

                case idwhitespaceState.whitespace_8:
                    return "whitespace";

                case idwhitespaceState.whitespace_9:
                    return "whitespace";

                case idwhitespaceState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getid_ignore(string input, ref int i)
        {
            return GetItem(input, ref i, false, "id-ignore", (int)(idignoreState.start), (int)(idignoreState.end), this.GetValidNextStates_id_ignore, this.GetStateTag_id_ignore, this.GetSubSpan_id_ignore);
        }

        public Int32[] GetValidNextStates_id_ignore(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((idignoreState)(currentState))
            {
                case idignoreState.start:
                    if (ch == 'i' || ch == 'I')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_0));
                    }
                    break;

                case idignoreState.ignore_0:
                    if (ch == 'g' || ch == 'G')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_1));
                    }
                    break;

                case idignoreState.ignore_1:
                    if (ch == 'n' || ch == 'N')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_2));
                    }
                    break;

                case idignoreState.ignore_2:
                    if (ch == 'o' || ch == 'O')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_3));
                    }
                    break;

                case idignoreState.ignore_3:
                    if (ch == 'r' || ch == 'R')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_4));
                    }
                    break;

                case idignoreState.ignore_4:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(idignoreState.ignore_5));
                    }
                    break;

                case idignoreState.ignore_5:
                    linksToEnd = true;
                    break;

                case idignoreState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_id_ignore(string input, ref int i, int nextState)
        {
            switch ((idignoreState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_id_ignore(int state)
        {
            switch ((idignoreState)(state))
            {
                case idignoreState.start:
                    return "start";

                case idignoreState.ignore_0:
                    return "ignore";

                case idignoreState.ignore_1:
                    return "ignore";

                case idignoreState.ignore_2:
                    return "ignore";

                case idignoreState.ignore_3:
                    return "ignore";

                case idignoreState.ignore_4:
                    return "ignore";

                case idignoreState.ignore_5:
                    return "ignore";

                case idignoreState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getid_case(string input, ref int i)
        {
            return GetItem(input, ref i, false, "id-case", (int)(idcaseState.start), (int)(idcaseState.end), this.GetValidNextStates_id_case, this.GetStateTag_id_case, this.GetSubSpan_id_case);
        }

        public Int32[] GetValidNextStates_id_case(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((idcaseState)(currentState))
            {
                case idcaseState.start:
                    if (ch == 'c' || ch == 'C')
                    {
                        validNextStates.Add((int)(idcaseState.case_0));
                    }
                    break;

                case idcaseState.case_0:
                    if (ch == 'a' || ch == 'A')
                    {
                        validNextStates.Add((int)(idcaseState.case_1));
                    }
                    break;

                case idcaseState.case_1:
                    if (ch == 's' || ch == 'S')
                    {
                        validNextStates.Add((int)(idcaseState.case_2));
                    }
                    break;

                case idcaseState.case_2:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(idcaseState.case_3));
                    }
                    break;

                case idcaseState.case_3:
                    linksToEnd = true;
                    break;

                case idcaseState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_id_case(string input, ref int i, int nextState)
        {
            switch ((idcaseState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_id_case(int state)
        {
            switch ((idcaseState)(state))
            {
                case idcaseState.start:
                    return "start";

                case idcaseState.case_0:
                    return "case";

                case idcaseState.case_1:
                    return "case";

                case idcaseState.case_2:
                    return "case";

                case idcaseState.case_3:
                    return "case";

                case idcaseState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getexpr(string input, ref int i)
        {
            return GetItem(input, ref i, true, "expr", (int)(exprState.start), (int)(exprState.end), this.GetValidNextStates_expr, this.GetStateTag_expr, this.GetSubSpan_expr);
        }

        public Int32[] GetValidNextStates_expr(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((exprState)(currentState))
            {
                case exprState.start:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[')
                    {
                        validNextStates.Add((int)(exprState.subexpr));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(exprState.orexpr));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(exprState.comment));
                    }
                    break;

                case exprState.subexpr:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[')
                    {
                        validNextStates.Add((int)(exprState.subexpr));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(exprState.orexpr));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(exprState.comment));
                    }
                    linksToEnd = true;
                    break;

                case exprState.orexpr:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[')
                    {
                        validNextStates.Add((int)(exprState.subexpr));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(exprState.orexpr));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(exprState.comment));
                    }
                    linksToEnd = true;
                    break;

                case exprState.comment:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[')
                    {
                        validNextStates.Add((int)(exprState.subexpr));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(exprState.orexpr));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(exprState.comment));
                    }
                    linksToEnd = true;
                    break;

                case exprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_expr(string input, ref int i, int nextState)
        {
            switch ((exprState)(nextState))
            {
                case exprState.subexpr:
                    return Getsubexpr(input, ref i);

                case exprState.orexpr:
                    return Getorexpr(input, ref i);

                case exprState.comment:
                    return Getcomment(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_expr(int state)
        {
            switch ((exprState)(state))
            {
                case exprState.start:
                    return "start";

                case exprState.subexpr:
                    return "subexpr";

                case exprState.orexpr:
                    return "orexpr";

                case exprState.comment:
                    return "comment";

                case exprState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getorexpr(string input, ref int i)
        {
            return GetItem(input, ref i, true, "orexpr", (int)(orexprState.start), (int)(orexprState.end), this.GetValidNextStates_orexpr, this.GetStateTag_orexpr, this.GetSubSpan_orexpr);
        }

        public Int32[] GetValidNextStates_orexpr(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((orexprState)(currentState))
            {
                case orexprState.start:
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(orexprState.oparen));
                    }
                    break;

                case orexprState.oparen:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[' || ch == '(' || ch == '/')
                    {
                        validNextStates.Add((int)(orexprState.expr_0));
                    }
                    break;

                case orexprState.expr_0:
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(orexprState.pipe));
                    }
                    if (ch == ')')
                    {
                        validNextStates.Add((int)(orexprState.cparen));
                    }
                    break;

                case orexprState.pipe:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_' || ch == '\'' || ch == '[' || ch == '(' || ch == '/')
                    {
                        validNextStates.Add((int)(orexprState.expr_1));
                    }
                    break;

                case orexprState.cparen:
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        validNextStates.Add((int)(orexprState.modifier));
                    }
                    linksToEnd = true;
                    break;

                case orexprState.expr_1:
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(orexprState.pipe));
                    }
                    if (ch == ')')
                    {
                        validNextStates.Add((int)(orexprState.cparen));
                    }
                    break;

                case orexprState.modifier:
                    linksToEnd = true;
                    break;

                case orexprState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_orexpr(string input, ref int i, int nextState)
        {
            switch ((orexprState)(nextState))
            {
                case orexprState.expr_0:
                    return Getexpr(input, ref i);

                case orexprState.expr_1:
                    return Getexpr(input, ref i);

                case orexprState.modifier:
                    return Getmodifier(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_orexpr(int state)
        {
            switch ((orexprState)(state))
            {
                case orexprState.start:
                    return "start";

                case orexprState.oparen:
                    return "(";

                case orexprState.expr_0:
                    return "expr";

                case orexprState.pipe:
                    return "|";

                case orexprState.cparen:
                    return ")";

                case orexprState.expr_1:
                    return "expr";

                case orexprState.modifier:
                    return "modifier";

                case orexprState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getsubexpr(string input, ref int i)
        {
            return GetItem(input, ref i, true, "subexpr", (int)(subexprState.start), (int)(subexprState.end), this.GetValidNextStates_subexpr, this.GetStateTag_subexpr, this.GetSubSpan_subexpr);
        }

        public Int32[] GetValidNextStates_subexpr(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((subexprState)(currentState))
            {
                case subexprState.start:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(subexprState.identifier));
                    }
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(subexprState.literal));
                    }
                    if (ch == '[')
                    {
                        validNextStates.Add((int)(subexprState.charclass));
                    }
                    break;

                case subexprState.identifier:
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        validNextStates.Add((int)(subexprState.modifier));
                    }
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(subexprState.colon));
                    }
                    linksToEnd = true;
                    break;

                case subexprState.literal:
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        validNextStates.Add((int)(subexprState.modifier));
                    }
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(subexprState.colon));
                    }
                    linksToEnd = true;
                    break;

                case subexprState.charclass:
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        validNextStates.Add((int)(subexprState.modifier));
                    }
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(subexprState.colon));
                    }
                    linksToEnd = true;
                    break;

                case subexprState.modifier:
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(subexprState.colon));
                    }
                    linksToEnd = true;
                    break;

                case subexprState.colon:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(subexprState.tag));
                    }
                    break;

                case subexprState.end:
                    break;

                case subexprState.tag:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_subexpr(string input, ref int i, int nextState)
        {
            switch ((subexprState)(nextState))
            {
                case subexprState.identifier:
                    return Getidentifier(input, ref i);

                case subexprState.literal:
                    return Getliteral(input, ref i);

                case subexprState.charclass:
                    return Getcharclass(input, ref i);

                case subexprState.modifier:
                    return Getmodifier(input, ref i);

                case subexprState.tag:
                    return Getidentifier(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_subexpr(int state)
        {
            switch ((subexprState)(state))
            {
                case subexprState.start:
                    return "start";

                case subexprState.identifier:
                    return "identifier";

                case subexprState.literal:
                    return "literal";

                case subexprState.charclass:
                    return "charclass";

                case subexprState.modifier:
                    return "modifier";

                case subexprState.colon:
                    return ":";

                case subexprState.end:
                    return "end";

                case subexprState.tag:
                    return "tag";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getmodifier(string input, ref int i)
        {
            return GetItem(input, ref i, true, "modifier", (int)(modifierState.start), (int)(modifierState.end), this.GetValidNextStates_modifier, this.GetStateTag_modifier, this.GetSubSpan_modifier);
        }

        public Int32[] GetValidNextStates_modifier(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((modifierState)(currentState))
            {
                case modifierState.start:
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        validNextStates.Add((int)(modifierState.star_plus_question));
                    }
                    break;

                case modifierState.star_plus_question:
                    linksToEnd = true;
                    break;

                case modifierState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_modifier(string input, ref int i, int nextState)
        {
            switch ((modifierState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_modifier(int state)
        {
            switch ((modifierState)(state))
            {
                case modifierState.start:
                    return "start";

                case modifierState.star_plus_question:
                    return "*+?";

                case modifierState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getnumber(string input, ref int i)
        {
            return GetItem(input, ref i, false, "number", (int)(numberState.start), (int)(numberState.end), this.GetValidNextStates_number, this.GetStateTag_number, this.GetSubSpan_number);
        }

        public Int32[] GetValidNextStates_number(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((numberState)(currentState))
            {
                case numberState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(numberState.class_digit));
                    }
                    break;

                case numberState.class_digit:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(numberState.class_digit));
                    }
                    linksToEnd = true;
                    break;

                case numberState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_number(string input, ref int i, int nextState)
        {
            switch ((numberState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_number(int state)
        {
            switch ((numberState)(state))
            {
                case numberState.start:
                    return "start";

                case numberState.class_digit:
                    return "\\d";

                case numberState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getidentifier(string input, ref int i)
        {
            return GetItem(input, ref i, false, "identifier", (int)(identifierState.start), (int)(identifierState.end), this.GetValidNextStates_identifier, this.GetStateTag_identifier, this.GetSubSpan_identifier);
        }

        public Int32[] GetValidNextStates_identifier(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((identifierState)(currentState))
            {
                case identifierState.start:
                    if (char.IsLetter(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(identifierState.w__));
                    }
                    break;

                case identifierState.w__:
                    if (char.IsLetterOrDigit(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(identifierState.w__d));
                    }
                    linksToEnd = true;
                    break;

                case identifierState.w__d:
                    if (char.IsLetterOrDigit(ch) || ch == '-' || ch == '_')
                    {
                        validNextStates.Add((int)(identifierState.w__d));
                    }
                    linksToEnd = true;
                    break;

                case identifierState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_identifier(string input, ref int i, int nextState)
        {
            switch ((identifierState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_identifier(int state)
        {
            switch ((identifierState)(state))
            {
                case identifierState.start:
                    return "start";

                case identifierState.w__:
                    return "\\w-_";

                case identifierState.w__d:
                    return "\\w-_\\d";

                case identifierState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getliteral(string input, ref int i)
        {
            return GetItem(input, ref i, false, "literal", (int)(literalState.start), (int)(literalState.end), this.GetValidNextStates_literal, this.GetStateTag_literal, this.GetSubSpan_literal);
        }

        public Int32[] GetValidNextStates_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((literalState)(currentState))
            {
                case literalState.start:
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(literalState.quote_0));
                    }
                    break;

                case literalState.quote_0:
                    if (!(ch == '\''))
                    {
                        validNextStates.Add((int)(literalState.litchar));
                    }
                    break;

                case literalState.litchar:
                    if (!(ch == '\''))
                    {
                        validNextStates.Add((int)(literalState.litchar));
                    }
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(literalState.quote_1));
                    }
                    break;

                case literalState.quote_1:
                    linksToEnd = true;
                    break;

                case literalState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_literal(string input, ref int i, int nextState)
        {
            switch ((literalState)(nextState))
            {
                case literalState.litchar:
                    return Getlitchar(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_literal(int state)
        {
            switch ((literalState)(state))
            {
                case literalState.start:
                    return "start";

                case literalState.quote_0:
                    return "'";

                case literalState.litchar:
                    return "litchar";

                case literalState.quote_1:
                    return "'";

                case literalState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getlitchar(string input, ref int i)
        {
            return GetItem(input, ref i, false, "litchar", (int)(litcharState.start), (int)(litcharState.end), this.GetValidNextStates_litchar, this.GetStateTag_litchar, this.GetSubSpan_litchar);
        }

        public Int32[] GetValidNextStates_litchar(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((litcharState)(currentState))
            {
                case litcharState.start:
                    if (!(ch == '\\' || ch == '\''))
                    {
                        validNextStates.Add((int)(litcharState.symbols));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(litcharState.bslash));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(litcharState.unicodechar));
                    }
                    break;

                case litcharState.symbols:
                    linksToEnd = true;
                    break;

                case litcharState.bslash:
                    if (ch == 'w' || ch == 'l' || ch == 'd' || ch == 's' || ch == 'r' || ch == 'n' || ch == 't' || ch == '\\' || ch == '\'')
                    {
                        validNextStates.Add((int)(litcharState.wldsrnt));
                    }
                    break;

                case litcharState.unicodechar:
                    linksToEnd = true;
                    break;

                case litcharState.end:
                    break;

                case litcharState.wldsrnt:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_litchar(string input, ref int i, int nextState)
        {
            switch ((litcharState)(nextState))
            {
                case litcharState.unicodechar:
                    return Getunicodechar(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_litchar(int state)
        {
            switch ((litcharState)(state))
            {
                case litcharState.start:
                    return "start";

                case litcharState.symbols:
                    return "^\\\\'";

                case litcharState.bslash:
                    return "\\";

                case litcharState.unicodechar:
                    return "unicodechar";

                case litcharState.end:
                    return "end";

                case litcharState.wldsrnt:
                    return "wldsrnt\\\\'";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getcharclass(string input, ref int i)
        {
            return GetItem(input, ref i, false, "charclass", (int)(charclassState.start), (int)(charclassState.end), this.GetValidNextStates_charclass, this.GetStateTag_charclass, this.GetSubSpan_charclass);
        }

        public Int32[] GetValidNextStates_charclass(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((charclassState)(currentState))
            {
                case charclassState.start:
                    if (ch == '[')
                    {
                        validNextStates.Add((int)(charclassState.obracket));
                    }
                    break;

                case charclassState.obracket:
                    if (ch == '^')
                    {
                        validNextStates.Add((int)(charclassState.chevron));
                    }
                    if (!(ch == '[' || ch == ']'))
                    {
                        validNextStates.Add((int)(charclassState.charclasschar));
                    }
                    break;

                case charclassState.chevron:
                    if (!(ch == '[' || ch == ']'))
                    {
                        validNextStates.Add((int)(charclassState.charclasschar));
                    }
                    break;

                case charclassState.charclasschar:
                    if (!(ch == '[' || ch == ']'))
                    {
                        validNextStates.Add((int)(charclassState.charclasschar));
                    }
                    if (ch == ']')
                    {
                        validNextStates.Add((int)(charclassState.cbracket));
                    }
                    break;

                case charclassState.cbracket:
                    linksToEnd = true;
                    break;

                case charclassState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_charclass(string input, ref int i, int nextState)
        {
            switch ((charclassState)(nextState))
            {
                case charclassState.charclasschar:
                    return Getcharclasschar(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_charclass(int state)
        {
            switch ((charclassState)(state))
            {
                case charclassState.start:
                    return "start";

                case charclassState.obracket:
                    return "[";

                case charclassState.chevron:
                    return "^";

                case charclassState.charclasschar:
                    return "charclasschar";

                case charclassState.cbracket:
                    return "]";

                case charclassState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getcharclasschar(string input, ref int i)
        {
            return GetItem(input, ref i, false, "charclasschar", (int)(charclasscharState.start), (int)(charclasscharState.end), this.GetValidNextStates_charclasschar, this.GetStateTag_charclasschar, this.GetSubSpan_charclasschar);
        }

        public Int32[] GetValidNextStates_charclasschar(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((charclasscharState)(currentState))
            {
                case charclasscharState.start:
                    if (!(ch == '\\' || ch == '[' || ch == ']'))
                    {
                        validNextStates.Add((int)(charclasscharState.symbols));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(charclasscharState.bslash));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(charclasscharState.unicodechar));
                    }
                    break;

                case charclasscharState.symbols:
                    linksToEnd = true;
                    break;

                case charclasscharState.bslash:
                    if (ch == 'w' || ch == 'l' || ch == 'd' || ch == 's' || ch == 'r' || ch == 'n' || ch == 't' || ch == '\\' || ch == '[' || ch == ']')
                    {
                        validNextStates.Add((int)(charclasscharState.wldsrnt));
                    }
                    break;

                case charclasscharState.unicodechar:
                    linksToEnd = true;
                    break;

                case charclasscharState.end:
                    break;

                case charclasscharState.wldsrnt:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_charclasschar(string input, ref int i, int nextState)
        {
            switch ((charclasscharState)(nextState))
            {
                case charclasscharState.unicodechar:
                    return Getunicodechar(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_charclasschar(int state)
        {
            switch ((charclasscharState)(state))
            {
                case charclasscharState.start:
                    return "start";

                case charclasscharState.symbols:
                    return "^\\\\\\[\\]";

                case charclasscharState.bslash:
                    return "\\";

                case charclasscharState.unicodechar:
                    return "unicodechar";

                case charclasscharState.end:
                    return "end";

                case charclasscharState.wldsrnt:
                    return "wldsrnt\\\\\\[\\]";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getunicodechar(string input, ref int i)
        {
            return GetItem(input, ref i, false, "unicodechar", (int)(unicodecharState.start), (int)(unicodecharState.end), this.GetValidNextStates_unicodechar, this.GetStateTag_unicodechar, this.GetSubSpan_unicodechar);
        }

        public Int32[] GetValidNextStates_unicodechar(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((unicodecharState)(currentState))
            {
                case unicodecharState.start:
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(unicodecharState.bslash_letter_x_0));
                    }
                    break;

                case unicodecharState.bslash_letter_x_0:
                    if (ch == 'x' || ch == 'X')
                    {
                        validNextStates.Add((int)(unicodecharState.bslash_letter_x_1));
                    }
                    break;

                case unicodecharState.bslash_letter_x_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodecharState.dabcdef_0));
                    }
                    break;

                case unicodecharState.dabcdef_0:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodecharState.dabcdef_1));
                    }
                    break;

                case unicodecharState.dabcdef_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodecharState.dabcdef_2));
                    }
                    break;

                case unicodecharState.dabcdef_2:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodecharState.dabcdef_3));
                    }
                    break;

                case unicodecharState.dabcdef_3:
                    linksToEnd = true;
                    break;

                case unicodecharState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_unicodechar(string input, ref int i, int nextState)
        {
            switch ((unicodecharState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_unicodechar(int state)
        {
            switch ((unicodecharState)(state))
            {
                case unicodecharState.start:
                    return "start";

                case unicodecharState.bslash_letter_x_0:
                    return "\\x";

                case unicodecharState.bslash_letter_x_1:
                    return "\\x";

                case unicodecharState.dabcdef_0:
                    return "\\dabcdef";

                case unicodecharState.dabcdef_1:
                    return "\\dabcdef";

                case unicodecharState.dabcdef_2:
                    return "\\dabcdef";

                case unicodecharState.dabcdef_3:
                    return "\\dabcdef";

                case unicodecharState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.Span Getcomment(string input, ref int i)
        {
            return GetItem(input, ref i, false, "comment", (int)(commentState.start), (int)(commentState.end), this.GetValidNextStates_comment, this.GetStateTag_comment, this.GetSubSpan_comment);
        }

        public Int32[] GetValidNextStates_comment(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((commentState)(currentState))
            {
                case commentState.start:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(commentState.slash_star_0));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(commentState.slash_slash_0));
                    }
                    break;

                case commentState.slash_star_0:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.slash_star_1));
                    }
                    break;

                case commentState.slash_star_1:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(commentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_1));
                    }
                    break;

                case commentState.slash_slash_0:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(commentState.slash_slash_1));
                    }
                    break;

                case commentState.slash_slash_1:
                    if (!(ch == '\r' || ch == '\n'))
                    {
                        validNextStates.Add((int)(commentState.rn_0));
                    }
                    if (ch == '\r' || ch == '\n')
                    {
                        validNextStates.Add((int)(commentState.rn_1));
                    }
                    break;

                case commentState.chevron_star:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(commentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_1));
                    }
                    break;

                case commentState.star_0:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_0));
                    }
                    if (!(ch == '*' || ch == '/'))
                    {
                        validNextStates.Add((int)(commentState.chevron_star_slash));
                    }
                    break;

                case commentState.star_1:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_1));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(commentState.slash));
                    }
                    break;

                case commentState.rn_0:
                    if (!(ch == '\r' || ch == '\n'))
                    {
                        validNextStates.Add((int)(commentState.rn_0));
                    }
                    if (ch == '\r' || ch == '\n')
                    {
                        validNextStates.Add((int)(commentState.rn_1));
                    }
                    break;

                case commentState.rn_1:
                    if (ch == '\r' || ch == '\n')
                    {
                        validNextStates.Add((int)(commentState.rn_1));
                    }
                    linksToEnd = true;
                    break;

                case commentState.chevron_star_slash:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(commentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(commentState.star_1));
                    }
                    break;

                case commentState.slash:
                    linksToEnd = true;
                    break;

                case commentState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.Span GetSubSpan_comment(string input, ref int i, int nextState)
        {
            switch ((commentState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.Span(i, 1, input);
            }
        }

        public string GetStateTag_comment(int state)
        {
            switch ((commentState)(state))
            {
                case commentState.start:
                    return "start";

                case commentState.slash_star_0:
                    return "/*";

                case commentState.slash_star_1:
                    return "/*";

                case commentState.slash_slash_0:
                    return "//";

                case commentState.slash_slash_1:
                    return "//";

                case commentState.chevron_star:
                    return "^*";

                case commentState.star_0:
                    return "*";

                case commentState.star_1:
                    return "*";

                case commentState.rn_0:
                    return "^\\r\\n";

                case commentState.rn_1:
                    return "\\r\\n";

                case commentState.chevron_star_slash:
                    return "^*/";

                case commentState.slash:
                    return "/";

                case commentState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }
    }
}


