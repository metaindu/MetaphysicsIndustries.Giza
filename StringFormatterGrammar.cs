using System;

namespace MetaphysicsIndustries.Giza
{
    public class StringFormatterGrammar : Grammar
    {
        public Definition def_0_format = new Definition("format");
        public Definition def_1_text = new Definition("text");
        public Definition def_2_escape = new Definition("escape");
        public Definition def_3_param = new Definition("param");
        public Definition def_4_name = new Definition("name");

        public DefRefNode node_format_0_text;
        public DefRefNode node_format_1_escape;
        public DefRefNode node_format_2_param;
        public CharNode node_text_0__005E__007B__007D_;
        public CharNode node_escape_0__007B__007B_;
        public CharNode node_escape_1__007B__007B_;
        public CharNode node_escape_2__007D__007D_;
        public CharNode node_escape_3__007D__007D_;
        public CharNode node_param_0__007B_;
        public DefRefNode node_param_1_name;
        public CharNode node_param_2__007D_;
        public CharNode node_name_0__005C_l_005F_;
        public CharNode node_name_1__005C_l_005C_d;

        public StringFormatterGrammar()
        {
            Definitions.Add(def_0_format);
            Definitions.Add(def_1_text);
            Definitions.Add(def_2_escape);
            Definitions.Add(def_3_param);
            Definitions.Add(def_4_name);

            node_format_0_text = new DefRefNode(def_1_text, "text");
            node_format_1_escape = new DefRefNode(def_2_escape, "escape");
            node_format_2_param = new DefRefNode(def_3_param, "param");
            def_0_format.Nodes.Add(node_format_0_text);
            def_0_format.Nodes.Add(node_format_1_escape);
            def_0_format.Nodes.Add(node_format_2_param);
            def_0_format.StartNodes.Add(node_format_0_text);
            def_0_format.StartNodes.Add(node_format_1_escape);
            def_0_format.StartNodes.Add(node_format_2_param);
            def_0_format.EndNodes.Add(node_format_0_text);
            def_0_format.EndNodes.Add(node_format_1_escape);
            def_0_format.EndNodes.Add(node_format_2_param);
            node_format_0_text.NextNodes.Add(node_format_0_text);
            node_format_0_text.NextNodes.Add(node_format_1_escape);
            node_format_0_text.NextNodes.Add(node_format_2_param);
            node_format_1_escape.NextNodes.Add(node_format_0_text);
            node_format_1_escape.NextNodes.Add(node_format_1_escape);
            node_format_1_escape.NextNodes.Add(node_format_2_param);
            node_format_2_param.NextNodes.Add(node_format_0_text);
            node_format_2_param.NextNodes.Add(node_format_1_escape);
            node_format_2_param.NextNodes.Add(node_format_2_param);

            def_1_text.Directives.Add(DefinitionDirective.Atomic);
            def_1_text.Directives.Add(DefinitionDirective.MindWhitespace);
            node_text_0__005E__007B__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("^{}"), "^{}");
            def_1_text.Nodes.Add(node_text_0__005E__007B__007D_);
            def_1_text.StartNodes.Add(node_text_0__005E__007B__007D_);
            def_1_text.EndNodes.Add(node_text_0__005E__007B__007D_);
            node_text_0__005E__007B__007D_.NextNodes.Add(node_text_0__005E__007B__007D_);

            node_escape_0__007B__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{{");
            node_escape_1__007B__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{{");
            node_escape_2__007D__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}}");
            node_escape_3__007D__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}}");
            def_2_escape.Nodes.Add(node_escape_0__007B__007B_);
            def_2_escape.Nodes.Add(node_escape_1__007B__007B_);
            def_2_escape.Nodes.Add(node_escape_2__007D__007D_);
            def_2_escape.Nodes.Add(node_escape_3__007D__007D_);
            def_2_escape.StartNodes.Add(node_escape_0__007B__007B_);
            def_2_escape.StartNodes.Add(node_escape_2__007D__007D_);
            def_2_escape.EndNodes.Add(node_escape_1__007B__007B_);
            def_2_escape.EndNodes.Add(node_escape_3__007D__007D_);
            node_escape_0__007B__007B_.NextNodes.Add(node_escape_1__007B__007B_);
            node_escape_2__007D__007D_.NextNodes.Add(node_escape_3__007D__007D_);

            node_param_0__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{");
            node_param_1_name = new DefRefNode(def_4_name, "name");
            node_param_2__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}");
            def_3_param.Nodes.Add(node_param_0__007B_);
            def_3_param.Nodes.Add(node_param_1_name);
            def_3_param.Nodes.Add(node_param_2__007D_);
            def_3_param.StartNodes.Add(node_param_0__007B_);
            def_3_param.EndNodes.Add(node_param_2__007D_);
            node_param_0__007B_.NextNodes.Add(node_param_1_name);
            node_param_1_name.NextNodes.Add(node_param_2__007D_);

            node_name_0__005C_l_005F_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\l_"), "\\l_");
            node_name_1__005C_l_005C_d = new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d"), "\\l\\d");
            def_4_name.Nodes.Add(node_name_0__005C_l_005F_);
            def_4_name.Nodes.Add(node_name_1__005C_l_005C_d);
            def_4_name.StartNodes.Add(node_name_0__005C_l_005F_);
            def_4_name.EndNodes.Add(node_name_1__005C_l_005C_d);
            def_4_name.EndNodes.Add(node_name_0__005C_l_005F_);
            node_name_0__005C_l_005F_.NextNodes.Add(node_name_1__005C_l_005C_d);
            node_name_1__005C_l_005C_d.NextNodes.Add(node_name_1__005C_l_005C_d);

        }
    }
}

