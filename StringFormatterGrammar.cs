
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

namespace MetaphysicsIndustries.Giza
{
    public class StringFormatterGrammar : NGrammar
    {
        public NDefinition def_escape = new NDefinition("escape");
        public NDefinition def_format = new NDefinition("format");
        public NDefinition def_name = new NDefinition("name");
        public NDefinition def_param = new NDefinition("param");
        public NDefinition def_text = new NDefinition("text");

        public CharNode node_escape_0__007B__007B_;
        public CharNode node_escape_1__007B__007B_;
        public CharNode node_escape_2__007D__007D_;
        public CharNode node_escape_3__007D__007D_;
        public DefRefNode node_format_0_text;
        public DefRefNode node_format_1_escape;
        public DefRefNode node_format_2_param;
        public CharNode node_name_0__005C_l_005F_;
        public CharNode node_name_1__005C_l_005C_d;
        public CharNode node_param_0__007B_;
        public CharNode node_param_1__005C_s;
        public DefRefNode node_param_2_name;
        public CharNode node_param_3__005C_s;
        public CharNode node_param_4__007D_;
        public CharNode node_text_0__005E__007B__007D_;

        public StringFormatterGrammar()
        {
            Definitions.Add(def_escape);
            Definitions.Add(def_format);
            Definitions.Add(def_name);
            Definitions.Add(def_param);
            Definitions.Add(def_text);

            def_escape.Directives.Add(DefinitionDirective.MindWhitespace);
            node_escape_0__007B__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{{");
            node_escape_1__007B__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{{");
            node_escape_2__007D__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}}");
            node_escape_3__007D__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}}");
            def_escape.Nodes.Add(node_escape_0__007B__007B_);
            def_escape.Nodes.Add(node_escape_1__007B__007B_);
            def_escape.Nodes.Add(node_escape_2__007D__007D_);
            def_escape.Nodes.Add(node_escape_3__007D__007D_);
            def_escape.StartNodes.Add(node_escape_0__007B__007B_);
            def_escape.StartNodes.Add(node_escape_2__007D__007D_);
            def_escape.EndNodes.Add(node_escape_1__007B__007B_);
            def_escape.EndNodes.Add(node_escape_3__007D__007D_);
            node_escape_0__007B__007B_.NextNodes.Add(node_escape_1__007B__007B_);
            node_escape_2__007D__007D_.NextNodes.Add(node_escape_3__007D__007D_);

            def_format.Directives.Add(DefinitionDirective.MindWhitespace);
            node_format_0_text = new DefRefNode(def_text, "text");
            node_format_1_escape = new DefRefNode(def_escape, "escape");
            node_format_2_param = new DefRefNode(def_param, "param");
            def_format.Nodes.Add(node_format_0_text);
            def_format.Nodes.Add(node_format_1_escape);
            def_format.Nodes.Add(node_format_2_param);
            def_format.StartNodes.Add(node_format_0_text);
            def_format.StartNodes.Add(node_format_1_escape);
            def_format.StartNodes.Add(node_format_2_param);
            def_format.EndNodes.Add(node_format_0_text);
            def_format.EndNodes.Add(node_format_1_escape);
            def_format.EndNodes.Add(node_format_2_param);
            node_format_0_text.NextNodes.Add(node_format_0_text);
            node_format_0_text.NextNodes.Add(node_format_1_escape);
            node_format_0_text.NextNodes.Add(node_format_2_param);
            node_format_1_escape.NextNodes.Add(node_format_0_text);
            node_format_1_escape.NextNodes.Add(node_format_1_escape);
            node_format_1_escape.NextNodes.Add(node_format_2_param);
            node_format_2_param.NextNodes.Add(node_format_0_text);
            node_format_2_param.NextNodes.Add(node_format_1_escape);
            node_format_2_param.NextNodes.Add(node_format_2_param);

            def_name.Directives.Add(DefinitionDirective.MindWhitespace);
            node_name_0__005C_l_005F_ = new CharNode(CharClass.FromUndelimitedCharClassText("\\l_"), "\\l_");
            node_name_1__005C_l_005C_d = new CharNode(CharClass.FromUndelimitedCharClassText("\\l\\d"), "\\l\\d");
            def_name.Nodes.Add(node_name_0__005C_l_005F_);
            def_name.Nodes.Add(node_name_1__005C_l_005C_d);
            def_name.StartNodes.Add(node_name_0__005C_l_005F_);
            def_name.EndNodes.Add(node_name_1__005C_l_005C_d);
            def_name.EndNodes.Add(node_name_0__005C_l_005F_);
            node_name_0__005C_l_005F_.NextNodes.Add(node_name_1__005C_l_005C_d);
            node_name_1__005C_l_005C_d.NextNodes.Add(node_name_1__005C_l_005C_d);

            def_param.Directives.Add(DefinitionDirective.MindWhitespace);
            node_param_0__007B_ = new CharNode(CharClass.FromUndelimitedCharClassText("{"), "{");
            node_param_1__005C_s = new CharNode(CharClass.FromUndelimitedCharClassText("\\s"), "\\s");
            node_param_2_name = new DefRefNode(def_name, "name");
            node_param_3__005C_s = new CharNode(CharClass.FromUndelimitedCharClassText("\\s"), "\\s");
            node_param_4__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("}"), "}");
            def_param.Nodes.Add(node_param_0__007B_);
            def_param.Nodes.Add(node_param_1__005C_s);
            def_param.Nodes.Add(node_param_2_name);
            def_param.Nodes.Add(node_param_3__005C_s);
            def_param.Nodes.Add(node_param_4__007D_);
            def_param.StartNodes.Add(node_param_0__007B_);
            def_param.EndNodes.Add(node_param_4__007D_);
            node_param_0__007B_.NextNodes.Add(node_param_1__005C_s);
            node_param_0__007B_.NextNodes.Add(node_param_2_name);
            node_param_1__005C_s.NextNodes.Add(node_param_1__005C_s);
            node_param_1__005C_s.NextNodes.Add(node_param_2_name);
            node_param_2_name.NextNodes.Add(node_param_3__005C_s);
            node_param_2_name.NextNodes.Add(node_param_4__007D_);
            node_param_3__005C_s.NextNodes.Add(node_param_3__005C_s);
            node_param_3__005C_s.NextNodes.Add(node_param_4__007D_);

            def_text.Directives.Add(DefinitionDirective.Atomic);
            def_text.Directives.Add(DefinitionDirective.MindWhitespace);
            node_text_0__005E__007B__007D_ = new CharNode(CharClass.FromUndelimitedCharClassText("^{}"), "^{}");
            def_text.Nodes.Add(node_text_0__005E__007B__007D_);
            def_text.StartNodes.Add(node_text_0__005E__007B__007D_);
            def_text.EndNodes.Add(node_text_0__005E__007B__007D_);
            node_text_0__005E__007B__007D_.NextNodes.Add(node_text_0__005E__007B__007D_);

        }
    }
}

