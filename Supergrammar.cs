using System;

namespace MetaphysicsIndustries.Giza
{
    public class Supergrammar : Grammar
    {
        public Definition def_0_grammar = new Definition("grammar");
        public Definition def_1_definition = new Definition("definition");
        public Definition def_2_directive = new Definition("directive");
        public Definition def_3_directive_item = new Definition("directive-item");
        public Definition def_4_id_whitespace = new Definition("id-whitespace");
        public Definition def_5_id_ignore = new Definition("id-ignore");
        public Definition def_6_id_case = new Definition("id-case");
        public Definition def_7_id_atomic = new Definition("id-atomic");
        public Definition def_8_expr = new Definition("expr");
        public Definition def_9_orexpr = new Definition("orexpr");
        public Definition def_10_subexpr = new Definition("subexpr");
        public Definition def_11_modifier = new Definition("modifier");
        public Definition def_12_number = new Definition("number");
        public Definition def_13_identifier = new Definition("identifier");
        public Definition def_14_literal = new Definition("literal");
        public Definition def_15_charclass = new Definition("charclass");
        public Definition def_16_unicodechar = new Definition("unicodechar");
        public Definition def_17_comment = new Definition("comment");

        public DefRefNode node_grammar_0_definition;
        public DefRefNode node_grammar_1_comment;
        public DefRefNode node_definition_0_directive;
        public DefRefNode node_definition_1_identifier;
        public CharNode node_definition_2__003D_;
        public DefRefNode node_definition_3_expr;
        public CharNode node_definition_4__003B_;
        public CharNode node_directive_0__003C_;
        public DefRefNode node_directive_1_directive_002D_item;
        public CharNode node_directive_2__002C_;
        public DefRefNode node_directive_3_directive_002D_item;
        public CharNode node_directive_4__003E_;
        public DefRefNode node_directive_item_0_id_002D_whitespace;
        public DefRefNode node_directive_item_1_id_002D_ignore;
        public CharNode node_directive_item_2__002D_;
        public DefRefNode node_directive_item_3_id_002D_case;
        public DefRefNode node_directive_item_4_id_002D_atomic;
        public CharNode node_id_whitespace_0_whitespace;
        public CharNode node_id_whitespace_1_whitespace;
        public CharNode node_id_whitespace_2_whitespace;
        public CharNode node_id_whitespace_3_whitespace;
        public CharNode node_id_whitespace_4_whitespace;
        public CharNode node_id_whitespace_5_whitespace;
        public CharNode node_id_whitespace_6_whitespace;
        public CharNode node_id_whitespace_7_whitespace;
        public CharNode node_id_whitespace_8_whitespace;
        public CharNode node_id_whitespace_9_whitespace;
        public CharNode node_id_ignore_0_ignore;
        public CharNode node_id_ignore_1_ignore;
        public CharNode node_id_ignore_2_ignore;
        public CharNode node_id_ignore_3_ignore;
        public CharNode node_id_ignore_4_ignore;
        public CharNode node_id_ignore_5_ignore;
        public CharNode node_id_case_0_case;
        public CharNode node_id_case_1_case;
        public CharNode node_id_case_2_case;
        public CharNode node_id_case_3_case;
        public CharNode node_id_atomic_0_atomic;
        public CharNode node_id_atomic_1_atomic;
        public CharNode node_id_atomic_2_atomic;
        public CharNode node_id_atomic_3_atomic;
        public CharNode node_id_atomic_4_atomic;
        public CharNode node_id_atomic_5_atomic;
        public DefRefNode node_expr_0_subexpr;
        public DefRefNode node_expr_1_orexpr;
        public DefRefNode node_expr_2_comment;
        public CharNode node_orexpr_0__0028_;
        public DefRefNode node_orexpr_1_expr;
        public CharNode node_orexpr_2__007C_;
        public DefRefNode node_orexpr_3_expr;
        public CharNode node_orexpr_4__0029_;
        public DefRefNode node_orexpr_5_modifier;
        public DefRefNode node_subexpr_0_identifier;
        public DefRefNode node_subexpr_1_literal;
        public DefRefNode node_subexpr_2_charclass;
        public DefRefNode node_subexpr_3_modifier;
        public CharNode node_subexpr_4__003A_;
        public DefRefNode node_subexpr_5_tag;
        public CharNode node_modifier_0__002A__002B__003F_;
        public CharNode node_number_0__005C_d;
        public CharNode node_identifier_0__005C_l_002D__;
        public CharNode node_identifier_1__005C_l_005C_d_002D__;
        public CharNode node_literal_0__0027_;
        public CharNode node_literal_1__005E__005C__005C__0027_;
        public CharNode node_literal_2__005C_;
        public CharNode node_literal_3_rnt_005C__005C__0027_;
        public DefRefNode node_literal_4_unicodechar;
        public CharNode node_literal_5__0027_;
        public CharNode node_charclass_0__005B_;
        public CharNode node_charclass_1__005E__005C__005C__005C__005B__005C__005D_;
        public CharNode node_charclass_2__005C_;
        public CharNode node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_;
        public DefRefNode node_charclass_4_unicodechar;
        public CharNode node_charclass_5__005D_;
        public CharNode node_unicodechar_0__005C_x;
        public CharNode node_unicodechar_1__005C_x;
        public CharNode node_unicodechar_2__005C_dabcdef;
        public CharNode node_unicodechar_3__005C_dabcdef;
        public CharNode node_unicodechar_4__005C_dabcdef;
        public CharNode node_unicodechar_5__005C_dabcdef;
        public CharNode node_comment_0__002F__002A_;
        public CharNode node_comment_1__002F__002A_;
        public CharNode node_comment_2__005E__002A_;
        public CharNode node_comment_3__002A_;
        public CharNode node_comment_4__005E__002A__002F_;
        public CharNode node_comment_5__002A_;
        public CharNode node_comment_6__002F_;
        public CharNode node_comment_7__002F__002F_;
        public CharNode node_comment_8__002F__002F_;
        public CharNode node_comment_9__005E__005C_r_005C_n;
        public CharNode node_comment_10__005C_r_005C_n;

        public Supergrammar()
        {
            Definitions.Add(def_0_grammar);
            Definitions.Add(def_1_definition);
            Definitions.Add(def_2_directive);
            Definitions.Add(def_3_directive_item);
            Definitions.Add(def_4_id_whitespace);
            Definitions.Add(def_5_id_ignore);
            Definitions.Add(def_6_id_case);
            Definitions.Add(def_7_id_atomic);
            Definitions.Add(def_8_expr);
            Definitions.Add(def_9_orexpr);
            Definitions.Add(def_10_subexpr);
            Definitions.Add(def_11_modifier);
            Definitions.Add(def_12_number);
            Definitions.Add(def_13_identifier);
            Definitions.Add(def_14_literal);
            Definitions.Add(def_15_charclass);
            Definitions.Add(def_16_unicodechar);
            Definitions.Add(def_17_comment);

            node_grammar_0_definition = new DefRefNode(def_1_definition, "definition");
            node_grammar_1_comment = new DefRefNode(def_17_comment, "comment");
            def_0_grammar.Nodes.Add(node_grammar_0_definition);
            def_0_grammar.Nodes.Add(node_grammar_1_comment);
            def_0_grammar.StartNodes.Add(node_grammar_0_definition);
            def_0_grammar.StartNodes.Add(node_grammar_1_comment);
            def_0_grammar.EndNodes.Add(node_grammar_0_definition);
            def_0_grammar.EndNodes.Add(node_grammar_1_comment);
            node_grammar_0_definition.NextNodes.Add(node_grammar_0_definition);
            node_grammar_0_definition.NextNodes.Add(node_grammar_1_comment);
            node_grammar_1_comment.NextNodes.Add(node_grammar_0_definition);
            node_grammar_1_comment.NextNodes.Add(node_grammar_1_comment);

            node_definition_0_directive = new DefRefNode(def_2_directive, "directive");
            node_definition_1_identifier = new DefRefNode(def_13_identifier, "identifier");
            node_definition_2__003D_ = new CharNode(CharClass.FromUndelimitedCharClassText("="), "=");
            node_definition_3_expr = new DefRefNode(def_8_expr, "expr");
            node_definition_4__003B_ = new CharNode(CharClass.FromUndelimitedCharClassText(";"), ";");
            def_1_definition.Nodes.Add(node_definition_0_directive);
            def_1_definition.Nodes.Add(node_definition_1_identifier);
            def_1_definition.Nodes.Add(node_definition_2__003D_);
            def_1_definition.Nodes.Add(node_definition_3_expr);
            def_1_definition.Nodes.Add(node_definition_4__003B_);
            def_1_definition.StartNodes.Add(node_definition_0_directive);
            def_1_definition.StartNodes.Add(node_definition_1_identifier);
            def_1_definition.EndNodes.Add(node_definition_4__003B_);
            node_definition_0_directive.NextNodes.Add(node_definition_0_directive);
            node_definition_0_directive.NextNodes.Add(node_definition_1_identifier);
            node_definition_1_identifier.NextNodes.Add(node_definition_2__003D_);
            node_definition_2__003D_.NextNodes.Add(node_definition_3_expr);
            node_definition_3_expr.NextNodes.Add(node_definition_4__003B_);

            node_directive_0__003C_ = new CharNode(CharClass.FromUndelimitedCharClassText("<"), "<");
            node_directive_1_directive_002D_item = new DefRefNode(def_3_directive_item, "directive-item");
            node_directive_2__002C_ = new CharNode(CharClass.FromUndelimitedCharClassText(","), ",");
            node_directive_3_directive_002D_item = new DefRefNode(def_3_directive_item, "directive-item");
            node_directive_4__003E_ = new CharNode(CharClass.FromUndelimitedCharClassText(">"), ">");
            def_2_directive.Nodes.Add(node_directive_0__003C_);
            def_2_directive.Nodes.Add(node_directive_1_directive_002D_item);
            def_2_directive.Nodes.Add(node_directive_2__002C_);
            def_2_directive.Nodes.Add(node_directive_3_directive_002D_item);
            def_2_directive.Nodes.Add(node_directive_4__003E_);
            def_2_directive.StartNodes.Add(node_directive_0__003C_);
            def_2_directive.EndNodes.Add(node_directive_4__003E_);
            node_directive_0__003C_.NextNodes.Add(node_directive_1_directive_002D_item);
            node_directive_1_directive_002D_item.NextNodes.Add(node_directive_2__002C_);
            node_directive_1_directive_002D_item.NextNodes.Add(node_directive_4__003E_);
            node_directive_2__002C_.NextNodes.Add(node_directive_3_directive_002D_item);
            node_directive_3_directive_002D_item.NextNodes.Add(node_directive_2__002C_);
            node_directive_3_directive_002D_item.NextNodes.Add(node_directive_4__003E_);

            node_directive_item_0_id_002D_whitespace = new DefRefNode(def_4_id_whitespace, "id-whitespace");
            node_directive_item_1_id_002D_ignore = new DefRefNode(def_5_id_ignore, "id-ignore");
            node_directive_item_2__002D_ = new CharNode(CharClass.FromUndelimitedCharClassText("-"), "-");
            node_directive_item_3_id_002D_case = new DefRefNode(def_6_id_case, "id-case");
            node_directive_item_4_id_002D_atomic = new DefRefNode(def_7_id_atomic, "id-atomic");
            def_3_directive_item.Nodes.Add(node_directive_item_0_id_002D_whitespace);
            def_3_directive_item.Nodes.Add(node_directive_item_1_id_002D_ignore);
            def_3_directive_item.Nodes.Add(node_directive_item_2__002D_);
            def_3_directive_item.Nodes.Add(node_directive_item_3_id_002D_case);
            def_3_directive_item.Nodes.Add(node_directive_item_4_id_002D_atomic);
            def_3_directive_item.StartNodes.Add(node_directive_item_0_id_002D_whitespace);
            def_3_directive_item.StartNodes.Add(node_directive_item_1_id_002D_ignore);
            def_3_directive_item.StartNodes.Add(node_directive_item_4_id_002D_atomic);
            def_3_directive_item.EndNodes.Add(node_directive_item_0_id_002D_whitespace);
            def_3_directive_item.EndNodes.Add(node_directive_item_3_id_002D_case);
            def_3_directive_item.EndNodes.Add(node_directive_item_4_id_002D_atomic);
            node_directive_item_1_id_002D_ignore.NextNodes.Add(node_directive_item_2__002D_);
            node_directive_item_1_id_002D_ignore.NextNodes.Add(node_directive_item_3_id_002D_case);
            node_directive_item_2__002D_.NextNodes.Add(node_directive_item_3_id_002D_case);

            def_4_id_whitespace.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_4_id_whitespace.Directives.Add(DefinitionDirective.IgnoreCase);
            node_id_whitespace_0_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("w"), "whitespace");
            node_id_whitespace_1_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("h"), "whitespace");
            node_id_whitespace_2_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("i"), "whitespace");
            node_id_whitespace_3_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("t"), "whitespace");
            node_id_whitespace_4_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("e"), "whitespace");
            node_id_whitespace_5_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("s"), "whitespace");
            node_id_whitespace_6_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("p"), "whitespace");
            node_id_whitespace_7_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("a"), "whitespace");
            node_id_whitespace_8_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("c"), "whitespace");
            node_id_whitespace_9_whitespace = new CharNode(CharClass.FromUndelimitedCharClassText("e"), "whitespace");
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_0_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_1_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_2_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_3_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_4_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_5_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_6_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_7_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_8_whitespace);
            def_4_id_whitespace.Nodes.Add(node_id_whitespace_9_whitespace);
            def_4_id_whitespace.StartNodes.Add(node_id_whitespace_0_whitespace);
            def_4_id_whitespace.EndNodes.Add(node_id_whitespace_9_whitespace);
            node_id_whitespace_0_whitespace.NextNodes.Add(node_id_whitespace_1_whitespace);
            node_id_whitespace_1_whitespace.NextNodes.Add(node_id_whitespace_2_whitespace);
            node_id_whitespace_2_whitespace.NextNodes.Add(node_id_whitespace_3_whitespace);
            node_id_whitespace_3_whitespace.NextNodes.Add(node_id_whitespace_4_whitespace);
            node_id_whitespace_4_whitespace.NextNodes.Add(node_id_whitespace_5_whitespace);
            node_id_whitespace_5_whitespace.NextNodes.Add(node_id_whitespace_6_whitespace);
            node_id_whitespace_6_whitespace.NextNodes.Add(node_id_whitespace_7_whitespace);
            node_id_whitespace_7_whitespace.NextNodes.Add(node_id_whitespace_8_whitespace);
            node_id_whitespace_8_whitespace.NextNodes.Add(node_id_whitespace_9_whitespace);

            def_5_id_ignore.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_5_id_ignore.Directives.Add(DefinitionDirective.IgnoreCase);
            node_id_ignore_0_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("i"), "ignore");
            node_id_ignore_1_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("g"), "ignore");
            node_id_ignore_2_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("n"), "ignore");
            node_id_ignore_3_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("o"), "ignore");
            node_id_ignore_4_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("r"), "ignore");
            node_id_ignore_5_ignore = new CharNode(CharClass.FromUndelimitedCharClassText("e"), "ignore");
            def_5_id_ignore.Nodes.Add(node_id_ignore_0_ignore);
            def_5_id_ignore.Nodes.Add(node_id_ignore_1_ignore);
            def_5_id_ignore.Nodes.Add(node_id_ignore_2_ignore);
            def_5_id_ignore.Nodes.Add(node_id_ignore_3_ignore);
            def_5_id_ignore.Nodes.Add(node_id_ignore_4_ignore);
            def_5_id_ignore.Nodes.Add(node_id_ignore_5_ignore);
            def_5_id_ignore.StartNodes.Add(node_id_ignore_0_ignore);
            def_5_id_ignore.EndNodes.Add(node_id_ignore_5_ignore);
            node_id_ignore_0_ignore.NextNodes.Add(node_id_ignore_1_ignore);
            node_id_ignore_1_ignore.NextNodes.Add(node_id_ignore_2_ignore);
            node_id_ignore_2_ignore.NextNodes.Add(node_id_ignore_3_ignore);
            node_id_ignore_3_ignore.NextNodes.Add(node_id_ignore_4_ignore);
            node_id_ignore_4_ignore.NextNodes.Add(node_id_ignore_5_ignore);

            def_6_id_case.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_6_id_case.Directives.Add(DefinitionDirective.IgnoreCase);
            node_id_case_0_case = new CharNode(CharClass.FromUndelimitedCharClassText("c"), "case");
            node_id_case_1_case = new CharNode(CharClass.FromUndelimitedCharClassText("a"), "case");
            node_id_case_2_case = new CharNode(CharClass.FromUndelimitedCharClassText("s"), "case");
            node_id_case_3_case = new CharNode(CharClass.FromUndelimitedCharClassText("e"), "case");
            def_6_id_case.Nodes.Add(node_id_case_0_case);
            def_6_id_case.Nodes.Add(node_id_case_1_case);
            def_6_id_case.Nodes.Add(node_id_case_2_case);
            def_6_id_case.Nodes.Add(node_id_case_3_case);
            def_6_id_case.StartNodes.Add(node_id_case_0_case);
            def_6_id_case.EndNodes.Add(node_id_case_3_case);
            node_id_case_0_case.NextNodes.Add(node_id_case_1_case);
            node_id_case_1_case.NextNodes.Add(node_id_case_2_case);
            node_id_case_2_case.NextNodes.Add(node_id_case_3_case);

            def_7_id_atomic.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_7_id_atomic.Directives.Add(DefinitionDirective.IgnoreCase);
            node_id_atomic_0_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("a"), "atomic");
            node_id_atomic_1_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("t"), "atomic");
            node_id_atomic_2_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("o"), "atomic");
            node_id_atomic_3_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("m"), "atomic");
            node_id_atomic_4_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("i"), "atomic");
            node_id_atomic_5_atomic = new CharNode(CharClass.FromUndelimitedCharClassText("c"), "atomic");
            def_7_id_atomic.Nodes.Add(node_id_atomic_0_atomic);
            def_7_id_atomic.Nodes.Add(node_id_atomic_1_atomic);
            def_7_id_atomic.Nodes.Add(node_id_atomic_2_atomic);
            def_7_id_atomic.Nodes.Add(node_id_atomic_3_atomic);
            def_7_id_atomic.Nodes.Add(node_id_atomic_4_atomic);
            def_7_id_atomic.Nodes.Add(node_id_atomic_5_atomic);
            def_7_id_atomic.StartNodes.Add(node_id_atomic_0_atomic);
            def_7_id_atomic.EndNodes.Add(node_id_atomic_5_atomic);
            node_id_atomic_0_atomic.NextNodes.Add(node_id_atomic_1_atomic);
            node_id_atomic_1_atomic.NextNodes.Add(node_id_atomic_2_atomic);
            node_id_atomic_2_atomic.NextNodes.Add(node_id_atomic_3_atomic);
            node_id_atomic_3_atomic.NextNodes.Add(node_id_atomic_4_atomic);
            node_id_atomic_4_atomic.NextNodes.Add(node_id_atomic_5_atomic);

            node_expr_0_subexpr = new DefRefNode(def_10_subexpr, "subexpr");
            node_expr_1_orexpr = new DefRefNode(def_9_orexpr, "orexpr");
            node_expr_2_comment = new DefRefNode(def_17_comment, "comment");
            def_8_expr.Nodes.Add(node_expr_0_subexpr);
            def_8_expr.Nodes.Add(node_expr_1_orexpr);
            def_8_expr.Nodes.Add(node_expr_2_comment);
            def_8_expr.StartNodes.Add(node_expr_0_subexpr);
            def_8_expr.StartNodes.Add(node_expr_1_orexpr);
            def_8_expr.StartNodes.Add(node_expr_2_comment);
            def_8_expr.EndNodes.Add(node_expr_0_subexpr);
            def_8_expr.EndNodes.Add(node_expr_1_orexpr);
            def_8_expr.EndNodes.Add(node_expr_2_comment);
            node_expr_0_subexpr.NextNodes.Add(node_expr_0_subexpr);
            node_expr_0_subexpr.NextNodes.Add(node_expr_1_orexpr);
            node_expr_0_subexpr.NextNodes.Add(node_expr_2_comment);
            node_expr_1_orexpr.NextNodes.Add(node_expr_0_subexpr);
            node_expr_1_orexpr.NextNodes.Add(node_expr_1_orexpr);
            node_expr_1_orexpr.NextNodes.Add(node_expr_2_comment);
            node_expr_2_comment.NextNodes.Add(node_expr_0_subexpr);
            node_expr_2_comment.NextNodes.Add(node_expr_1_orexpr);
            node_expr_2_comment.NextNodes.Add(node_expr_2_comment);

            node_orexpr_0__0028_ = new CharNode(CharClass.FromUndelimitedCharClassText("("), "(");
            node_orexpr_1_expr = new DefRefNode(def_8_expr, "expr");
            node_orexpr_2__007C_ = new CharNode(CharClass.FromUndelimitedCharClassText("|"), "|");
            node_orexpr_3_expr = new DefRefNode(def_8_expr, "expr");
            node_orexpr_4__0029_ = new CharNode(CharClass.FromUndelimitedCharClassText(")"), ")");
            node_orexpr_5_modifier = new DefRefNode(def_11_modifier, "modifier");
            def_9_orexpr.Nodes.Add(node_orexpr_0__0028_);
            def_9_orexpr.Nodes.Add(node_orexpr_1_expr);
            def_9_orexpr.Nodes.Add(node_orexpr_2__007C_);
            def_9_orexpr.Nodes.Add(node_orexpr_3_expr);
            def_9_orexpr.Nodes.Add(node_orexpr_4__0029_);
            def_9_orexpr.Nodes.Add(node_orexpr_5_modifier);
            def_9_orexpr.StartNodes.Add(node_orexpr_0__0028_);
            def_9_orexpr.EndNodes.Add(node_orexpr_5_modifier);
            def_9_orexpr.EndNodes.Add(node_orexpr_4__0029_);
            node_orexpr_0__0028_.NextNodes.Add(node_orexpr_1_expr);
            node_orexpr_1_expr.NextNodes.Add(node_orexpr_2__007C_);
            node_orexpr_1_expr.NextNodes.Add(node_orexpr_4__0029_);
            node_orexpr_2__007C_.NextNodes.Add(node_orexpr_3_expr);
            node_orexpr_3_expr.NextNodes.Add(node_orexpr_2__007C_);
            node_orexpr_3_expr.NextNodes.Add(node_orexpr_4__0029_);
            node_orexpr_4__0029_.NextNodes.Add(node_orexpr_5_modifier);

            node_subexpr_0_identifier = new DefRefNode(def_13_identifier, "identifier");
            node_subexpr_1_literal = new DefRefNode(def_14_literal, "literal");
            node_subexpr_2_charclass = new DefRefNode(def_15_charclass, "charclass");
            node_subexpr_3_modifier = new DefRefNode(def_11_modifier, "modifier");
            node_subexpr_4__003A_ = new CharNode(CharClass.FromUndelimitedCharClassText(":"), ":");
            node_subexpr_5_tag = new DefRefNode(def_13_identifier, "tag");
            def_10_subexpr.Nodes.Add(node_subexpr_0_identifier);
            def_10_subexpr.Nodes.Add(node_subexpr_1_literal);
            def_10_subexpr.Nodes.Add(node_subexpr_2_charclass);
            def_10_subexpr.Nodes.Add(node_subexpr_3_modifier);
            def_10_subexpr.Nodes.Add(node_subexpr_4__003A_);
            def_10_subexpr.Nodes.Add(node_subexpr_5_tag);
            def_10_subexpr.StartNodes.Add(node_subexpr_0_identifier);
            def_10_subexpr.StartNodes.Add(node_subexpr_1_literal);
            def_10_subexpr.StartNodes.Add(node_subexpr_2_charclass);
            def_10_subexpr.EndNodes.Add(node_subexpr_5_tag);
            def_10_subexpr.EndNodes.Add(node_subexpr_3_modifier);
            def_10_subexpr.EndNodes.Add(node_subexpr_0_identifier);
            def_10_subexpr.EndNodes.Add(node_subexpr_1_literal);
            def_10_subexpr.EndNodes.Add(node_subexpr_2_charclass);
            node_subexpr_0_identifier.NextNodes.Add(node_subexpr_3_modifier);
            node_subexpr_0_identifier.NextNodes.Add(node_subexpr_4__003A_);
            node_subexpr_1_literal.NextNodes.Add(node_subexpr_3_modifier);
            node_subexpr_1_literal.NextNodes.Add(node_subexpr_4__003A_);
            node_subexpr_2_charclass.NextNodes.Add(node_subexpr_3_modifier);
            node_subexpr_2_charclass.NextNodes.Add(node_subexpr_4__003A_);
            node_subexpr_3_modifier.NextNodes.Add(node_subexpr_4__003A_);
            node_subexpr_4__003A_.NextNodes.Add(node_subexpr_5_tag);

            node_modifier_0__002A__002B__003F_ = new CharNode(CharClass.FromUndelimitedCharClassText("*+?"), "*+?");
            def_11_modifier.Nodes.Add(node_modifier_0__002A__002B__003F_);
            def_11_modifier.StartNodes.Add(node_modifier_0__002A__002B__003F_);
            def_11_modifier.EndNodes.Add(node_modifier_0__002A__002B__003F_);

            def_12_number.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_12_number.Directives.Add(DefinitionDirective.Atomic);
            node_number_0__005C_d = new CharNode(CharClass.FromUndelimitedCharClassText("\\d"), "\\d");
            def_12_number.Nodes.Add(node_number_0__005C_d);
            def_12_number.StartNodes.Add(node_number_0__005C_d);
            def_12_number.EndNodes.Add(node_number_0__005C_d);
            node_number_0__005C_d.NextNodes.Add(node_number_0__005C_d);

            def_13_identifier.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_13_identifier.Directives.Add(DefinitionDirective.Atomic);
            node_identifier_0__005C_l_002D__ = new CharNode(CharClass.FromUndelimitedCharClassText("\\l-_"), "\\l-_");
            node_identifier_1__005C_l_005C_d_002D__ = new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d-_"), "\\l\\d-_");
            def_13_identifier.Nodes.Add(node_identifier_0__005C_l_002D__);
            def_13_identifier.Nodes.Add(node_identifier_1__005C_l_005C_d_002D__);
            def_13_identifier.StartNodes.Add(node_identifier_0__005C_l_002D__);
            def_13_identifier.EndNodes.Add(node_identifier_1__005C_l_005C_d_002D__);
            def_13_identifier.EndNodes.Add(node_identifier_0__005C_l_002D__);
            node_identifier_0__005C_l_002D__.NextNodes.Add(node_identifier_1__005C_l_005C_d_002D__);
            node_identifier_1__005C_l_005C_d_002D__.NextNodes.Add(node_identifier_1__005C_l_005C_d_002D__);

            def_14_literal.Directives.Add(DefinitionDirective.IncludeWhitespace);
            node_literal_0__0027_ = new CharNode(CharClass.FromUndelimitedCharClassText("'"), "'");
            node_literal_1__005E__005C__005C__0027_ = new CharNode(CharClass.FromUndelimitedCharClassText("^\\\\'"), "^\\\\'");
            node_literal_2__005C_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\\\"), "\\");
            node_literal_3_rnt_005C__005C__0027_ = new CharNode(CharClass.FromUndelimitedCharClassText("rnt\\\\'"), "rnt\\\\'");
            node_literal_4_unicodechar = new DefRefNode(def_16_unicodechar, "unicodechar");
            node_literal_5__0027_ = new CharNode(CharClass.FromUndelimitedCharClassText("'"), "'");
            def_14_literal.Nodes.Add(node_literal_0__0027_);
            def_14_literal.Nodes.Add(node_literal_1__005E__005C__005C__0027_);
            def_14_literal.Nodes.Add(node_literal_2__005C_);
            def_14_literal.Nodes.Add(node_literal_3_rnt_005C__005C__0027_);
            def_14_literal.Nodes.Add(node_literal_4_unicodechar);
            def_14_literal.Nodes.Add(node_literal_5__0027_);
            def_14_literal.StartNodes.Add(node_literal_0__0027_);
            def_14_literal.EndNodes.Add(node_literal_5__0027_);
            node_literal_0__0027_.NextNodes.Add(node_literal_1__005E__005C__005C__0027_);
            node_literal_0__0027_.NextNodes.Add(node_literal_2__005C_);
            node_literal_0__0027_.NextNodes.Add(node_literal_4_unicodechar);
            node_literal_1__005E__005C__005C__0027_.NextNodes.Add(node_literal_1__005E__005C__005C__0027_);
            node_literal_1__005E__005C__005C__0027_.NextNodes.Add(node_literal_2__005C_);
            node_literal_1__005E__005C__005C__0027_.NextNodes.Add(node_literal_4_unicodechar);
            node_literal_1__005E__005C__005C__0027_.NextNodes.Add(node_literal_5__0027_);
            node_literal_2__005C_.NextNodes.Add(node_literal_3_rnt_005C__005C__0027_);
            node_literal_3_rnt_005C__005C__0027_.NextNodes.Add(node_literal_1__005E__005C__005C__0027_);
            node_literal_3_rnt_005C__005C__0027_.NextNodes.Add(node_literal_2__005C_);
            node_literal_3_rnt_005C__005C__0027_.NextNodes.Add(node_literal_4_unicodechar);
            node_literal_3_rnt_005C__005C__0027_.NextNodes.Add(node_literal_5__0027_);
            node_literal_4_unicodechar.NextNodes.Add(node_literal_1__005E__005C__005C__0027_);
            node_literal_4_unicodechar.NextNodes.Add(node_literal_2__005C_);
            node_literal_4_unicodechar.NextNodes.Add(node_literal_4_unicodechar);
            node_literal_4_unicodechar.NextNodes.Add(node_literal_5__0027_);

            def_15_charclass.Directives.Add(DefinitionDirective.IncludeWhitespace);
            node_charclass_0__005B_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\["), "[");
            node_charclass_1__005E__005C__005C__005C__005B__005C__005D_ = new CharNode(CharClass.FromUndelimitedCharClassText("^\\\\\\[\\]"), "^\\\\\\[\\]");
            node_charclass_2__005C_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\\\"), "\\");
            node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_ = new CharNode(CharClass.FromUndelimitedCharClassText("wldsrnt\\\\\\[\\]"), "wldsrnt\\\\\\[\\]");
            node_charclass_4_unicodechar = new DefRefNode(def_16_unicodechar, "unicodechar");
            node_charclass_5__005D_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\]"), "]");
            def_15_charclass.Nodes.Add(node_charclass_0__005B_);
            def_15_charclass.Nodes.Add(node_charclass_1__005E__005C__005C__005C__005B__005C__005D_);
            def_15_charclass.Nodes.Add(node_charclass_2__005C_);
            def_15_charclass.Nodes.Add(node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_);
            def_15_charclass.Nodes.Add(node_charclass_4_unicodechar);
            def_15_charclass.Nodes.Add(node_charclass_5__005D_);
            def_15_charclass.StartNodes.Add(node_charclass_0__005B_);
            def_15_charclass.EndNodes.Add(node_charclass_5__005D_);
            node_charclass_0__005B_.NextNodes.Add(node_charclass_1__005E__005C__005C__005C__005B__005C__005D_);
            node_charclass_0__005B_.NextNodes.Add(node_charclass_2__005C_);
            node_charclass_0__005B_.NextNodes.Add(node_charclass_4_unicodechar);
            node_charclass_1__005E__005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_1__005E__005C__005C__005C__005B__005C__005D_);
            node_charclass_1__005E__005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_2__005C_);
            node_charclass_1__005E__005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_4_unicodechar);
            node_charclass_1__005E__005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_5__005D_);
            node_charclass_2__005C_.NextNodes.Add(node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_);
            node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_1__005E__005C__005C__005C__005B__005C__005D_);
            node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_2__005C_);
            node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_4_unicodechar);
            node_charclass_3_wldsrnt_005C__005C__005C__005B__005C__005D_.NextNodes.Add(node_charclass_5__005D_);
            node_charclass_4_unicodechar.NextNodes.Add(node_charclass_1__005E__005C__005C__005C__005B__005C__005D_);
            node_charclass_4_unicodechar.NextNodes.Add(node_charclass_2__005C_);
            node_charclass_4_unicodechar.NextNodes.Add(node_charclass_4_unicodechar);
            node_charclass_4_unicodechar.NextNodes.Add(node_charclass_5__005D_);

            def_16_unicodechar.Directives.Add(DefinitionDirective.IncludeWhitespace);
            def_16_unicodechar.Directives.Add(DefinitionDirective.IgnoreCase);
            node_unicodechar_0__005C_x = new CharNode(CharClass.FromUndelimitedCharClassText("\\\\"), "\\x");
            node_unicodechar_1__005C_x = new CharNode(CharClass.FromUndelimitedCharClassText("x"), "\\x");
            node_unicodechar_2__005C_dabcdef = new CharNode(CharClass.FromUndelimitedCharClassText("\\dabcdef"), "\\dabcdef");
            node_unicodechar_3__005C_dabcdef = new CharNode(CharClass.FromUndelimitedCharClassText("\\dabcdef"), "\\dabcdef");
            node_unicodechar_4__005C_dabcdef = new CharNode(CharClass.FromUndelimitedCharClassText("\\dabcdef"), "\\dabcdef");
            node_unicodechar_5__005C_dabcdef = new CharNode(CharClass.FromUndelimitedCharClassText("\\dabcdef"), "\\dabcdef");
            def_16_unicodechar.Nodes.Add(node_unicodechar_0__005C_x);
            def_16_unicodechar.Nodes.Add(node_unicodechar_1__005C_x);
            def_16_unicodechar.Nodes.Add(node_unicodechar_2__005C_dabcdef);
            def_16_unicodechar.Nodes.Add(node_unicodechar_3__005C_dabcdef);
            def_16_unicodechar.Nodes.Add(node_unicodechar_4__005C_dabcdef);
            def_16_unicodechar.Nodes.Add(node_unicodechar_5__005C_dabcdef);
            def_16_unicodechar.StartNodes.Add(node_unicodechar_0__005C_x);
            def_16_unicodechar.EndNodes.Add(node_unicodechar_5__005C_dabcdef);
            node_unicodechar_0__005C_x.NextNodes.Add(node_unicodechar_1__005C_x);
            node_unicodechar_1__005C_x.NextNodes.Add(node_unicodechar_2__005C_dabcdef);
            node_unicodechar_2__005C_dabcdef.NextNodes.Add(node_unicodechar_3__005C_dabcdef);
            node_unicodechar_3__005C_dabcdef.NextNodes.Add(node_unicodechar_4__005C_dabcdef);
            node_unicodechar_4__005C_dabcdef.NextNodes.Add(node_unicodechar_5__005C_dabcdef);

            def_17_comment.Directives.Add(DefinitionDirective.IncludeWhitespace);
            node_comment_0__002F__002A_ = new CharNode(CharClass.FromUndelimitedCharClassText("/"), "/*");
            node_comment_1__002F__002A_ = new CharNode(CharClass.FromUndelimitedCharClassText("*"), "/*");
            node_comment_2__005E__002A_ = new CharNode(CharClass.FromUndelimitedCharClassText("^*"), "^*");
            node_comment_3__002A_ = new CharNode(CharClass.FromUndelimitedCharClassText("*"), "*");
            node_comment_4__005E__002A__002F_ = new CharNode(CharClass.FromUndelimitedCharClassText("^*/"), "^*/");
            node_comment_5__002A_ = new CharNode(CharClass.FromUndelimitedCharClassText("*"), "*");
            node_comment_6__002F_ = new CharNode(CharClass.FromUndelimitedCharClassText("/"), "/");
            node_comment_7__002F__002F_ = new CharNode(CharClass.FromUndelimitedCharClassText("/"), "//");
            node_comment_8__002F__002F_ = new CharNode(CharClass.FromUndelimitedCharClassText("/"), "//");
            node_comment_9__005E__005C_r_005C_n = new CharNode(CharClass.FromUndelimitedCharClassText("^\\r\\n"), "^\\r\\n");
            node_comment_10__005C_r_005C_n = new CharNode(CharClass.FromUndelimitedCharClassText("\\r\\n"), "\\r\\n");
            def_17_comment.Nodes.Add(node_comment_0__002F__002A_);
            def_17_comment.Nodes.Add(node_comment_1__002F__002A_);
            def_17_comment.Nodes.Add(node_comment_2__005E__002A_);
            def_17_comment.Nodes.Add(node_comment_3__002A_);
            def_17_comment.Nodes.Add(node_comment_4__005E__002A__002F_);
            def_17_comment.Nodes.Add(node_comment_5__002A_);
            def_17_comment.Nodes.Add(node_comment_6__002F_);
            def_17_comment.Nodes.Add(node_comment_7__002F__002F_);
            def_17_comment.Nodes.Add(node_comment_8__002F__002F_);
            def_17_comment.Nodes.Add(node_comment_9__005E__005C_r_005C_n);
            def_17_comment.Nodes.Add(node_comment_10__005C_r_005C_n);
            def_17_comment.StartNodes.Add(node_comment_0__002F__002A_);
            def_17_comment.StartNodes.Add(node_comment_7__002F__002F_);
            def_17_comment.EndNodes.Add(node_comment_6__002F_);
            def_17_comment.EndNodes.Add(node_comment_10__005C_r_005C_n);
            node_comment_0__002F__002A_.NextNodes.Add(node_comment_1__002F__002A_);
            node_comment_1__002F__002A_.NextNodes.Add(node_comment_2__005E__002A_);
            node_comment_1__002F__002A_.NextNodes.Add(node_comment_3__002A_);
            node_comment_1__002F__002A_.NextNodes.Add(node_comment_5__002A_);
            node_comment_2__005E__002A_.NextNodes.Add(node_comment_2__005E__002A_);
            node_comment_2__005E__002A_.NextNodes.Add(node_comment_3__002A_);
            node_comment_2__005E__002A_.NextNodes.Add(node_comment_5__002A_);
            node_comment_3__002A_.NextNodes.Add(node_comment_3__002A_);
            node_comment_3__002A_.NextNodes.Add(node_comment_4__005E__002A__002F_);
            node_comment_4__005E__002A__002F_.NextNodes.Add(node_comment_2__005E__002A_);
            node_comment_4__005E__002A__002F_.NextNodes.Add(node_comment_3__002A_);
            node_comment_4__005E__002A__002F_.NextNodes.Add(node_comment_5__002A_);
            node_comment_5__002A_.NextNodes.Add(node_comment_5__002A_);
            node_comment_5__002A_.NextNodes.Add(node_comment_6__002F_);
            node_comment_7__002F__002F_.NextNodes.Add(node_comment_8__002F__002F_);
            node_comment_8__002F__002F_.NextNodes.Add(node_comment_9__005E__005C_r_005C_n);
            node_comment_8__002F__002F_.NextNodes.Add(node_comment_10__005C_r_005C_n);
            node_comment_9__005E__005C_r_005C_n.NextNodes.Add(node_comment_9__005E__005C_r_005C_n);
            node_comment_9__005E__005C_r_005C_n.NextNodes.Add(node_comment_10__005C_r_005C_n);
            node_comment_10__005C_r_005C_n.NextNodes.Add(node_comment_10__005C_r_005C_n);

        }
    }
}

