
using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class CSharp2Lexical : MetaphysicsIndustries.Giza.BaseParser
    {
        enum inputState
        {
            start,
            input_section,
            end,
        }
        enum inputsectionState
        {
            start,
            input_section_part,
            end,
        }
        enum inputsectionpartState
        {
            start,
            new_line,
            input_elements,
            pp_directive,
            end,
        }
        enum inputelementsState
        {
            start,
            input_element,
            end,
        }
        enum inputelementState
        {
            start,
            whitespace,
            comment,
            token,
            end,
        }
        enum newlineState
        {
            start,
            x000dx000a_0,
            x000dx000a_1,
            x000dx000a_2,
            x000dx000a_3,
            x000dx000a_4,
            x000dx000a_5,
            x000dx000a_6,
            x000dx000a_7,
            x000dx000a_8,
            x000dx000a_9,
            x000d_0,
            x000d_1,
            x000d_2,
            x000d_3,
            x000d_4,
            x000a_0,
            x000a_1,
            x000a_2,
            x000a_3,
            x000a_4,
            x2085_0,
            x2085_1,
            x2085_2,
            x2085_3,
            x2085_4,
            x2028_0,
            x2028_1,
            x2028_2,
            x2028_3,
            x2028_4,
            x2029_0,
            x2029_1,
            x2029_2,
            x2029_3,
            x2029_4,
            end,
        }
        enum whitespaceState
        {
            start,
            tx0009x0008x000c,
            end,
        }
        enum commentState
        {
            start,
            single_line_comment,
            delimited_comment,
            end,
        }
        enum inputcharactersState
        {
            start,
            nrx0085x2028x2029,
            end,
        }
        enum singlelinecommentState
        {
            start,
            slash_slash_0,
            slash_slash_1,
            input_characters,
            end,
        }
        enum delimitedcommentState
        {
            start,
            slash_star_0,
            slash_star_1,
            chevron_star,
            star_0,
            star_1,
            chevron_slash,
            slash,
            end,
        }
        enum tokenState
        {
            start,
            identifier,
            keyword,
            integer_literal,
            real_literal,
            character_literal,
            string_literal,
            operator_or_punctuator,
            end,
        }
        enum unicodeescapesequenceState
        {
            start,
            bslash_letter_u_0,
            bslash_letter_u_1,
            bslash_letter_U_0,
            bslash_letter_U_1,
            hex_digit_0,
            hex_digit_1,
            hex_digit_2,
            hex_digit_3,
            hex_digit_4,
            hex_digit_5,
            hex_digit_6,
            hex_digit_7,
            end,
            hex_digit_8,
            hex_digit_9,
            hex_digit_10,
            hex_digit_11,
        }
        enum identifierState
        {
            start,
            available_identifier,
            at,
            end,
            identifier_or_keyword,
        }
        enum availableidentifierState
        {
            start,
            identifier_or_keyword,
            end,
        }
        enum identifierorkeywordState
        {
            start,
            bslash_letter_l_score,
            ld_,
            end,
        }
        enum keywordState
        {
            start,
            abstract_0,
            abstract_1,
            abstract_2,
            abstract_3,
            abstract_4,
            abstract_5,
            abstract_6,
            abstract_7,
            as_0,
            as_1,
            base_0,
            base_1,
            base_2,
            base_3,
            bool_0,
            bool_1,
            bool_2,
            bool_3,
            break_0,
            break_1,
            break_2,
            break_3,
            break_4,
            byte_0,
            byte_1,
            byte_2,
            byte_3,
            case_0,
            case_1,
            case_2,
            case_3,
            catch_0,
            catch_1,
            catch_2,
            catch_3,
            catch_4,
            char_0,
            char_1,
            char_2,
            char_3,
            checked_0,
            checked_1,
            checked_2,
            checked_3,
            checked_4,
            checked_5,
            checked_6,
            class_0,
            class_1,
            class_2,
            class_3,
            class_4,
            const_0,
            const_1,
            const_2,
            const_3,
            const_4,
            continue_0,
            continue_1,
            continue_2,
            continue_3,
            continue_4,
            continue_5,
            continue_6,
            continue_7,
            decimal_0,
            decimal_1,
            decimal_2,
            decimal_3,
            decimal_4,
            decimal_5,
            decimal_6,
            default_0,
            default_1,
            default_2,
            default_3,
            default_4,
            default_5,
            default_6,
            delegate_0,
            delegate_1,
            delegate_2,
            delegate_3,
            delegate_4,
            delegate_5,
            delegate_6,
            delegate_7,
            do_0,
            do_1,
            double_0,
            double_1,
            double_2,
            double_3,
            double_4,
            double_5,
            else_0,
            else_1,
            else_2,
            else_3,
            enum_0,
            enum_1,
            enum_2,
            enum_3,
            event_0,
            event_1,
            event_2,
            event_3,
            event_4,
            explicit_0,
            explicit_1,
            explicit_2,
            explicit_3,
            explicit_4,
            explicit_5,
            explicit_6,
            explicit_7,
            extern_0,
            extern_1,
            extern_2,
            extern_3,
            extern_4,
            extern_5,
            false_0,
            false_1,
            false_2,
            false_3,
            false_4,
            finally_0,
            finally_1,
            finally_2,
            finally_3,
            finally_4,
            finally_5,
            finally_6,
            fixed_0,
            fixed_1,
            fixed_2,
            fixed_3,
            fixed_4,
            float_0,
            float_1,
            float_2,
            float_3,
            float_4,
            for_0,
            for_1,
            for_2,
            foreach_0,
            foreach_1,
            foreach_2,
            foreach_3,
            foreach_4,
            foreach_5,
            foreach_6,
            goto_0,
            goto_1,
            goto_2,
            goto_3,
            if_0,
            if_1,
            implicit_0,
            implicit_1,
            implicit_2,
            implicit_3,
            implicit_4,
            implicit_5,
            implicit_6,
            implicit_7,
            in_0,
            in_1,
            int_0,
            int_1,
            int_2,
            interface_0,
            interface_1,
            interface_2,
            interface_3,
            interface_4,
            interface_5,
            interface_6,
            interface_7,
            interface_8,
            internal_0,
            internal_1,
            internal_2,
            internal_3,
            internal_4,
            internal_5,
            internal_6,
            internal_7,
            is_0,
            is_1,
            lock_0,
            lock_1,
            lock_2,
            lock_3,
            long_0,
            long_1,
            long_2,
            long_3,
            namespace_0,
            namespace_1,
            namespace_2,
            namespace_3,
            namespace_4,
            namespace_5,
            namespace_6,
            namespace_7,
            namespace_8,
            new_0,
            new_1,
            new_2,
            null_0,
            null_1,
            null_2,
            null_3,
            object_0,
            object_1,
            object_2,
            object_3,
            object_4,
            object_5,
            operator_0,
            operator_1,
            operator_2,
            operator_3,
            operator_4,
            operator_5,
            operator_6,
            operator_7,
            out_0,
            out_1,
            out_2,
            override_0,
            override_1,
            override_2,
            override_3,
            override_4,
            override_5,
            override_6,
            override_7,
            params_0,
            params_1,
            params_2,
            params_3,
            params_4,
            params_5,
            private_0,
            private_1,
            private_2,
            private_3,
            private_4,
            private_5,
            private_6,
            protected_0,
            protected_1,
            protected_2,
            protected_3,
            protected_4,
            protected_5,
            protected_6,
            protected_7,
            protected_8,
            public_0,
            public_1,
            public_2,
            public_3,
            public_4,
            public_5,
            readonly_0,
            readonly_1,
            readonly_2,
            readonly_3,
            readonly_4,
            readonly_5,
            readonly_6,
            readonly_7,
            ref_0,
            ref_1,
            ref_2,
            return_0,
            return_1,
            return_2,
            return_3,
            return_4,
            return_5,
            sbyte_0,
            sbyte_1,
            sbyte_2,
            sbyte_3,
            sbyte_4,
            sealed_0,
            sealed_1,
            sealed_2,
            sealed_3,
            sealed_4,
            sealed_5,
            short_0,
            short_1,
            short_2,
            short_3,
            short_4,
            sizeof_0,
            sizeof_1,
            sizeof_2,
            sizeof_3,
            sizeof_4,
            sizeof_5,
            stackalloc_0,
            stackalloc_1,
            stackalloc_2,
            stackalloc_3,
            stackalloc_4,
            stackalloc_5,
            stackalloc_6,
            stackalloc_7,
            stackalloc_8,
            stackalloc_9,
            static_0,
            static_1,
            static_2,
            static_3,
            static_4,
            static_5,
            string_0,
            string_1,
            string_2,
            string_3,
            string_4,
            string_5,
            struct_0,
            struct_1,
            struct_2,
            struct_3,
            struct_4,
            struct_5,
            switch_0,
            switch_1,
            switch_2,
            switch_3,
            switch_4,
            switch_5,
            this_0,
            this_1,
            this_2,
            this_3,
            throw_0,
            throw_1,
            throw_2,
            throw_3,
            throw_4,
            true_0,
            true_1,
            true_2,
            true_3,
            try_0,
            try_1,
            try_2,
            typeof_0,
            typeof_1,
            typeof_2,
            typeof_3,
            typeof_4,
            typeof_5,
            uint_0,
            uint_1,
            uint_2,
            uint_3,
            ulong_0,
            ulong_1,
            ulong_2,
            ulong_3,
            ulong_4,
            unchecked_0,
            unchecked_1,
            unchecked_2,
            unchecked_3,
            unchecked_4,
            unchecked_5,
            unchecked_6,
            unchecked_7,
            unchecked_8,
            unsafe_0,
            unsafe_1,
            unsafe_2,
            unsafe_3,
            unsafe_4,
            unsafe_5,
            ushort_0,
            ushort_1,
            ushort_2,
            ushort_3,
            ushort_4,
            ushort_5,
            using_0,
            using_1,
            using_2,
            using_3,
            using_4,
            virtual_0,
            virtual_1,
            virtual_2,
            virtual_3,
            virtual_4,
            virtual_5,
            virtual_6,
            void_0,
            void_1,
            void_2,
            void_3,
            volatile_0,
            volatile_1,
            volatile_2,
            volatile_3,
            volatile_4,
            volatile_5,
            volatile_6,
            volatile_7,
            while_0,
            while_1,
            while_2,
            while_3,
            while_4,
            end,
        }
        enum literalState
        {
            start,
            boolean_literal,
            integer_literal,
            real_literal,
            character_literal,
            string_literal,
            null_literal,
            end,
        }
        enum booleanliteralState
        {
            start,
            true_0,
            true_1,
            true_2,
            true_3,
            false_0,
            false_1,
            false_2,
            false_3,
            false_4,
            end,
        }
        enum integerliteralState
        {
            start,
            decimal_integer_literal,
            hexadecimal_integer_literal,
            end,
        }
        enum decimalintegerliteralState
        {
            start,
            class_digit,
            integer_type_suffix,
            end,
        }
        enum decimaldigitsState
        {
            start,
            decimal_digit,
            end,
        }
        enum decimaldigitState
        {
            start,
            class_digit,
            end,
        }
        enum integertypesuffixState
        {
            start,
            letter_u_0,
            letter_l_0,
            letter_l_1,
            letter_u_1,
            end,
        }
        enum hexadecimalintegerliteralState
        {
            start,
            _0x_0,
            _0x_1,
            hex_digits,
            integer_type_suffix,
            end,
        }
        enum hexdigitsState
        {
            start,
            hex_digit,
            end,
        }
        enum hexdigitState
        {
            start,
            dabcdef,
            end,
        }
        enum realliteralState
        {
            start,
            decimal_digits_0,
            period_0,
            decimal_digits_1,
            decimal_digits_2,
            period_1,
            decimal_digits_3,
            exponent_part_0,
            real_type_suffix_0,
            decimal_digits_4,
            exponent_part_1,
            real_type_suffix_1,
            end,
            real_type_suffix_2,
            exponent_part_2,
            real_type_suffix_3,
        }
        enum exponentpartState
        {
            start,
            letter_e,
            plus_hyphen,
            class_digit,
            end,
        }
        enum realtypesuffixState
        {
            start,
            fdm,
            end,
        }
        enum characterliteralState
        {
            start,
            quote_0,
            character,
            quote_1,
            end,
        }
        enum characterState
        {
            start,
            single_character,
            simple_escape_sequence,
            hexadecimal_escape_sequence,
            unicode_escape_sequence,
            end,
        }
        enum singlecharacterState
        {
            start,
            nrx0085x2028x2029,
            end,
        }
        enum simpleescapesequenceState
        {
            start,
            bslash,
            _0abfnrtv,
            end,
        }
        enum hexadecimalescapesequenceState
        {
            start,
            bslash_letter_x_0,
            bslash_letter_x_1,
            hex_digit_0,
            hex_digit_1,
            hex_digit_2,
            hex_digit_3,
            end,
        }
        enum stringliteralState
        {
            start,
            regular_string_literal,
            verbatim_string_literal,
            end,
        }
        enum regularstringliteralState
        {
            start,
            dquote_0,
            regular_string_literal_character,
            dquote_1,
            end,
        }
        enum regularstringliteralcharacterState
        {
            start,
            single_regular_string_literal_character,
            simple_escape_sequence,
            hexadecimal_escape_sequence,
            unicode_escape_sequence,
            end,
        }
        enum singleregularstringliteralcharacterState
        {
            start,
            nrx0085x2028x2029,
            end,
        }
        enum verbatimstringliteralState
        {
            start,
            at_dquote_0,
            at_dquote_1,
            verbatim_string_literal_character,
            dquote,
            end,
        }
        enum verbatimstringliteralcharacterState
        {
            start,
            chevron_dquote,
            dquote_dquote_0,
            dquote_dquote_1,
            end,
        }
        enum nullliteralState
        {
            start,
            null_0,
            null_1,
            null_2,
            null_3,
            end,
        }
        enum operatororpunctuatorState
        {
            start,
            obrace,
            cbrace,
            obracket,
            cbracket,
            oparen,
            cparen,
            period,
            comma,
            colon,
            semi,
            plus,
            hyphen,
            star,
            slash,
            percent,
            amp,
            pipe,
            chevron,
            exclamation,
            tilde,
            equal,
            less,
            greater,
            question,
            question_question_0,
            question_question_1,
            colon_colon_0,
            colon_colon_1,
            plus_plus_0,
            plus_plus_1,
            hyphen_hyphen_0,
            hyphen_hyphen_1,
            amp_amp_0,
            amp_amp_1,
            pipe_pipe_0,
            pipe_pipe_1,
            hyphen_greater_0,
            hyphen_greater_1,
            equal_equal_0,
            equal_equal_1,
            exclamation_equal_0,
            exclamation_equal_1,
            less_equal_0,
            less_equal_1,
            greater_equal_0,
            greater_equal_1,
            plus_equal_0,
            plus_equal_1,
            hyphen_equal_0,
            hyphen_equal_1,
            star_equal_0,
            star_equal_1,
            slash_equal_0,
            slash_equal_1,
            percent_equal_0,
            percent_equal_1,
            amp_equal_0,
            amp_equal_1,
            pipe_equal_0,
            pipe_equal_1,
            chevron_equal_0,
            chevron_equal_1,
            less_less_0,
            less_less_1,
            less_less_equal_0,
            less_less_equal_1,
            less_less_equal_2,
            end,
        }
        enum rightshiftState
        {
            start,
            greater_0,
            greater_1,
            end,
        }
        enum rightshiftassignmentState
        {
            start,
            greater,
            greater_equal_0,
            greater_equal_1,
            end,
        }
        enum ppdirectiveState
        {
            start,
            pp_declaration,
            pp_conditional,
            pp_line,
            pp_diagnostic,
            pp_region,
            pp_pragma,
            end,
        }
        enum conditionalsymbolState
        {
            start,
            identifier,
            end,
        }
        enum ppexpressionState
        {
            start,
            pp_sub_expression_0,
            pp_operator,
            whitespace_0,
            end,
            whitespace_1,
            pp_sub_expression_1,
        }
        enum ppsubexpressionState
        {
            start,
            pp_unary_expression,
            pp_primary_expression,
            end,
        }
        enum ppoperatorState
        {
            start,
            amp_amp_0,
            amp_amp_1,
            pipe_pipe_0,
            pipe_pipe_1,
            equal_equal_0,
            equal_equal_1,
            exclamation_equal_0,
            exclamation_equal_1,
            end,
        }
        enum ppunaryexpressionState
        {
            start,
            exclamation,
            whitespace,
            pp_sub_expression,
            end,
        }
        enum ppprimaryexpressionState
        {
            start,
            true_0,
            true_1,
            true_2,
            true_3,
            false_0,
            false_1,
            false_2,
            false_3,
            false_4,
            conditional_symbol,
            oparen,
            end,
            whitespace_0,
            pp_expression,
            whitespace_1,
            cparen,
        }
        enum ppdeclarationState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            define_0,
            define_1,
            define_2,
            define_3,
            define_4,
            define_5,
            undef_0,
            undef_1,
            undef_2,
            undef_3,
            undef_4,
            whitespace_2,
            conditional_symbol,
            pp_new_line,
            end,
        }
        enum ppnewlineState
        {
            start,
            single_line_comment,
            new_line,
            whitespace,
            end,
        }
        enum ppconditionalState
        {
            start,
            pp_if_section,
            pp_elif_sections,
            pp_else_section,
            pp_endif,
            end,
        }
        enum ppifsectionState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            if_0,
            if_1,
            whitespace_2,
            pp_expression,
            pp_new_line,
            conditional_section,
            end,
        }
        enum ppelifsectionsState
        {
            start,
            pp_elif_section,
            end,
        }
        enum ppelifsectionState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            elif_0,
            elif_1,
            elif_2,
            elif_3,
            whitespace_2,
            pp_expression,
            pp_new_line,
            conditional_section,
            end,
        }
        enum ppelsesectionState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            else_0,
            else_1,
            else_2,
            else_3,
            pp_new_line,
            conditional_section,
            end,
        }
        enum ppendifState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            endif_0,
            endif_1,
            endif_2,
            endif_3,
            endif_4,
            pp_new_line,
            end,
        }
        enum conditionalsectionState
        {
            start,
            input_section,
            skipped_section,
            end,
        }
        enum skippedsectionState
        {
            start,
            skipped_section_part,
            end,
        }
        enum skippedsectionpartState
        {
            start,
            skipped_characters,
            new_line,
            whitespace,
            pp_directive,
            end,
        }
        enum skippedcharactersState
        {
            start,
            chevron_hash,
            input_characters,
            end,
        }
        enum pplineState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            line_0,
            line_1,
            line_2,
            line_3,
            whitespace_2,
            line_indicator,
            pp_new_line,
            end,
        }
        enum lineindicatorState
        {
            start,
            class_digit_0,
            class_digit_1,
            identifier_or_keyword,
            whitespace,
            end,
            file_name,
        }
        enum filenameState
        {
            start,
            dquote_0,
            file_name_character,
            dquote_1,
            end,
        }
        enum filenamecharacterState
        {
            start,
            nrx0085x2028x2029,
            end,
        }
        enum ppdiagnosticState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            error_0,
            error_1,
            error_2,
            error_3,
            error_4,
            warning_0,
            warning_1,
            warning_2,
            warning_3,
            warning_4,
            warning_5,
            warning_6,
            pp_message,
            end,
        }
        enum ppmessageState
        {
            start,
            new_line_0,
            whitespace,
            end,
            input_characters,
            new_line_1,
        }
        enum ppregionState
        {
            start,
            pp_start_region,
            conditional_section,
            pp_end_region,
            end,
        }
        enum ppstartregionState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            region_0,
            region_1,
            region_2,
            region_3,
            region_4,
            region_5,
            pp_message,
            end,
        }
        enum ppendregionState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            endregion_0,
            endregion_1,
            endregion_2,
            endregion_3,
            endregion_4,
            endregion_5,
            endregion_6,
            endregion_7,
            endregion_8,
            pp_message,
            end,
        }
        enum pppragmaState
        {
            start,
            hash,
            whitespace_0,
            whitespace_1,
            pragma_0,
            pragma_1,
            pragma_2,
            pragma_3,
            pragma_4,
            pragma_5,
            pp_pragma_text,
            end,
        }
        enum pppragmatextState
        {
            start,
            new_line,
            whitespace,
            end,
            input_characters,
        }
        public MetaphysicsIndustries.Giza.ParseSpan Getinput(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.ParseSpan span = Getinput(input, ref i);

            if (span != null)
            {
                span.Tag = "input";
            }
            return span;
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getliteral(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.ParseSpan span = Getliteral(input, ref i);

            if (span != null)
            {
                span.Tag = "literal";
            }
            return span;
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getright_shift(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.ParseSpan span = Getright_shift(input, ref i);

            if (span != null)
            {
                span.Tag = "right-shift";
            }
            return span;
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getright_shift_assignment(string input)
        {
            int i = 0;
            MetaphysicsIndustries.Giza.ParseSpan span = Getright_shift_assignment(input, ref i);

            if (span != null)
            {
                span.Tag = "right-shift-assignment";
            }
            return span;
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input", (int)(inputState.start), (int)(inputState.end), this.GetValidNextStates_input, this.GetStateTag_input, this.GetSubSpan_input);
        }

        public Int32[] GetValidNextStates_input(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputState)(currentState))
            {
                case inputState.start:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?' || ch == '#')
                    {
                        validNextStates.Add((int)(inputState.input_section));
                    }
                    break;

                case inputState.input_section:
                    linksToEnd = true;
                    break;

                case inputState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input(string input, ref int i, int nextState)
        {
            switch ((inputState)(nextState))
            {
                case inputState.input_section:
                    return Getinput_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input(int state)
        {
            switch ((inputState)(state))
            {
                case inputState.start:
                    return "start";

                case inputState.input_section:
                    return "input-section";

                case inputState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input-section", (int)(inputsectionState.start), (int)(inputsectionState.end), this.GetValidNextStates_input_section, this.GetStateTag_input_section, this.GetSubSpan_input_section);
        }

        public Int32[] GetValidNextStates_input_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputsectionState)(currentState))
            {
                case inputsectionState.start:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?' || ch == '#')
                    {
                        validNextStates.Add((int)(inputsectionState.input_section_part));
                    }
                    break;

                case inputsectionState.input_section_part:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?' || ch == '#')
                    {
                        validNextStates.Add((int)(inputsectionState.input_section_part));
                    }
                    linksToEnd = true;
                    break;

                case inputsectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input_section(string input, ref int i, int nextState)
        {
            switch ((inputsectionState)(nextState))
            {
                case inputsectionState.input_section_part:
                    return Getinput_section_part(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input_section(int state)
        {
            switch ((inputsectionState)(state))
            {
                case inputsectionState.start:
                    return "start";

                case inputsectionState.input_section_part:
                    return "input-section-part";

                case inputsectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput_section_part(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input-section-part", (int)(inputsectionpartState.start), (int)(inputsectionpartState.end), this.GetValidNextStates_input_section_part, this.GetStateTag_input_section_part, this.GetSubSpan_input_section_part);
        }

        public Int32[] GetValidNextStates_input_section_part(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputsectionpartState)(currentState))
            {
                case inputsectionpartState.start:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(inputsectionpartState.new_line));
                    }
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?')
                    {
                        validNextStates.Add((int)(inputsectionpartState.input_elements));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(inputsectionpartState.pp_directive));
                    }
                    break;

                case inputsectionpartState.new_line:
                    linksToEnd = true;
                    break;

                case inputsectionpartState.input_elements:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(inputsectionpartState.new_line));
                    }
                    break;

                case inputsectionpartState.pp_directive:
                    linksToEnd = true;
                    break;

                case inputsectionpartState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input_section_part(string input, ref int i, int nextState)
        {
            switch ((inputsectionpartState)(nextState))
            {
                case inputsectionpartState.new_line:
                    return Getnew_line(input, ref i);

                case inputsectionpartState.input_elements:
                    return Getinput_elements(input, ref i);

                case inputsectionpartState.pp_directive:
                    return Getpp_directive(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input_section_part(int state)
        {
            switch ((inputsectionpartState)(state))
            {
                case inputsectionpartState.start:
                    return "start";

                case inputsectionpartState.new_line:
                    return "new-line";

                case inputsectionpartState.input_elements:
                    return "input-elements";

                case inputsectionpartState.pp_directive:
                    return "pp-directive";

                case inputsectionpartState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput_elements(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input-elements", (int)(inputelementsState.start), (int)(inputelementsState.end), this.GetValidNextStates_input_elements, this.GetStateTag_input_elements, this.GetSubSpan_input_elements);
        }

        public Int32[] GetValidNextStates_input_elements(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputelementsState)(currentState))
            {
                case inputelementsState.start:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?')
                    {
                        validNextStates.Add((int)(inputelementsState.input_element));
                    }
                    break;

                case inputelementsState.input_element:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?')
                    {
                        validNextStates.Add((int)(inputelementsState.input_element));
                    }
                    linksToEnd = true;
                    break;

                case inputelementsState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input_elements(string input, ref int i, int nextState)
        {
            switch ((inputelementsState)(nextState))
            {
                case inputelementsState.input_element:
                    return Getinput_element(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input_elements(int state)
        {
            switch ((inputelementsState)(state))
            {
                case inputelementsState.start:
                    return "start";

                case inputelementsState.input_element:
                    return "input-element";

                case inputelementsState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput_element(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input-element", (int)(inputelementState.start), (int)(inputelementState.end), this.GetValidNextStates_input_element, this.GetStateTag_input_element, this.GetSubSpan_input_element);
        }

        public Int32[] GetValidNextStates_input_element(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputelementState)(currentState))
            {
                case inputelementState.start:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(inputelementState.whitespace));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(inputelementState.comment));
                    }
                    if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?')
                    {
                        validNextStates.Add((int)(inputelementState.token));
                    }
                    break;

                case inputelementState.whitespace:
                    linksToEnd = true;
                    break;

                case inputelementState.comment:
                    linksToEnd = true;
                    break;

                case inputelementState.token:
                    linksToEnd = true;
                    break;

                case inputelementState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input_element(string input, ref int i, int nextState)
        {
            switch ((inputelementState)(nextState))
            {
                case inputelementState.whitespace:
                    return Getwhitespace(input, ref i);

                case inputelementState.comment:
                    return Getcomment(input, ref i);

                case inputelementState.token:
                    return Gettoken(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input_element(int state)
        {
            switch ((inputelementState)(state))
            {
                case inputelementState.start:
                    return "start";

                case inputelementState.whitespace:
                    return "whitespace";

                case inputelementState.comment:
                    return "comment";

                case inputelementState.token:
                    return "token";

                case inputelementState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getnew_line(string input, ref int i)
        {
            return GetItem(input, ref i, true, "new-line", (int)(newlineState.start), (int)(newlineState.end), this.GetValidNextStates_new_line, this.GetStateTag_new_line, this.GetSubSpan_new_line);
        }

        public Int32[] GetValidNextStates_new_line(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((newlineState)(currentState))
            {
                case newlineState.start:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_0));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x000d_0));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x000a_0));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x2085_0));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x2028_0));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x2029_0));
                    }
                    break;

                case newlineState.x000dx000a_0:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_1));
                    }
                    break;

                case newlineState.x000dx000a_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_2));
                    }
                    break;

                case newlineState.x000dx000a_2:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_3));
                    }
                    break;

                case newlineState.x000dx000a_3:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_4));
                    }
                    break;

                case newlineState.x000dx000a_4:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_5));
                    }
                    break;

                case newlineState.x000dx000a_5:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_6));
                    }
                    break;

                case newlineState.x000dx000a_6:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_7));
                    }
                    break;

                case newlineState.x000dx000a_7:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_8));
                    }
                    break;

                case newlineState.x000dx000a_8:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(newlineState.x000dx000a_9));
                    }
                    break;

                case newlineState.x000dx000a_9:
                    linksToEnd = true;
                    break;

                case newlineState.x000d_0:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000d_1));
                    }
                    break;

                case newlineState.x000d_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000d_2));
                    }
                    break;

                case newlineState.x000d_2:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000d_3));
                    }
                    break;

                case newlineState.x000d_3:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(newlineState.x000d_4));
                    }
                    break;

                case newlineState.x000d_4:
                    linksToEnd = true;
                    break;

                case newlineState.x000a_0:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000a_1));
                    }
                    break;

                case newlineState.x000a_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000a_2));
                    }
                    break;

                case newlineState.x000a_2:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x000a_3));
                    }
                    break;

                case newlineState.x000a_3:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(newlineState.x000a_4));
                    }
                    break;

                case newlineState.x000a_4:
                    linksToEnd = true;
                    break;

                case newlineState.x2085_0:
                    if (ch == '2')
                    {
                        validNextStates.Add((int)(newlineState.x2085_1));
                    }
                    break;

                case newlineState.x2085_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x2085_2));
                    }
                    break;

                case newlineState.x2085_2:
                    if (ch == '8')
                    {
                        validNextStates.Add((int)(newlineState.x2085_3));
                    }
                    break;

                case newlineState.x2085_3:
                    if (ch == '5')
                    {
                        validNextStates.Add((int)(newlineState.x2085_4));
                    }
                    break;

                case newlineState.x2085_4:
                    linksToEnd = true;
                    break;

                case newlineState.x2028_0:
                    if (ch == '2')
                    {
                        validNextStates.Add((int)(newlineState.x2028_1));
                    }
                    break;

                case newlineState.x2028_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x2028_2));
                    }
                    break;

                case newlineState.x2028_2:
                    if (ch == '2')
                    {
                        validNextStates.Add((int)(newlineState.x2028_3));
                    }
                    break;

                case newlineState.x2028_3:
                    if (ch == '8')
                    {
                        validNextStates.Add((int)(newlineState.x2028_4));
                    }
                    break;

                case newlineState.x2028_4:
                    linksToEnd = true;
                    break;

                case newlineState.x2029_0:
                    if (ch == '2')
                    {
                        validNextStates.Add((int)(newlineState.x2029_1));
                    }
                    break;

                case newlineState.x2029_1:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(newlineState.x2029_2));
                    }
                    break;

                case newlineState.x2029_2:
                    if (ch == '2')
                    {
                        validNextStates.Add((int)(newlineState.x2029_3));
                    }
                    break;

                case newlineState.x2029_3:
                    if (ch == '9')
                    {
                        validNextStates.Add((int)(newlineState.x2029_4));
                    }
                    break;

                case newlineState.x2029_4:
                    linksToEnd = true;
                    break;

                case newlineState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_new_line(string input, ref int i, int nextState)
        {
            switch ((newlineState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_new_line(int state)
        {
            switch ((newlineState)(state))
            {
                case newlineState.start:
                    return "start";

                case newlineState.x000dx000a_0:
                    return "x000dx000a";

                case newlineState.x000dx000a_1:
                    return "x000dx000a";

                case newlineState.x000dx000a_2:
                    return "x000dx000a";

                case newlineState.x000dx000a_3:
                    return "x000dx000a";

                case newlineState.x000dx000a_4:
                    return "x000dx000a";

                case newlineState.x000dx000a_5:
                    return "x000dx000a";

                case newlineState.x000dx000a_6:
                    return "x000dx000a";

                case newlineState.x000dx000a_7:
                    return "x000dx000a";

                case newlineState.x000dx000a_8:
                    return "x000dx000a";

                case newlineState.x000dx000a_9:
                    return "x000dx000a";

                case newlineState.x000d_0:
                    return "x000d";

                case newlineState.x000d_1:
                    return "x000d";

                case newlineState.x000d_2:
                    return "x000d";

                case newlineState.x000d_3:
                    return "x000d";

                case newlineState.x000d_4:
                    return "x000d";

                case newlineState.x000a_0:
                    return "x000a";

                case newlineState.x000a_1:
                    return "x000a";

                case newlineState.x000a_2:
                    return "x000a";

                case newlineState.x000a_3:
                    return "x000a";

                case newlineState.x000a_4:
                    return "x000a";

                case newlineState.x2085_0:
                    return "x2085";

                case newlineState.x2085_1:
                    return "x2085";

                case newlineState.x2085_2:
                    return "x2085";

                case newlineState.x2085_3:
                    return "x2085";

                case newlineState.x2085_4:
                    return "x2085";

                case newlineState.x2028_0:
                    return "x2028";

                case newlineState.x2028_1:
                    return "x2028";

                case newlineState.x2028_2:
                    return "x2028";

                case newlineState.x2028_3:
                    return "x2028";

                case newlineState.x2028_4:
                    return "x2028";

                case newlineState.x2029_0:
                    return "x2029";

                case newlineState.x2029_1:
                    return "x2029";

                case newlineState.x2029_2:
                    return "x2029";

                case newlineState.x2029_3:
                    return "x2029";

                case newlineState.x2029_4:
                    return "x2029";

                case newlineState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getwhitespace(string input, ref int i)
        {
            return GetItem(input, ref i, true, "whitespace", (int)(whitespaceState.start), (int)(whitespaceState.end), this.GetValidNextStates_whitespace, this.GetStateTag_whitespace, this.GetSubSpan_whitespace);
        }

        public Int32[] GetValidNextStates_whitespace(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((whitespaceState)(currentState))
            {
                case whitespaceState.start:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(whitespaceState.tx0009x0008x000c));
                    }
                    break;

                case whitespaceState.tx0009x0008x000c:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(whitespaceState.tx0009x0008x000c));
                    }
                    linksToEnd = true;
                    break;

                case whitespaceState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_whitespace(string input, ref int i, int nextState)
        {
            switch ((whitespaceState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_whitespace(int state)
        {
            switch ((whitespaceState)(state))
            {
                case whitespaceState.start:
                    return "start";

                case whitespaceState.tx0009x0008x000c:
                    return " \\t\\x0009\\x0008\\x000c";

                case whitespaceState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getcomment(string input, ref int i)
        {
            return GetItem(input, ref i, true, "comment", (int)(commentState.start), (int)(commentState.end), this.GetValidNextStates_comment, this.GetStateTag_comment, this.GetSubSpan_comment);
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
                        validNextStates.Add((int)(commentState.single_line_comment));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(commentState.delimited_comment));
                    }
                    break;

                case commentState.single_line_comment:
                    linksToEnd = true;
                    break;

                case commentState.delimited_comment:
                    linksToEnd = true;
                    break;

                case commentState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_comment(string input, ref int i, int nextState)
        {
            switch ((commentState)(nextState))
            {
                case commentState.single_line_comment:
                    return Getsingle_line_comment(input, ref i);

                case commentState.delimited_comment:
                    return Getdelimited_comment(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_comment(int state)
        {
            switch ((commentState)(state))
            {
                case commentState.start:
                    return "start";

                case commentState.single_line_comment:
                    return "single-line-comment";

                case commentState.delimited_comment:
                    return "delimited-comment";

                case commentState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinput_characters(string input, ref int i)
        {
            return GetItem(input, ref i, true, "input-characters", (int)(inputcharactersState.start), (int)(inputcharactersState.end), this.GetValidNextStates_input_characters, this.GetStateTag_input_characters, this.GetSubSpan_input_characters);
        }

        public Int32[] GetValidNextStates_input_characters(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((inputcharactersState)(currentState))
            {
                case inputcharactersState.start:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(inputcharactersState.nrx0085x2028x2029));
                    }
                    break;

                case inputcharactersState.nrx0085x2028x2029:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(inputcharactersState.nrx0085x2028x2029));
                    }
                    linksToEnd = true;
                    break;

                case inputcharactersState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_input_characters(string input, ref int i, int nextState)
        {
            switch ((inputcharactersState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_input_characters(int state)
        {
            switch ((inputcharactersState)(state))
            {
                case inputcharactersState.start:
                    return "start";

                case inputcharactersState.nrx0085x2028x2029:
                    return "^\\n\\r\\x0085\\x2028\\x2029";

                case inputcharactersState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getsingle_line_comment(string input, ref int i)
        {
            return GetItem(input, ref i, true, "single-line-comment", (int)(singlelinecommentState.start), (int)(singlelinecommentState.end), this.GetValidNextStates_single_line_comment, this.GetStateTag_single_line_comment, this.GetSubSpan_single_line_comment);
        }

        public Int32[] GetValidNextStates_single_line_comment(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((singlelinecommentState)(currentState))
            {
                case singlelinecommentState.start:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(singlelinecommentState.slash_slash_0));
                    }
                    break;

                case singlelinecommentState.slash_slash_0:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(singlelinecommentState.slash_slash_1));
                    }
                    break;

                case singlelinecommentState.slash_slash_1:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(singlelinecommentState.input_characters));
                    }
                    linksToEnd = true;
                    break;

                case singlelinecommentState.input_characters:
                    linksToEnd = true;
                    break;

                case singlelinecommentState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_single_line_comment(string input, ref int i, int nextState)
        {
            switch ((singlelinecommentState)(nextState))
            {
                case singlelinecommentState.input_characters:
                    return Getinput_characters(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_single_line_comment(int state)
        {
            switch ((singlelinecommentState)(state))
            {
                case singlelinecommentState.start:
                    return "start";

                case singlelinecommentState.slash_slash_0:
                    return "//";

                case singlelinecommentState.slash_slash_1:
                    return "//";

                case singlelinecommentState.input_characters:
                    return "input-characters";

                case singlelinecommentState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getdelimited_comment(string input, ref int i)
        {
            return GetItem(input, ref i, true, "delimited-comment", (int)(delimitedcommentState.start), (int)(delimitedcommentState.end), this.GetValidNextStates_delimited_comment, this.GetStateTag_delimited_comment, this.GetSubSpan_delimited_comment);
        }

        public Int32[] GetValidNextStates_delimited_comment(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((delimitedcommentState)(currentState))
            {
                case delimitedcommentState.start:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(delimitedcommentState.slash_star_0));
                    }
                    break;

                case delimitedcommentState.slash_star_0:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.slash_star_1));
                    }
                    break;

                case delimitedcommentState.slash_star_1:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(delimitedcommentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_1));
                    }
                    break;

                case delimitedcommentState.chevron_star:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(delimitedcommentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_1));
                    }
                    break;

                case delimitedcommentState.star_0:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_0));
                    }
                    if (!(ch == '/'))
                    {
                        validNextStates.Add((int)(delimitedcommentState.chevron_slash));
                    }
                    break;

                case delimitedcommentState.star_1:
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_1));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(delimitedcommentState.slash));
                    }
                    break;

                case delimitedcommentState.chevron_slash:
                    if (!(ch == '*'))
                    {
                        validNextStates.Add((int)(delimitedcommentState.chevron_star));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(delimitedcommentState.star_1));
                    }
                    break;

                case delimitedcommentState.slash:
                    linksToEnd = true;
                    break;

                case delimitedcommentState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_delimited_comment(string input, ref int i, int nextState)
        {
            switch ((delimitedcommentState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_delimited_comment(int state)
        {
            switch ((delimitedcommentState)(state))
            {
                case delimitedcommentState.start:
                    return "start";

                case delimitedcommentState.slash_star_0:
                    return "/*";

                case delimitedcommentState.slash_star_1:
                    return "/*";

                case delimitedcommentState.chevron_star:
                    return "^*";

                case delimitedcommentState.star_0:
                    return "*";

                case delimitedcommentState.star_1:
                    return "*";

                case delimitedcommentState.chevron_slash:
                    return "^/";

                case delimitedcommentState.slash:
                    return "/";

                case delimitedcommentState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Gettoken(string input, ref int i)
        {
            return GetItem(input, ref i, true, "token", (int)(tokenState.start), (int)(tokenState.end), this.GetValidNextStates_token, this.GetStateTag_token, this.GetSubSpan_token);
        }

        public Int32[] GetValidNextStates_token(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((tokenState)(currentState))
            {
                case tokenState.start:
                    if (char.IsLetter(ch) || ch == '_' || ch == '@')
                    {
                        validNextStates.Add((int)(tokenState.identifier));
                    }
                    if (ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'g' || ch == 'i' || ch == 'l' || ch == 'n' || ch == 'o' || ch == 'p' || ch == 'r' || ch == 's' || ch == 't' || ch == 'u' || ch == 'v' || ch == 'w')
                    {
                        validNextStates.Add((int)(tokenState.keyword));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(tokenState.integer_literal));
                    }
                    if (char.IsDigit(ch) || ch == '.')
                    {
                        validNextStates.Add((int)(tokenState.real_literal));
                    }
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(tokenState.character_literal));
                    }
                    if (ch == '"' || ch == '@')
                    {
                        validNextStates.Add((int)(tokenState.string_literal));
                    }
                    if (ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == '.' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?')
                    {
                        validNextStates.Add((int)(tokenState.operator_or_punctuator));
                    }
                    break;

                case tokenState.identifier:
                    linksToEnd = true;
                    break;

                case tokenState.keyword:
                    linksToEnd = true;
                    break;

                case tokenState.integer_literal:
                    linksToEnd = true;
                    break;

                case tokenState.real_literal:
                    linksToEnd = true;
                    break;

                case tokenState.character_literal:
                    linksToEnd = true;
                    break;

                case tokenState.string_literal:
                    linksToEnd = true;
                    break;

                case tokenState.operator_or_punctuator:
                    linksToEnd = true;
                    break;

                case tokenState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_token(string input, ref int i, int nextState)
        {
            switch ((tokenState)(nextState))
            {
                case tokenState.identifier:
                    return Getidentifier(input, ref i);

                case tokenState.keyword:
                    return Getkeyword(input, ref i);

                case tokenState.integer_literal:
                    return Getinteger_literal(input, ref i);

                case tokenState.real_literal:
                    return Getreal_literal(input, ref i);

                case tokenState.character_literal:
                    return Getcharacter_literal(input, ref i);

                case tokenState.string_literal:
                    return Getstring_literal(input, ref i);

                case tokenState.operator_or_punctuator:
                    return Getoperator_or_punctuator(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_token(int state)
        {
            switch ((tokenState)(state))
            {
                case tokenState.start:
                    return "start";

                case tokenState.identifier:
                    return "identifier";

                case tokenState.keyword:
                    return "keyword";

                case tokenState.integer_literal:
                    return "integer-literal";

                case tokenState.real_literal:
                    return "real-literal";

                case tokenState.character_literal:
                    return "character-literal";

                case tokenState.string_literal:
                    return "string-literal";

                case tokenState.operator_or_punctuator:
                    return "operator-or-punctuator";

                case tokenState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getunicode_escape_sequence(string input, ref int i)
        {
            return GetItem(input, ref i, true, "unicode-escape-sequence", (int)(unicodeescapesequenceState.start), (int)(unicodeescapesequenceState.end), this.GetValidNextStates_unicode_escape_sequence, this.GetStateTag_unicode_escape_sequence, this.GetSubSpan_unicode_escape_sequence);
        }

        public Int32[] GetValidNextStates_unicode_escape_sequence(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((unicodeescapesequenceState)(currentState))
            {
                case unicodeescapesequenceState.start:
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.bslash_letter_u_0));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.bslash_letter_U_0));
                    }
                    break;

                case unicodeescapesequenceState.bslash_letter_u_0:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.bslash_letter_u_1));
                    }
                    break;

                case unicodeescapesequenceState.bslash_letter_u_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_0));
                    }
                    break;

                case unicodeescapesequenceState.bslash_letter_U_0:
                    if (ch == 'U')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.bslash_letter_U_1));
                    }
                    break;

                case unicodeescapesequenceState.bslash_letter_U_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_1));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_0:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_2));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_3));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_2:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_4));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_3:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_5));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_4:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_6));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_5:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_7));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_6:
                    linksToEnd = true;
                    break;

                case unicodeescapesequenceState.hex_digit_7:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_8));
                    }
                    break;

                case unicodeescapesequenceState.end:
                    break;

                case unicodeescapesequenceState.hex_digit_8:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_9));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_9:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_10));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_10:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(unicodeescapesequenceState.hex_digit_11));
                    }
                    break;

                case unicodeescapesequenceState.hex_digit_11:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_unicode_escape_sequence(string input, ref int i, int nextState)
        {
            switch ((unicodeescapesequenceState)(nextState))
            {
                case unicodeescapesequenceState.hex_digit_0:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_1:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_2:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_3:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_4:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_5:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_6:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_7:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_8:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_9:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_10:
                    return Gethex_digit(input, ref i);

                case unicodeescapesequenceState.hex_digit_11:
                    return Gethex_digit(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_unicode_escape_sequence(int state)
        {
            switch ((unicodeescapesequenceState)(state))
            {
                case unicodeescapesequenceState.start:
                    return "start";

                case unicodeescapesequenceState.bslash_letter_u_0:
                    return "\\u";

                case unicodeescapesequenceState.bslash_letter_u_1:
                    return "\\u";

                case unicodeescapesequenceState.bslash_letter_U_0:
                    return "\\U";

                case unicodeescapesequenceState.bslash_letter_U_1:
                    return "\\U";

                case unicodeescapesequenceState.hex_digit_0:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_1:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_2:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_3:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_4:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_5:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_6:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_7:
                    return "hex-digit";

                case unicodeescapesequenceState.end:
                    return "end";

                case unicodeescapesequenceState.hex_digit_8:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_9:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_10:
                    return "hex-digit";

                case unicodeescapesequenceState.hex_digit_11:
                    return "hex-digit";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getidentifier(string input, ref int i)
        {
            return GetItem(input, ref i, true, "identifier", (int)(identifierState.start), (int)(identifierState.end), this.GetValidNextStates_identifier, this.GetStateTag_identifier, this.GetSubSpan_identifier);
        }

        public Int32[] GetValidNextStates_identifier(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((identifierState)(currentState))
            {
                case identifierState.start:
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(identifierState.available_identifier));
                    }
                    if (ch == '@')
                    {
                        validNextStates.Add((int)(identifierState.at));
                    }
                    break;

                case identifierState.available_identifier:
                    linksToEnd = true;
                    break;

                case identifierState.at:
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(identifierState.identifier_or_keyword));
                    }
                    break;

                case identifierState.end:
                    break;

                case identifierState.identifier_or_keyword:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_identifier(string input, ref int i, int nextState)
        {
            switch ((identifierState)(nextState))
            {
                case identifierState.available_identifier:
                    return Getavailable_identifier(input, ref i);

                case identifierState.identifier_or_keyword:
                    return Getidentifier_or_keyword(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_identifier(int state)
        {
            switch ((identifierState)(state))
            {
                case identifierState.start:
                    return "start";

                case identifierState.available_identifier:
                    return "available-identifier";

                case identifierState.at:
                    return "@";

                case identifierState.end:
                    return "end";

                case identifierState.identifier_or_keyword:
                    return "identifier-or-keyword";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getavailable_identifier(string input, ref int i)
        {
            return GetItem(input, ref i, true, "available-identifier", (int)(availableidentifierState.start), (int)(availableidentifierState.end), this.GetValidNextStates_available_identifier, this.GetStateTag_available_identifier, this.GetSubSpan_available_identifier);
        }

        public Int32[] GetValidNextStates_available_identifier(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((availableidentifierState)(currentState))
            {
                case availableidentifierState.start:
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(availableidentifierState.identifier_or_keyword));
                    }
                    break;

                case availableidentifierState.identifier_or_keyword:
                    linksToEnd = true;
                    break;

                case availableidentifierState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_available_identifier(string input, ref int i, int nextState)
        {
            switch ((availableidentifierState)(nextState))
            {
                case availableidentifierState.identifier_or_keyword:
                    return Getidentifier_or_keyword(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_available_identifier(int state)
        {
            switch ((availableidentifierState)(state))
            {
                case availableidentifierState.start:
                    return "start";

                case availableidentifierState.identifier_or_keyword:
                    return "identifier-or-keyword";

                case availableidentifierState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getidentifier_or_keyword(string input, ref int i)
        {
            return GetItem(input, ref i, true, "identifier-or-keyword", (int)(identifierorkeywordState.start), (int)(identifierorkeywordState.end), this.GetValidNextStates_identifier_or_keyword, this.GetStateTag_identifier_or_keyword, this.GetSubSpan_identifier_or_keyword);
        }

        public Int32[] GetValidNextStates_identifier_or_keyword(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((identifierorkeywordState)(currentState))
            {
                case identifierorkeywordState.start:
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(identifierorkeywordState.bslash_letter_l_score));
                    }
                    break;

                case identifierorkeywordState.bslash_letter_l_score:
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(identifierorkeywordState.ld_));
                    }
                    linksToEnd = true;
                    break;

                case identifierorkeywordState.ld_:
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(identifierorkeywordState.ld_));
                    }
                    linksToEnd = true;
                    break;

                case identifierorkeywordState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_identifier_or_keyword(string input, ref int i, int nextState)
        {
            switch ((identifierorkeywordState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_identifier_or_keyword(int state)
        {
            switch ((identifierorkeywordState)(state))
            {
                case identifierorkeywordState.start:
                    return "start";

                case identifierorkeywordState.bslash_letter_l_score:
                    return "\\l_";

                case identifierorkeywordState.ld_:
                    return "\\l\\d_";

                case identifierorkeywordState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getkeyword(string input, ref int i)
        {
            return GetItem(input, ref i, true, "keyword", (int)(keywordState.start), (int)(keywordState.end), this.GetValidNextStates_keyword, this.GetStateTag_keyword, this.GetSubSpan_keyword);
        }

        public Int32[] GetValidNextStates_keyword(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((keywordState)(currentState))
            {
                case keywordState.start:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.abstract_0));
                    }
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.as_0));
                    }
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.base_0));
                    }
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.bool_0));
                    }
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.break_0));
                    }
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.byte_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.case_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.catch_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.char_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.checked_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.class_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.const_0));
                    }
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.continue_0));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.decimal_0));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.default_0));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.delegate_0));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.do_0));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.double_0));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.else_0));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.enum_0));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.event_0));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.explicit_0));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.extern_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.false_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.finally_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.fixed_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.float_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.for_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.foreach_0));
                    }
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.goto_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.if_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.implicit_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.in_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.int_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.interface_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.internal_0));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.is_0));
                    }
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.lock_0));
                    }
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.long_0));
                    }
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.namespace_0));
                    }
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.new_0));
                    }
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.null_0));
                    }
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.object_0));
                    }
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.operator_0));
                    }
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.out_0));
                    }
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.override_0));
                    }
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.params_0));
                    }
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.private_0));
                    }
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.protected_0));
                    }
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.public_0));
                    }
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.readonly_0));
                    }
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.ref_0));
                    }
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.return_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.sbyte_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.sealed_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.short_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.static_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.string_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.struct_0));
                    }
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.switch_0));
                    }
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.this_0));
                    }
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.throw_0));
                    }
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.true_0));
                    }
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.try_0));
                    }
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.typeof_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.uint_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.ulong_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.ushort_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.using_0));
                    }
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.virtual_0));
                    }
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.void_0));
                    }
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.volatile_0));
                    }
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(keywordState.while_0));
                    }
                    break;

                case keywordState.abstract_0:
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.abstract_1));
                    }
                    break;

                case keywordState.abstract_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.abstract_2));
                    }
                    break;

                case keywordState.abstract_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.abstract_3));
                    }
                    break;

                case keywordState.abstract_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.abstract_4));
                    }
                    break;

                case keywordState.abstract_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.abstract_5));
                    }
                    break;

                case keywordState.abstract_5:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.abstract_6));
                    }
                    break;

                case keywordState.abstract_6:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.abstract_7));
                    }
                    break;

                case keywordState.abstract_7:
                    linksToEnd = true;
                    break;

                case keywordState.as_0:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.as_1));
                    }
                    break;

                case keywordState.as_1:
                    linksToEnd = true;
                    break;

                case keywordState.base_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.base_1));
                    }
                    break;

                case keywordState.base_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.base_2));
                    }
                    break;

                case keywordState.base_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.base_3));
                    }
                    break;

                case keywordState.base_3:
                    linksToEnd = true;
                    break;

                case keywordState.bool_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.bool_1));
                    }
                    break;

                case keywordState.bool_1:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.bool_2));
                    }
                    break;

                case keywordState.bool_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.bool_3));
                    }
                    break;

                case keywordState.bool_3:
                    linksToEnd = true;
                    break;

                case keywordState.break_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.break_1));
                    }
                    break;

                case keywordState.break_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.break_2));
                    }
                    break;

                case keywordState.break_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.break_3));
                    }
                    break;

                case keywordState.break_3:
                    if (ch == 'k')
                    {
                        validNextStates.Add((int)(keywordState.break_4));
                    }
                    break;

                case keywordState.break_4:
                    linksToEnd = true;
                    break;

                case keywordState.byte_0:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.byte_1));
                    }
                    break;

                case keywordState.byte_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.byte_2));
                    }
                    break;

                case keywordState.byte_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.byte_3));
                    }
                    break;

                case keywordState.byte_3:
                    linksToEnd = true;
                    break;

                case keywordState.case_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.case_1));
                    }
                    break;

                case keywordState.case_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.case_2));
                    }
                    break;

                case keywordState.case_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.case_3));
                    }
                    break;

                case keywordState.case_3:
                    linksToEnd = true;
                    break;

                case keywordState.catch_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.catch_1));
                    }
                    break;

                case keywordState.catch_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.catch_2));
                    }
                    break;

                case keywordState.catch_2:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.catch_3));
                    }
                    break;

                case keywordState.catch_3:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.catch_4));
                    }
                    break;

                case keywordState.catch_4:
                    linksToEnd = true;
                    break;

                case keywordState.char_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.char_1));
                    }
                    break;

                case keywordState.char_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.char_2));
                    }
                    break;

                case keywordState.char_2:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.char_3));
                    }
                    break;

                case keywordState.char_3:
                    linksToEnd = true;
                    break;

                case keywordState.checked_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.checked_1));
                    }
                    break;

                case keywordState.checked_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.checked_2));
                    }
                    break;

                case keywordState.checked_2:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.checked_3));
                    }
                    break;

                case keywordState.checked_3:
                    if (ch == 'k')
                    {
                        validNextStates.Add((int)(keywordState.checked_4));
                    }
                    break;

                case keywordState.checked_4:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.checked_5));
                    }
                    break;

                case keywordState.checked_5:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.checked_6));
                    }
                    break;

                case keywordState.checked_6:
                    linksToEnd = true;
                    break;

                case keywordState.class_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.class_1));
                    }
                    break;

                case keywordState.class_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.class_2));
                    }
                    break;

                case keywordState.class_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.class_3));
                    }
                    break;

                case keywordState.class_3:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.class_4));
                    }
                    break;

                case keywordState.class_4:
                    linksToEnd = true;
                    break;

                case keywordState.const_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.const_1));
                    }
                    break;

                case keywordState.const_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.const_2));
                    }
                    break;

                case keywordState.const_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.const_3));
                    }
                    break;

                case keywordState.const_3:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.const_4));
                    }
                    break;

                case keywordState.const_4:
                    linksToEnd = true;
                    break;

                case keywordState.continue_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.continue_1));
                    }
                    break;

                case keywordState.continue_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.continue_2));
                    }
                    break;

                case keywordState.continue_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.continue_3));
                    }
                    break;

                case keywordState.continue_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.continue_4));
                    }
                    break;

                case keywordState.continue_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.continue_5));
                    }
                    break;

                case keywordState.continue_5:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.continue_6));
                    }
                    break;

                case keywordState.continue_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.continue_7));
                    }
                    break;

                case keywordState.continue_7:
                    linksToEnd = true;
                    break;

                case keywordState.decimal_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.decimal_1));
                    }
                    break;

                case keywordState.decimal_1:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.decimal_2));
                    }
                    break;

                case keywordState.decimal_2:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.decimal_3));
                    }
                    break;

                case keywordState.decimal_3:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(keywordState.decimal_4));
                    }
                    break;

                case keywordState.decimal_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.decimal_5));
                    }
                    break;

                case keywordState.decimal_5:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.decimal_6));
                    }
                    break;

                case keywordState.decimal_6:
                    linksToEnd = true;
                    break;

                case keywordState.default_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.default_1));
                    }
                    break;

                case keywordState.default_1:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.default_2));
                    }
                    break;

                case keywordState.default_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.default_3));
                    }
                    break;

                case keywordState.default_3:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.default_4));
                    }
                    break;

                case keywordState.default_4:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.default_5));
                    }
                    break;

                case keywordState.default_5:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.default_6));
                    }
                    break;

                case keywordState.default_6:
                    linksToEnd = true;
                    break;

                case keywordState.delegate_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.delegate_1));
                    }
                    break;

                case keywordState.delegate_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.delegate_2));
                    }
                    break;

                case keywordState.delegate_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.delegate_3));
                    }
                    break;

                case keywordState.delegate_3:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.delegate_4));
                    }
                    break;

                case keywordState.delegate_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.delegate_5));
                    }
                    break;

                case keywordState.delegate_5:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.delegate_6));
                    }
                    break;

                case keywordState.delegate_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.delegate_7));
                    }
                    break;

                case keywordState.delegate_7:
                    linksToEnd = true;
                    break;

                case keywordState.do_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.do_1));
                    }
                    break;

                case keywordState.do_1:
                    linksToEnd = true;
                    break;

                case keywordState.double_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.double_1));
                    }
                    break;

                case keywordState.double_1:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.double_2));
                    }
                    break;

                case keywordState.double_2:
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.double_3));
                    }
                    break;

                case keywordState.double_3:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.double_4));
                    }
                    break;

                case keywordState.double_4:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.double_5));
                    }
                    break;

                case keywordState.double_5:
                    linksToEnd = true;
                    break;

                case keywordState.else_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.else_1));
                    }
                    break;

                case keywordState.else_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.else_2));
                    }
                    break;

                case keywordState.else_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.else_3));
                    }
                    break;

                case keywordState.else_3:
                    linksToEnd = true;
                    break;

                case keywordState.enum_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.enum_1));
                    }
                    break;

                case keywordState.enum_1:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.enum_2));
                    }
                    break;

                case keywordState.enum_2:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(keywordState.enum_3));
                    }
                    break;

                case keywordState.enum_3:
                    linksToEnd = true;
                    break;

                case keywordState.event_0:
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.event_1));
                    }
                    break;

                case keywordState.event_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.event_2));
                    }
                    break;

                case keywordState.event_2:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.event_3));
                    }
                    break;

                case keywordState.event_3:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.event_4));
                    }
                    break;

                case keywordState.event_4:
                    linksToEnd = true;
                    break;

                case keywordState.explicit_0:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(keywordState.explicit_1));
                    }
                    break;

                case keywordState.explicit_1:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.explicit_2));
                    }
                    break;

                case keywordState.explicit_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.explicit_3));
                    }
                    break;

                case keywordState.explicit_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.explicit_4));
                    }
                    break;

                case keywordState.explicit_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.explicit_5));
                    }
                    break;

                case keywordState.explicit_5:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.explicit_6));
                    }
                    break;

                case keywordState.explicit_6:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.explicit_7));
                    }
                    break;

                case keywordState.explicit_7:
                    linksToEnd = true;
                    break;

                case keywordState.extern_0:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(keywordState.extern_1));
                    }
                    break;

                case keywordState.extern_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.extern_2));
                    }
                    break;

                case keywordState.extern_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.extern_3));
                    }
                    break;

                case keywordState.extern_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.extern_4));
                    }
                    break;

                case keywordState.extern_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.extern_5));
                    }
                    break;

                case keywordState.extern_5:
                    linksToEnd = true;
                    break;

                case keywordState.false_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.false_1));
                    }
                    break;

                case keywordState.false_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.false_2));
                    }
                    break;

                case keywordState.false_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.false_3));
                    }
                    break;

                case keywordState.false_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.false_4));
                    }
                    break;

                case keywordState.false_4:
                    linksToEnd = true;
                    break;

                case keywordState.finally_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.finally_1));
                    }
                    break;

                case keywordState.finally_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.finally_2));
                    }
                    break;

                case keywordState.finally_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.finally_3));
                    }
                    break;

                case keywordState.finally_3:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.finally_4));
                    }
                    break;

                case keywordState.finally_4:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.finally_5));
                    }
                    break;

                case keywordState.finally_5:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.finally_6));
                    }
                    break;

                case keywordState.finally_6:
                    linksToEnd = true;
                    break;

                case keywordState.fixed_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.fixed_1));
                    }
                    break;

                case keywordState.fixed_1:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(keywordState.fixed_2));
                    }
                    break;

                case keywordState.fixed_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.fixed_3));
                    }
                    break;

                case keywordState.fixed_3:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.fixed_4));
                    }
                    break;

                case keywordState.fixed_4:
                    linksToEnd = true;
                    break;

                case keywordState.float_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.float_1));
                    }
                    break;

                case keywordState.float_1:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.float_2));
                    }
                    break;

                case keywordState.float_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.float_3));
                    }
                    break;

                case keywordState.float_3:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.float_4));
                    }
                    break;

                case keywordState.float_4:
                    linksToEnd = true;
                    break;

                case keywordState.for_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.for_1));
                    }
                    break;

                case keywordState.for_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.for_2));
                    }
                    break;

                case keywordState.for_2:
                    linksToEnd = true;
                    break;

                case keywordState.foreach_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.foreach_1));
                    }
                    break;

                case keywordState.foreach_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.foreach_2));
                    }
                    break;

                case keywordState.foreach_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.foreach_3));
                    }
                    break;

                case keywordState.foreach_3:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.foreach_4));
                    }
                    break;

                case keywordState.foreach_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.foreach_5));
                    }
                    break;

                case keywordState.foreach_5:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.foreach_6));
                    }
                    break;

                case keywordState.foreach_6:
                    linksToEnd = true;
                    break;

                case keywordState.goto_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.goto_1));
                    }
                    break;

                case keywordState.goto_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.goto_2));
                    }
                    break;

                case keywordState.goto_2:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.goto_3));
                    }
                    break;

                case keywordState.goto_3:
                    linksToEnd = true;
                    break;

                case keywordState.if_0:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.if_1));
                    }
                    break;

                case keywordState.if_1:
                    linksToEnd = true;
                    break;

                case keywordState.implicit_0:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(keywordState.implicit_1));
                    }
                    break;

                case keywordState.implicit_1:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.implicit_2));
                    }
                    break;

                case keywordState.implicit_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.implicit_3));
                    }
                    break;

                case keywordState.implicit_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.implicit_4));
                    }
                    break;

                case keywordState.implicit_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.implicit_5));
                    }
                    break;

                case keywordState.implicit_5:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.implicit_6));
                    }
                    break;

                case keywordState.implicit_6:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.implicit_7));
                    }
                    break;

                case keywordState.implicit_7:
                    linksToEnd = true;
                    break;

                case keywordState.in_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.in_1));
                    }
                    break;

                case keywordState.in_1:
                    linksToEnd = true;
                    break;

                case keywordState.int_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.int_1));
                    }
                    break;

                case keywordState.int_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.int_2));
                    }
                    break;

                case keywordState.int_2:
                    linksToEnd = true;
                    break;

                case keywordState.interface_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.interface_1));
                    }
                    break;

                case keywordState.interface_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.interface_2));
                    }
                    break;

                case keywordState.interface_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.interface_3));
                    }
                    break;

                case keywordState.interface_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.interface_4));
                    }
                    break;

                case keywordState.interface_4:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.interface_5));
                    }
                    break;

                case keywordState.interface_5:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.interface_6));
                    }
                    break;

                case keywordState.interface_6:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.interface_7));
                    }
                    break;

                case keywordState.interface_7:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.interface_8));
                    }
                    break;

                case keywordState.interface_8:
                    linksToEnd = true;
                    break;

                case keywordState.internal_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.internal_1));
                    }
                    break;

                case keywordState.internal_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.internal_2));
                    }
                    break;

                case keywordState.internal_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.internal_3));
                    }
                    break;

                case keywordState.internal_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.internal_4));
                    }
                    break;

                case keywordState.internal_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.internal_5));
                    }
                    break;

                case keywordState.internal_5:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.internal_6));
                    }
                    break;

                case keywordState.internal_6:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.internal_7));
                    }
                    break;

                case keywordState.internal_7:
                    linksToEnd = true;
                    break;

                case keywordState.is_0:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.is_1));
                    }
                    break;

                case keywordState.is_1:
                    linksToEnd = true;
                    break;

                case keywordState.lock_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.lock_1));
                    }
                    break;

                case keywordState.lock_1:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.lock_2));
                    }
                    break;

                case keywordState.lock_2:
                    if (ch == 'k')
                    {
                        validNextStates.Add((int)(keywordState.lock_3));
                    }
                    break;

                case keywordState.lock_3:
                    linksToEnd = true;
                    break;

                case keywordState.long_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.long_1));
                    }
                    break;

                case keywordState.long_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.long_2));
                    }
                    break;

                case keywordState.long_2:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.long_3));
                    }
                    break;

                case keywordState.long_3:
                    linksToEnd = true;
                    break;

                case keywordState.namespace_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.namespace_1));
                    }
                    break;

                case keywordState.namespace_1:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(keywordState.namespace_2));
                    }
                    break;

                case keywordState.namespace_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.namespace_3));
                    }
                    break;

                case keywordState.namespace_3:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.namespace_4));
                    }
                    break;

                case keywordState.namespace_4:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.namespace_5));
                    }
                    break;

                case keywordState.namespace_5:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.namespace_6));
                    }
                    break;

                case keywordState.namespace_6:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.namespace_7));
                    }
                    break;

                case keywordState.namespace_7:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.namespace_8));
                    }
                    break;

                case keywordState.namespace_8:
                    linksToEnd = true;
                    break;

                case keywordState.new_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.new_1));
                    }
                    break;

                case keywordState.new_1:
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(keywordState.new_2));
                    }
                    break;

                case keywordState.new_2:
                    linksToEnd = true;
                    break;

                case keywordState.null_0:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.null_1));
                    }
                    break;

                case keywordState.null_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.null_2));
                    }
                    break;

                case keywordState.null_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.null_3));
                    }
                    break;

                case keywordState.null_3:
                    linksToEnd = true;
                    break;

                case keywordState.object_0:
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.object_1));
                    }
                    break;

                case keywordState.object_1:
                    if (ch == 'j')
                    {
                        validNextStates.Add((int)(keywordState.object_2));
                    }
                    break;

                case keywordState.object_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.object_3));
                    }
                    break;

                case keywordState.object_3:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.object_4));
                    }
                    break;

                case keywordState.object_4:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.object_5));
                    }
                    break;

                case keywordState.object_5:
                    linksToEnd = true;
                    break;

                case keywordState.operator_0:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.operator_1));
                    }
                    break;

                case keywordState.operator_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.operator_2));
                    }
                    break;

                case keywordState.operator_2:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.operator_3));
                    }
                    break;

                case keywordState.operator_3:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.operator_4));
                    }
                    break;

                case keywordState.operator_4:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.operator_5));
                    }
                    break;

                case keywordState.operator_5:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.operator_6));
                    }
                    break;

                case keywordState.operator_6:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.operator_7));
                    }
                    break;

                case keywordState.operator_7:
                    linksToEnd = true;
                    break;

                case keywordState.out_0:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.out_1));
                    }
                    break;

                case keywordState.out_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.out_2));
                    }
                    break;

                case keywordState.out_2:
                    linksToEnd = true;
                    break;

                case keywordState.override_0:
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.override_1));
                    }
                    break;

                case keywordState.override_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.override_2));
                    }
                    break;

                case keywordState.override_2:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.override_3));
                    }
                    break;

                case keywordState.override_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.override_4));
                    }
                    break;

                case keywordState.override_4:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.override_5));
                    }
                    break;

                case keywordState.override_5:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.override_6));
                    }
                    break;

                case keywordState.override_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.override_7));
                    }
                    break;

                case keywordState.override_7:
                    linksToEnd = true;
                    break;

                case keywordState.params_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.params_1));
                    }
                    break;

                case keywordState.params_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.params_2));
                    }
                    break;

                case keywordState.params_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.params_3));
                    }
                    break;

                case keywordState.params_3:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(keywordState.params_4));
                    }
                    break;

                case keywordState.params_4:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.params_5));
                    }
                    break;

                case keywordState.params_5:
                    linksToEnd = true;
                    break;

                case keywordState.private_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.private_1));
                    }
                    break;

                case keywordState.private_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.private_2));
                    }
                    break;

                case keywordState.private_2:
                    if (ch == 'v')
                    {
                        validNextStates.Add((int)(keywordState.private_3));
                    }
                    break;

                case keywordState.private_3:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.private_4));
                    }
                    break;

                case keywordState.private_4:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.private_5));
                    }
                    break;

                case keywordState.private_5:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.private_6));
                    }
                    break;

                case keywordState.private_6:
                    linksToEnd = true;
                    break;

                case keywordState.protected_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.protected_1));
                    }
                    break;

                case keywordState.protected_1:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.protected_2));
                    }
                    break;

                case keywordState.protected_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.protected_3));
                    }
                    break;

                case keywordState.protected_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.protected_4));
                    }
                    break;

                case keywordState.protected_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.protected_5));
                    }
                    break;

                case keywordState.protected_5:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.protected_6));
                    }
                    break;

                case keywordState.protected_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.protected_7));
                    }
                    break;

                case keywordState.protected_7:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.protected_8));
                    }
                    break;

                case keywordState.protected_8:
                    linksToEnd = true;
                    break;

                case keywordState.public_0:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.public_1));
                    }
                    break;

                case keywordState.public_1:
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.public_2));
                    }
                    break;

                case keywordState.public_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.public_3));
                    }
                    break;

                case keywordState.public_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.public_4));
                    }
                    break;

                case keywordState.public_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.public_5));
                    }
                    break;

                case keywordState.public_5:
                    linksToEnd = true;
                    break;

                case keywordState.readonly_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.readonly_1));
                    }
                    break;

                case keywordState.readonly_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.readonly_2));
                    }
                    break;

                case keywordState.readonly_2:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.readonly_3));
                    }
                    break;

                case keywordState.readonly_3:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.readonly_4));
                    }
                    break;

                case keywordState.readonly_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.readonly_5));
                    }
                    break;

                case keywordState.readonly_5:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.readonly_6));
                    }
                    break;

                case keywordState.readonly_6:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.readonly_7));
                    }
                    break;

                case keywordState.readonly_7:
                    linksToEnd = true;
                    break;

                case keywordState.ref_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.ref_1));
                    }
                    break;

                case keywordState.ref_1:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.ref_2));
                    }
                    break;

                case keywordState.ref_2:
                    linksToEnd = true;
                    break;

                case keywordState.return_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.return_1));
                    }
                    break;

                case keywordState.return_1:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.return_2));
                    }
                    break;

                case keywordState.return_2:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.return_3));
                    }
                    break;

                case keywordState.return_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.return_4));
                    }
                    break;

                case keywordState.return_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.return_5));
                    }
                    break;

                case keywordState.return_5:
                    linksToEnd = true;
                    break;

                case keywordState.sbyte_0:
                    if (ch == 'b')
                    {
                        validNextStates.Add((int)(keywordState.sbyte_1));
                    }
                    break;

                case keywordState.sbyte_1:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.sbyte_2));
                    }
                    break;

                case keywordState.sbyte_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.sbyte_3));
                    }
                    break;

                case keywordState.sbyte_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.sbyte_4));
                    }
                    break;

                case keywordState.sbyte_4:
                    linksToEnd = true;
                    break;

                case keywordState.sealed_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.sealed_1));
                    }
                    break;

                case keywordState.sealed_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.sealed_2));
                    }
                    break;

                case keywordState.sealed_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.sealed_3));
                    }
                    break;

                case keywordState.sealed_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.sealed_4));
                    }
                    break;

                case keywordState.sealed_4:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.sealed_5));
                    }
                    break;

                case keywordState.sealed_5:
                    linksToEnd = true;
                    break;

                case keywordState.short_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.short_1));
                    }
                    break;

                case keywordState.short_1:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.short_2));
                    }
                    break;

                case keywordState.short_2:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.short_3));
                    }
                    break;

                case keywordState.short_3:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.short_4));
                    }
                    break;

                case keywordState.short_4:
                    linksToEnd = true;
                    break;

                case keywordState.sizeof_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_1));
                    }
                    break;

                case keywordState.sizeof_1:
                    if (ch == 'z')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_2));
                    }
                    break;

                case keywordState.sizeof_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_3));
                    }
                    break;

                case keywordState.sizeof_3:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_4));
                    }
                    break;

                case keywordState.sizeof_4:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.sizeof_5));
                    }
                    break;

                case keywordState.sizeof_5:
                    linksToEnd = true;
                    break;

                case keywordState.stackalloc_0:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_1));
                    }
                    break;

                case keywordState.stackalloc_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_2));
                    }
                    break;

                case keywordState.stackalloc_2:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_3));
                    }
                    break;

                case keywordState.stackalloc_3:
                    if (ch == 'k')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_4));
                    }
                    break;

                case keywordState.stackalloc_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_5));
                    }
                    break;

                case keywordState.stackalloc_5:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_6));
                    }
                    break;

                case keywordState.stackalloc_6:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_7));
                    }
                    break;

                case keywordState.stackalloc_7:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_8));
                    }
                    break;

                case keywordState.stackalloc_8:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.stackalloc_9));
                    }
                    break;

                case keywordState.stackalloc_9:
                    linksToEnd = true;
                    break;

                case keywordState.static_0:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.static_1));
                    }
                    break;

                case keywordState.static_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.static_2));
                    }
                    break;

                case keywordState.static_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.static_3));
                    }
                    break;

                case keywordState.static_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.static_4));
                    }
                    break;

                case keywordState.static_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.static_5));
                    }
                    break;

                case keywordState.static_5:
                    linksToEnd = true;
                    break;

                case keywordState.string_0:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.string_1));
                    }
                    break;

                case keywordState.string_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.string_2));
                    }
                    break;

                case keywordState.string_2:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.string_3));
                    }
                    break;

                case keywordState.string_3:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.string_4));
                    }
                    break;

                case keywordState.string_4:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.string_5));
                    }
                    break;

                case keywordState.string_5:
                    linksToEnd = true;
                    break;

                case keywordState.struct_0:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.struct_1));
                    }
                    break;

                case keywordState.struct_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.struct_2));
                    }
                    break;

                case keywordState.struct_2:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.struct_3));
                    }
                    break;

                case keywordState.struct_3:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.struct_4));
                    }
                    break;

                case keywordState.struct_4:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.struct_5));
                    }
                    break;

                case keywordState.struct_5:
                    linksToEnd = true;
                    break;

                case keywordState.switch_0:
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(keywordState.switch_1));
                    }
                    break;

                case keywordState.switch_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.switch_2));
                    }
                    break;

                case keywordState.switch_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.switch_3));
                    }
                    break;

                case keywordState.switch_3:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.switch_4));
                    }
                    break;

                case keywordState.switch_4:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.switch_5));
                    }
                    break;

                case keywordState.switch_5:
                    linksToEnd = true;
                    break;

                case keywordState.this_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.this_1));
                    }
                    break;

                case keywordState.this_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.this_2));
                    }
                    break;

                case keywordState.this_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.this_3));
                    }
                    break;

                case keywordState.this_3:
                    linksToEnd = true;
                    break;

                case keywordState.throw_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.throw_1));
                    }
                    break;

                case keywordState.throw_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.throw_2));
                    }
                    break;

                case keywordState.throw_2:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.throw_3));
                    }
                    break;

                case keywordState.throw_3:
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(keywordState.throw_4));
                    }
                    break;

                case keywordState.throw_4:
                    linksToEnd = true;
                    break;

                case keywordState.true_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.true_1));
                    }
                    break;

                case keywordState.true_1:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.true_2));
                    }
                    break;

                case keywordState.true_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.true_3));
                    }
                    break;

                case keywordState.true_3:
                    linksToEnd = true;
                    break;

                case keywordState.try_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.try_1));
                    }
                    break;

                case keywordState.try_1:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.try_2));
                    }
                    break;

                case keywordState.try_2:
                    linksToEnd = true;
                    break;

                case keywordState.typeof_0:
                    if (ch == 'y')
                    {
                        validNextStates.Add((int)(keywordState.typeof_1));
                    }
                    break;

                case keywordState.typeof_1:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(keywordState.typeof_2));
                    }
                    break;

                case keywordState.typeof_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.typeof_3));
                    }
                    break;

                case keywordState.typeof_3:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.typeof_4));
                    }
                    break;

                case keywordState.typeof_4:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.typeof_5));
                    }
                    break;

                case keywordState.typeof_5:
                    linksToEnd = true;
                    break;

                case keywordState.uint_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.uint_1));
                    }
                    break;

                case keywordState.uint_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.uint_2));
                    }
                    break;

                case keywordState.uint_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.uint_3));
                    }
                    break;

                case keywordState.uint_3:
                    linksToEnd = true;
                    break;

                case keywordState.ulong_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.ulong_1));
                    }
                    break;

                case keywordState.ulong_1:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.ulong_2));
                    }
                    break;

                case keywordState.ulong_2:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.ulong_3));
                    }
                    break;

                case keywordState.ulong_3:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.ulong_4));
                    }
                    break;

                case keywordState.ulong_4:
                    linksToEnd = true;
                    break;

                case keywordState.unchecked_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_1));
                    }
                    break;

                case keywordState.unchecked_1:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_2));
                    }
                    break;

                case keywordState.unchecked_2:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_3));
                    }
                    break;

                case keywordState.unchecked_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_4));
                    }
                    break;

                case keywordState.unchecked_4:
                    if (ch == 'c')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_5));
                    }
                    break;

                case keywordState.unchecked_5:
                    if (ch == 'k')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_6));
                    }
                    break;

                case keywordState.unchecked_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_7));
                    }
                    break;

                case keywordState.unchecked_7:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.unchecked_8));
                    }
                    break;

                case keywordState.unchecked_8:
                    linksToEnd = true;
                    break;

                case keywordState.unsafe_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_1));
                    }
                    break;

                case keywordState.unsafe_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_2));
                    }
                    break;

                case keywordState.unsafe_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_3));
                    }
                    break;

                case keywordState.unsafe_3:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_4));
                    }
                    break;

                case keywordState.unsafe_4:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.unsafe_5));
                    }
                    break;

                case keywordState.unsafe_5:
                    linksToEnd = true;
                    break;

                case keywordState.ushort_0:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.ushort_1));
                    }
                    break;

                case keywordState.ushort_1:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.ushort_2));
                    }
                    break;

                case keywordState.ushort_2:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.ushort_3));
                    }
                    break;

                case keywordState.ushort_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.ushort_4));
                    }
                    break;

                case keywordState.ushort_4:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.ushort_5));
                    }
                    break;

                case keywordState.ushort_5:
                    linksToEnd = true;
                    break;

                case keywordState.using_0:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(keywordState.using_1));
                    }
                    break;

                case keywordState.using_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.using_2));
                    }
                    break;

                case keywordState.using_2:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(keywordState.using_3));
                    }
                    break;

                case keywordState.using_3:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(keywordState.using_4));
                    }
                    break;

                case keywordState.using_4:
                    linksToEnd = true;
                    break;

                case keywordState.virtual_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.virtual_1));
                    }
                    break;

                case keywordState.virtual_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(keywordState.virtual_2));
                    }
                    break;

                case keywordState.virtual_2:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.virtual_3));
                    }
                    break;

                case keywordState.virtual_3:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(keywordState.virtual_4));
                    }
                    break;

                case keywordState.virtual_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.virtual_5));
                    }
                    break;

                case keywordState.virtual_5:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.virtual_6));
                    }
                    break;

                case keywordState.virtual_6:
                    linksToEnd = true;
                    break;

                case keywordState.void_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.void_1));
                    }
                    break;

                case keywordState.void_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.void_2));
                    }
                    break;

                case keywordState.void_2:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(keywordState.void_3));
                    }
                    break;

                case keywordState.void_3:
                    linksToEnd = true;
                    break;

                case keywordState.volatile_0:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(keywordState.volatile_1));
                    }
                    break;

                case keywordState.volatile_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.volatile_2));
                    }
                    break;

                case keywordState.volatile_2:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(keywordState.volatile_3));
                    }
                    break;

                case keywordState.volatile_3:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(keywordState.volatile_4));
                    }
                    break;

                case keywordState.volatile_4:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.volatile_5));
                    }
                    break;

                case keywordState.volatile_5:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.volatile_6));
                    }
                    break;

                case keywordState.volatile_6:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.volatile_7));
                    }
                    break;

                case keywordState.volatile_7:
                    linksToEnd = true;
                    break;

                case keywordState.while_0:
                    if (ch == 'h')
                    {
                        validNextStates.Add((int)(keywordState.while_1));
                    }
                    break;

                case keywordState.while_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(keywordState.while_2));
                    }
                    break;

                case keywordState.while_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(keywordState.while_3));
                    }
                    break;

                case keywordState.while_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(keywordState.while_4));
                    }
                    break;

                case keywordState.while_4:
                    linksToEnd = true;
                    break;

                case keywordState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_keyword(string input, ref int i, int nextState)
        {
            switch ((keywordState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_keyword(int state)
        {
            switch ((keywordState)(state))
            {
                case keywordState.start:
                    return "start";

                case keywordState.abstract_0:
                    return "abstract";

                case keywordState.abstract_1:
                    return "abstract";

                case keywordState.abstract_2:
                    return "abstract";

                case keywordState.abstract_3:
                    return "abstract";

                case keywordState.abstract_4:
                    return "abstract";

                case keywordState.abstract_5:
                    return "abstract";

                case keywordState.abstract_6:
                    return "abstract";

                case keywordState.abstract_7:
                    return "abstract";

                case keywordState.as_0:
                    return "as";

                case keywordState.as_1:
                    return "as";

                case keywordState.base_0:
                    return "base";

                case keywordState.base_1:
                    return "base";

                case keywordState.base_2:
                    return "base";

                case keywordState.base_3:
                    return "base";

                case keywordState.bool_0:
                    return "bool";

                case keywordState.bool_1:
                    return "bool";

                case keywordState.bool_2:
                    return "bool";

                case keywordState.bool_3:
                    return "bool";

                case keywordState.break_0:
                    return "break";

                case keywordState.break_1:
                    return "break";

                case keywordState.break_2:
                    return "break";

                case keywordState.break_3:
                    return "break";

                case keywordState.break_4:
                    return "break";

                case keywordState.byte_0:
                    return "byte";

                case keywordState.byte_1:
                    return "byte";

                case keywordState.byte_2:
                    return "byte";

                case keywordState.byte_3:
                    return "byte";

                case keywordState.case_0:
                    return "case";

                case keywordState.case_1:
                    return "case";

                case keywordState.case_2:
                    return "case";

                case keywordState.case_3:
                    return "case";

                case keywordState.catch_0:
                    return "catch";

                case keywordState.catch_1:
                    return "catch";

                case keywordState.catch_2:
                    return "catch";

                case keywordState.catch_3:
                    return "catch";

                case keywordState.catch_4:
                    return "catch";

                case keywordState.char_0:
                    return "char";

                case keywordState.char_1:
                    return "char";

                case keywordState.char_2:
                    return "char";

                case keywordState.char_3:
                    return "char";

                case keywordState.checked_0:
                    return "checked";

                case keywordState.checked_1:
                    return "checked";

                case keywordState.checked_2:
                    return "checked";

                case keywordState.checked_3:
                    return "checked";

                case keywordState.checked_4:
                    return "checked";

                case keywordState.checked_5:
                    return "checked";

                case keywordState.checked_6:
                    return "checked";

                case keywordState.class_0:
                    return "class";

                case keywordState.class_1:
                    return "class";

                case keywordState.class_2:
                    return "class";

                case keywordState.class_3:
                    return "class";

                case keywordState.class_4:
                    return "class";

                case keywordState.const_0:
                    return "const";

                case keywordState.const_1:
                    return "const";

                case keywordState.const_2:
                    return "const";

                case keywordState.const_3:
                    return "const";

                case keywordState.const_4:
                    return "const";

                case keywordState.continue_0:
                    return "continue";

                case keywordState.continue_1:
                    return "continue";

                case keywordState.continue_2:
                    return "continue";

                case keywordState.continue_3:
                    return "continue";

                case keywordState.continue_4:
                    return "continue";

                case keywordState.continue_5:
                    return "continue";

                case keywordState.continue_6:
                    return "continue";

                case keywordState.continue_7:
                    return "continue";

                case keywordState.decimal_0:
                    return "decimal";

                case keywordState.decimal_1:
                    return "decimal";

                case keywordState.decimal_2:
                    return "decimal";

                case keywordState.decimal_3:
                    return "decimal";

                case keywordState.decimal_4:
                    return "decimal";

                case keywordState.decimal_5:
                    return "decimal";

                case keywordState.decimal_6:
                    return "decimal";

                case keywordState.default_0:
                    return "default";

                case keywordState.default_1:
                    return "default";

                case keywordState.default_2:
                    return "default";

                case keywordState.default_3:
                    return "default";

                case keywordState.default_4:
                    return "default";

                case keywordState.default_5:
                    return "default";

                case keywordState.default_6:
                    return "default";

                case keywordState.delegate_0:
                    return "delegate";

                case keywordState.delegate_1:
                    return "delegate";

                case keywordState.delegate_2:
                    return "delegate";

                case keywordState.delegate_3:
                    return "delegate";

                case keywordState.delegate_4:
                    return "delegate";

                case keywordState.delegate_5:
                    return "delegate";

                case keywordState.delegate_6:
                    return "delegate";

                case keywordState.delegate_7:
                    return "delegate";

                case keywordState.do_0:
                    return "do";

                case keywordState.do_1:
                    return "do";

                case keywordState.double_0:
                    return "double";

                case keywordState.double_1:
                    return "double";

                case keywordState.double_2:
                    return "double";

                case keywordState.double_3:
                    return "double";

                case keywordState.double_4:
                    return "double";

                case keywordState.double_5:
                    return "double";

                case keywordState.else_0:
                    return "else";

                case keywordState.else_1:
                    return "else";

                case keywordState.else_2:
                    return "else";

                case keywordState.else_3:
                    return "else";

                case keywordState.enum_0:
                    return "enum";

                case keywordState.enum_1:
                    return "enum";

                case keywordState.enum_2:
                    return "enum";

                case keywordState.enum_3:
                    return "enum";

                case keywordState.event_0:
                    return "event";

                case keywordState.event_1:
                    return "event";

                case keywordState.event_2:
                    return "event";

                case keywordState.event_3:
                    return "event";

                case keywordState.event_4:
                    return "event";

                case keywordState.explicit_0:
                    return "explicit";

                case keywordState.explicit_1:
                    return "explicit";

                case keywordState.explicit_2:
                    return "explicit";

                case keywordState.explicit_3:
                    return "explicit";

                case keywordState.explicit_4:
                    return "explicit";

                case keywordState.explicit_5:
                    return "explicit";

                case keywordState.explicit_6:
                    return "explicit";

                case keywordState.explicit_7:
                    return "explicit";

                case keywordState.extern_0:
                    return "extern";

                case keywordState.extern_1:
                    return "extern";

                case keywordState.extern_2:
                    return "extern";

                case keywordState.extern_3:
                    return "extern";

                case keywordState.extern_4:
                    return "extern";

                case keywordState.extern_5:
                    return "extern";

                case keywordState.false_0:
                    return "false";

                case keywordState.false_1:
                    return "false";

                case keywordState.false_2:
                    return "false";

                case keywordState.false_3:
                    return "false";

                case keywordState.false_4:
                    return "false";

                case keywordState.finally_0:
                    return "finally";

                case keywordState.finally_1:
                    return "finally";

                case keywordState.finally_2:
                    return "finally";

                case keywordState.finally_3:
                    return "finally";

                case keywordState.finally_4:
                    return "finally";

                case keywordState.finally_5:
                    return "finally";

                case keywordState.finally_6:
                    return "finally";

                case keywordState.fixed_0:
                    return "fixed";

                case keywordState.fixed_1:
                    return "fixed";

                case keywordState.fixed_2:
                    return "fixed";

                case keywordState.fixed_3:
                    return "fixed";

                case keywordState.fixed_4:
                    return "fixed";

                case keywordState.float_0:
                    return "float";

                case keywordState.float_1:
                    return "float";

                case keywordState.float_2:
                    return "float";

                case keywordState.float_3:
                    return "float";

                case keywordState.float_4:
                    return "float";

                case keywordState.for_0:
                    return "for";

                case keywordState.for_1:
                    return "for";

                case keywordState.for_2:
                    return "for";

                case keywordState.foreach_0:
                    return "foreach";

                case keywordState.foreach_1:
                    return "foreach";

                case keywordState.foreach_2:
                    return "foreach";

                case keywordState.foreach_3:
                    return "foreach";

                case keywordState.foreach_4:
                    return "foreach";

                case keywordState.foreach_5:
                    return "foreach";

                case keywordState.foreach_6:
                    return "foreach";

                case keywordState.goto_0:
                    return "goto";

                case keywordState.goto_1:
                    return "goto";

                case keywordState.goto_2:
                    return "goto";

                case keywordState.goto_3:
                    return "goto";

                case keywordState.if_0:
                    return "if";

                case keywordState.if_1:
                    return "if";

                case keywordState.implicit_0:
                    return "implicit";

                case keywordState.implicit_1:
                    return "implicit";

                case keywordState.implicit_2:
                    return "implicit";

                case keywordState.implicit_3:
                    return "implicit";

                case keywordState.implicit_4:
                    return "implicit";

                case keywordState.implicit_5:
                    return "implicit";

                case keywordState.implicit_6:
                    return "implicit";

                case keywordState.implicit_7:
                    return "implicit";

                case keywordState.in_0:
                    return "in";

                case keywordState.in_1:
                    return "in";

                case keywordState.int_0:
                    return "int";

                case keywordState.int_1:
                    return "int";

                case keywordState.int_2:
                    return "int";

                case keywordState.interface_0:
                    return "interface";

                case keywordState.interface_1:
                    return "interface";

                case keywordState.interface_2:
                    return "interface";

                case keywordState.interface_3:
                    return "interface";

                case keywordState.interface_4:
                    return "interface";

                case keywordState.interface_5:
                    return "interface";

                case keywordState.interface_6:
                    return "interface";

                case keywordState.interface_7:
                    return "interface";

                case keywordState.interface_8:
                    return "interface";

                case keywordState.internal_0:
                    return "internal";

                case keywordState.internal_1:
                    return "internal";

                case keywordState.internal_2:
                    return "internal";

                case keywordState.internal_3:
                    return "internal";

                case keywordState.internal_4:
                    return "internal";

                case keywordState.internal_5:
                    return "internal";

                case keywordState.internal_6:
                    return "internal";

                case keywordState.internal_7:
                    return "internal";

                case keywordState.is_0:
                    return "is";

                case keywordState.is_1:
                    return "is";

                case keywordState.lock_0:
                    return "lock";

                case keywordState.lock_1:
                    return "lock";

                case keywordState.lock_2:
                    return "lock";

                case keywordState.lock_3:
                    return "lock";

                case keywordState.long_0:
                    return "long";

                case keywordState.long_1:
                    return "long";

                case keywordState.long_2:
                    return "long";

                case keywordState.long_3:
                    return "long";

                case keywordState.namespace_0:
                    return "namespace";

                case keywordState.namespace_1:
                    return "namespace";

                case keywordState.namespace_2:
                    return "namespace";

                case keywordState.namespace_3:
                    return "namespace";

                case keywordState.namespace_4:
                    return "namespace";

                case keywordState.namespace_5:
                    return "namespace";

                case keywordState.namespace_6:
                    return "namespace";

                case keywordState.namespace_7:
                    return "namespace";

                case keywordState.namespace_8:
                    return "namespace";

                case keywordState.new_0:
                    return "new";

                case keywordState.new_1:
                    return "new";

                case keywordState.new_2:
                    return "new";

                case keywordState.null_0:
                    return "null";

                case keywordState.null_1:
                    return "null";

                case keywordState.null_2:
                    return "null";

                case keywordState.null_3:
                    return "null";

                case keywordState.object_0:
                    return "object";

                case keywordState.object_1:
                    return "object";

                case keywordState.object_2:
                    return "object";

                case keywordState.object_3:
                    return "object";

                case keywordState.object_4:
                    return "object";

                case keywordState.object_5:
                    return "object";

                case keywordState.operator_0:
                    return "operator";

                case keywordState.operator_1:
                    return "operator";

                case keywordState.operator_2:
                    return "operator";

                case keywordState.operator_3:
                    return "operator";

                case keywordState.operator_4:
                    return "operator";

                case keywordState.operator_5:
                    return "operator";

                case keywordState.operator_6:
                    return "operator";

                case keywordState.operator_7:
                    return "operator";

                case keywordState.out_0:
                    return "out";

                case keywordState.out_1:
                    return "out";

                case keywordState.out_2:
                    return "out";

                case keywordState.override_0:
                    return "override";

                case keywordState.override_1:
                    return "override";

                case keywordState.override_2:
                    return "override";

                case keywordState.override_3:
                    return "override";

                case keywordState.override_4:
                    return "override";

                case keywordState.override_5:
                    return "override";

                case keywordState.override_6:
                    return "override";

                case keywordState.override_7:
                    return "override";

                case keywordState.params_0:
                    return "params";

                case keywordState.params_1:
                    return "params";

                case keywordState.params_2:
                    return "params";

                case keywordState.params_3:
                    return "params";

                case keywordState.params_4:
                    return "params";

                case keywordState.params_5:
                    return "params";

                case keywordState.private_0:
                    return "private";

                case keywordState.private_1:
                    return "private";

                case keywordState.private_2:
                    return "private";

                case keywordState.private_3:
                    return "private";

                case keywordState.private_4:
                    return "private";

                case keywordState.private_5:
                    return "private";

                case keywordState.private_6:
                    return "private";

                case keywordState.protected_0:
                    return "protected";

                case keywordState.protected_1:
                    return "protected";

                case keywordState.protected_2:
                    return "protected";

                case keywordState.protected_3:
                    return "protected";

                case keywordState.protected_4:
                    return "protected";

                case keywordState.protected_5:
                    return "protected";

                case keywordState.protected_6:
                    return "protected";

                case keywordState.protected_7:
                    return "protected";

                case keywordState.protected_8:
                    return "protected";

                case keywordState.public_0:
                    return "public";

                case keywordState.public_1:
                    return "public";

                case keywordState.public_2:
                    return "public";

                case keywordState.public_3:
                    return "public";

                case keywordState.public_4:
                    return "public";

                case keywordState.public_5:
                    return "public";

                case keywordState.readonly_0:
                    return "readonly";

                case keywordState.readonly_1:
                    return "readonly";

                case keywordState.readonly_2:
                    return "readonly";

                case keywordState.readonly_3:
                    return "readonly";

                case keywordState.readonly_4:
                    return "readonly";

                case keywordState.readonly_5:
                    return "readonly";

                case keywordState.readonly_6:
                    return "readonly";

                case keywordState.readonly_7:
                    return "readonly";

                case keywordState.ref_0:
                    return "ref";

                case keywordState.ref_1:
                    return "ref";

                case keywordState.ref_2:
                    return "ref";

                case keywordState.return_0:
                    return "return";

                case keywordState.return_1:
                    return "return";

                case keywordState.return_2:
                    return "return";

                case keywordState.return_3:
                    return "return";

                case keywordState.return_4:
                    return "return";

                case keywordState.return_5:
                    return "return";

                case keywordState.sbyte_0:
                    return "sbyte";

                case keywordState.sbyte_1:
                    return "sbyte";

                case keywordState.sbyte_2:
                    return "sbyte";

                case keywordState.sbyte_3:
                    return "sbyte";

                case keywordState.sbyte_4:
                    return "sbyte";

                case keywordState.sealed_0:
                    return "sealed";

                case keywordState.sealed_1:
                    return "sealed";

                case keywordState.sealed_2:
                    return "sealed";

                case keywordState.sealed_3:
                    return "sealed";

                case keywordState.sealed_4:
                    return "sealed";

                case keywordState.sealed_5:
                    return "sealed";

                case keywordState.short_0:
                    return "short";

                case keywordState.short_1:
                    return "short";

                case keywordState.short_2:
                    return "short";

                case keywordState.short_3:
                    return "short";

                case keywordState.short_4:
                    return "short";

                case keywordState.sizeof_0:
                    return "sizeof";

                case keywordState.sizeof_1:
                    return "sizeof";

                case keywordState.sizeof_2:
                    return "sizeof";

                case keywordState.sizeof_3:
                    return "sizeof";

                case keywordState.sizeof_4:
                    return "sizeof";

                case keywordState.sizeof_5:
                    return "sizeof";

                case keywordState.stackalloc_0:
                    return "stackalloc";

                case keywordState.stackalloc_1:
                    return "stackalloc";

                case keywordState.stackalloc_2:
                    return "stackalloc";

                case keywordState.stackalloc_3:
                    return "stackalloc";

                case keywordState.stackalloc_4:
                    return "stackalloc";

                case keywordState.stackalloc_5:
                    return "stackalloc";

                case keywordState.stackalloc_6:
                    return "stackalloc";

                case keywordState.stackalloc_7:
                    return "stackalloc";

                case keywordState.stackalloc_8:
                    return "stackalloc";

                case keywordState.stackalloc_9:
                    return "stackalloc";

                case keywordState.static_0:
                    return "static";

                case keywordState.static_1:
                    return "static";

                case keywordState.static_2:
                    return "static";

                case keywordState.static_3:
                    return "static";

                case keywordState.static_4:
                    return "static";

                case keywordState.static_5:
                    return "static";

                case keywordState.string_0:
                    return "string";

                case keywordState.string_1:
                    return "string";

                case keywordState.string_2:
                    return "string";

                case keywordState.string_3:
                    return "string";

                case keywordState.string_4:
                    return "string";

                case keywordState.string_5:
                    return "string";

                case keywordState.struct_0:
                    return "struct";

                case keywordState.struct_1:
                    return "struct";

                case keywordState.struct_2:
                    return "struct";

                case keywordState.struct_3:
                    return "struct";

                case keywordState.struct_4:
                    return "struct";

                case keywordState.struct_5:
                    return "struct";

                case keywordState.switch_0:
                    return "switch";

                case keywordState.switch_1:
                    return "switch";

                case keywordState.switch_2:
                    return "switch";

                case keywordState.switch_3:
                    return "switch";

                case keywordState.switch_4:
                    return "switch";

                case keywordState.switch_5:
                    return "switch";

                case keywordState.this_0:
                    return "this";

                case keywordState.this_1:
                    return "this";

                case keywordState.this_2:
                    return "this";

                case keywordState.this_3:
                    return "this";

                case keywordState.throw_0:
                    return "throw";

                case keywordState.throw_1:
                    return "throw";

                case keywordState.throw_2:
                    return "throw";

                case keywordState.throw_3:
                    return "throw";

                case keywordState.throw_4:
                    return "throw";

                case keywordState.true_0:
                    return "true";

                case keywordState.true_1:
                    return "true";

                case keywordState.true_2:
                    return "true";

                case keywordState.true_3:
                    return "true";

                case keywordState.try_0:
                    return "try";

                case keywordState.try_1:
                    return "try";

                case keywordState.try_2:
                    return "try";

                case keywordState.typeof_0:
                    return "typeof";

                case keywordState.typeof_1:
                    return "typeof";

                case keywordState.typeof_2:
                    return "typeof";

                case keywordState.typeof_3:
                    return "typeof";

                case keywordState.typeof_4:
                    return "typeof";

                case keywordState.typeof_5:
                    return "typeof";

                case keywordState.uint_0:
                    return "uint";

                case keywordState.uint_1:
                    return "uint";

                case keywordState.uint_2:
                    return "uint";

                case keywordState.uint_3:
                    return "uint";

                case keywordState.ulong_0:
                    return "ulong";

                case keywordState.ulong_1:
                    return "ulong";

                case keywordState.ulong_2:
                    return "ulong";

                case keywordState.ulong_3:
                    return "ulong";

                case keywordState.ulong_4:
                    return "ulong";

                case keywordState.unchecked_0:
                    return "unchecked";

                case keywordState.unchecked_1:
                    return "unchecked";

                case keywordState.unchecked_2:
                    return "unchecked";

                case keywordState.unchecked_3:
                    return "unchecked";

                case keywordState.unchecked_4:
                    return "unchecked";

                case keywordState.unchecked_5:
                    return "unchecked";

                case keywordState.unchecked_6:
                    return "unchecked";

                case keywordState.unchecked_7:
                    return "unchecked";

                case keywordState.unchecked_8:
                    return "unchecked";

                case keywordState.unsafe_0:
                    return "unsafe";

                case keywordState.unsafe_1:
                    return "unsafe";

                case keywordState.unsafe_2:
                    return "unsafe";

                case keywordState.unsafe_3:
                    return "unsafe";

                case keywordState.unsafe_4:
                    return "unsafe";

                case keywordState.unsafe_5:
                    return "unsafe";

                case keywordState.ushort_0:
                    return "ushort";

                case keywordState.ushort_1:
                    return "ushort";

                case keywordState.ushort_2:
                    return "ushort";

                case keywordState.ushort_3:
                    return "ushort";

                case keywordState.ushort_4:
                    return "ushort";

                case keywordState.ushort_5:
                    return "ushort";

                case keywordState.using_0:
                    return "using";

                case keywordState.using_1:
                    return "using";

                case keywordState.using_2:
                    return "using";

                case keywordState.using_3:
                    return "using";

                case keywordState.using_4:
                    return "using";

                case keywordState.virtual_0:
                    return "virtual";

                case keywordState.virtual_1:
                    return "virtual";

                case keywordState.virtual_2:
                    return "virtual";

                case keywordState.virtual_3:
                    return "virtual";

                case keywordState.virtual_4:
                    return "virtual";

                case keywordState.virtual_5:
                    return "virtual";

                case keywordState.virtual_6:
                    return "virtual";

                case keywordState.void_0:
                    return "void";

                case keywordState.void_1:
                    return "void";

                case keywordState.void_2:
                    return "void";

                case keywordState.void_3:
                    return "void";

                case keywordState.volatile_0:
                    return "volatile";

                case keywordState.volatile_1:
                    return "volatile";

                case keywordState.volatile_2:
                    return "volatile";

                case keywordState.volatile_3:
                    return "volatile";

                case keywordState.volatile_4:
                    return "volatile";

                case keywordState.volatile_5:
                    return "volatile";

                case keywordState.volatile_6:
                    return "volatile";

                case keywordState.volatile_7:
                    return "volatile";

                case keywordState.while_0:
                    return "while";

                case keywordState.while_1:
                    return "while";

                case keywordState.while_2:
                    return "while";

                case keywordState.while_3:
                    return "while";

                case keywordState.while_4:
                    return "while";

                case keywordState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getliteral(string input, ref int i)
        {
            return GetItem(input, ref i, true, "literal", (int)(literalState.start), (int)(literalState.end), this.GetValidNextStates_literal, this.GetStateTag_literal, this.GetSubSpan_literal);
        }

        public Int32[] GetValidNextStates_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((literalState)(currentState))
            {
                case literalState.start:
                    if (ch == 't' || ch == 'f')
                    {
                        validNextStates.Add((int)(literalState.boolean_literal));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(literalState.integer_literal));
                    }
                    if (char.IsDigit(ch) || ch == '.')
                    {
                        validNextStates.Add((int)(literalState.real_literal));
                    }
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(literalState.character_literal));
                    }
                    if (ch == '"' || ch == '@')
                    {
                        validNextStates.Add((int)(literalState.string_literal));
                    }
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(literalState.null_literal));
                    }
                    break;

                case literalState.boolean_literal:
                    linksToEnd = true;
                    break;

                case literalState.integer_literal:
                    linksToEnd = true;
                    break;

                case literalState.real_literal:
                    linksToEnd = true;
                    break;

                case literalState.character_literal:
                    linksToEnd = true;
                    break;

                case literalState.string_literal:
                    linksToEnd = true;
                    break;

                case literalState.null_literal:
                    linksToEnd = true;
                    break;

                case literalState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_literal(string input, ref int i, int nextState)
        {
            switch ((literalState)(nextState))
            {
                case literalState.boolean_literal:
                    return Getboolean_literal(input, ref i);

                case literalState.integer_literal:
                    return Getinteger_literal(input, ref i);

                case literalState.real_literal:
                    return Getreal_literal(input, ref i);

                case literalState.character_literal:
                    return Getcharacter_literal(input, ref i);

                case literalState.string_literal:
                    return Getstring_literal(input, ref i);

                case literalState.null_literal:
                    return Getnull_literal(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_literal(int state)
        {
            switch ((literalState)(state))
            {
                case literalState.start:
                    return "start";

                case literalState.boolean_literal:
                    return "boolean-literal";

                case literalState.integer_literal:
                    return "integer-literal";

                case literalState.real_literal:
                    return "real-literal";

                case literalState.character_literal:
                    return "character-literal";

                case literalState.string_literal:
                    return "string-literal";

                case literalState.null_literal:
                    return "null-literal";

                case literalState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getboolean_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "boolean-literal", (int)(booleanliteralState.start), (int)(booleanliteralState.end), this.GetValidNextStates_boolean_literal, this.GetStateTag_boolean_literal, this.GetSubSpan_boolean_literal);
        }

        public Int32[] GetValidNextStates_boolean_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((booleanliteralState)(currentState))
            {
                case booleanliteralState.start:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(booleanliteralState.true_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(booleanliteralState.false_0));
                    }
                    break;

                case booleanliteralState.true_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(booleanliteralState.true_1));
                    }
                    break;

                case booleanliteralState.true_1:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(booleanliteralState.true_2));
                    }
                    break;

                case booleanliteralState.true_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(booleanliteralState.true_3));
                    }
                    break;

                case booleanliteralState.true_3:
                    linksToEnd = true;
                    break;

                case booleanliteralState.false_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(booleanliteralState.false_1));
                    }
                    break;

                case booleanliteralState.false_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(booleanliteralState.false_2));
                    }
                    break;

                case booleanliteralState.false_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(booleanliteralState.false_3));
                    }
                    break;

                case booleanliteralState.false_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(booleanliteralState.false_4));
                    }
                    break;

                case booleanliteralState.false_4:
                    linksToEnd = true;
                    break;

                case booleanliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_boolean_literal(string input, ref int i, int nextState)
        {
            switch ((booleanliteralState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_boolean_literal(int state)
        {
            switch ((booleanliteralState)(state))
            {
                case booleanliteralState.start:
                    return "start";

                case booleanliteralState.true_0:
                    return "true";

                case booleanliteralState.true_1:
                    return "true";

                case booleanliteralState.true_2:
                    return "true";

                case booleanliteralState.true_3:
                    return "true";

                case booleanliteralState.false_0:
                    return "false";

                case booleanliteralState.false_1:
                    return "false";

                case booleanliteralState.false_2:
                    return "false";

                case booleanliteralState.false_3:
                    return "false";

                case booleanliteralState.false_4:
                    return "false";

                case booleanliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinteger_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "integer-literal", (int)(integerliteralState.start), (int)(integerliteralState.end), this.GetValidNextStates_integer_literal, this.GetStateTag_integer_literal, this.GetSubSpan_integer_literal);
        }

        public Int32[] GetValidNextStates_integer_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((integerliteralState)(currentState))
            {
                case integerliteralState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(integerliteralState.decimal_integer_literal));
                    }
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(integerliteralState.hexadecimal_integer_literal));
                    }
                    break;

                case integerliteralState.decimal_integer_literal:
                    linksToEnd = true;
                    break;

                case integerliteralState.hexadecimal_integer_literal:
                    linksToEnd = true;
                    break;

                case integerliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_integer_literal(string input, ref int i, int nextState)
        {
            switch ((integerliteralState)(nextState))
            {
                case integerliteralState.decimal_integer_literal:
                    return Getdecimal_integer_literal(input, ref i);

                case integerliteralState.hexadecimal_integer_literal:
                    return Gethexadecimal_integer_literal(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_integer_literal(int state)
        {
            switch ((integerliteralState)(state))
            {
                case integerliteralState.start:
                    return "start";

                case integerliteralState.decimal_integer_literal:
                    return "decimal-integer-literal";

                case integerliteralState.hexadecimal_integer_literal:
                    return "hexadecimal-integer-literal";

                case integerliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getdecimal_integer_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "decimal-integer-literal", (int)(decimalintegerliteralState.start), (int)(decimalintegerliteralState.end), this.GetValidNextStates_decimal_integer_literal, this.GetStateTag_decimal_integer_literal, this.GetSubSpan_decimal_integer_literal);
        }

        public Int32[] GetValidNextStates_decimal_integer_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((decimalintegerliteralState)(currentState))
            {
                case decimalintegerliteralState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(decimalintegerliteralState.class_digit));
                    }
                    break;

                case decimalintegerliteralState.class_digit:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(decimalintegerliteralState.class_digit));
                    }
                    if (ch == 'u' || ch == 'l' || ch == 'U' || ch == 'L')
                    {
                        validNextStates.Add((int)(decimalintegerliteralState.integer_type_suffix));
                    }
                    linksToEnd = true;
                    break;

                case decimalintegerliteralState.integer_type_suffix:
                    linksToEnd = true;
                    break;

                case decimalintegerliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_decimal_integer_literal(string input, ref int i, int nextState)
        {
            switch ((decimalintegerliteralState)(nextState))
            {
                case decimalintegerliteralState.integer_type_suffix:
                    return Getinteger_type_suffix(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_decimal_integer_literal(int state)
        {
            switch ((decimalintegerliteralState)(state))
            {
                case decimalintegerliteralState.start:
                    return "start";

                case decimalintegerliteralState.class_digit:
                    return "\\d";

                case decimalintegerliteralState.integer_type_suffix:
                    return "integer-type-suffix";

                case decimalintegerliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getdecimal_digits(string input, ref int i)
        {
            return GetItem(input, ref i, true, "decimal-digits", (int)(decimaldigitsState.start), (int)(decimaldigitsState.end), this.GetValidNextStates_decimal_digits, this.GetStateTag_decimal_digits, this.GetSubSpan_decimal_digits);
        }

        public Int32[] GetValidNextStates_decimal_digits(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((decimaldigitsState)(currentState))
            {
                case decimaldigitsState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(decimaldigitsState.decimal_digit));
                    }
                    break;

                case decimaldigitsState.decimal_digit:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(decimaldigitsState.decimal_digit));
                    }
                    linksToEnd = true;
                    break;

                case decimaldigitsState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_decimal_digits(string input, ref int i, int nextState)
        {
            switch ((decimaldigitsState)(nextState))
            {
                case decimaldigitsState.decimal_digit:
                    return Getdecimal_digit(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_decimal_digits(int state)
        {
            switch ((decimaldigitsState)(state))
            {
                case decimaldigitsState.start:
                    return "start";

                case decimaldigitsState.decimal_digit:
                    return "decimal-digit";

                case decimaldigitsState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getdecimal_digit(string input, ref int i)
        {
            return GetItem(input, ref i, true, "decimal-digit", (int)(decimaldigitState.start), (int)(decimaldigitState.end), this.GetValidNextStates_decimal_digit, this.GetStateTag_decimal_digit, this.GetSubSpan_decimal_digit);
        }

        public Int32[] GetValidNextStates_decimal_digit(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((decimaldigitState)(currentState))
            {
                case decimaldigitState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(decimaldigitState.class_digit));
                    }
                    break;

                case decimaldigitState.class_digit:
                    linksToEnd = true;
                    break;

                case decimaldigitState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_decimal_digit(string input, ref int i, int nextState)
        {
            switch ((decimaldigitState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_decimal_digit(int state)
        {
            switch ((decimaldigitState)(state))
            {
                case decimaldigitState.start:
                    return "start";

                case decimaldigitState.class_digit:
                    return "\\d";

                case decimaldigitState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getinteger_type_suffix(string input, ref int i)
        {
            return GetItem(input, ref i, true, "integer-type-suffix", (int)(integertypesuffixState.start), (int)(integertypesuffixState.end), this.GetValidNextStates_integer_type_suffix, this.GetStateTag_integer_type_suffix, this.GetSubSpan_integer_type_suffix);
        }

        public Int32[] GetValidNextStates_integer_type_suffix(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((integertypesuffixState)(currentState))
            {
                case integertypesuffixState.start:
                    if (ch == 'u' || ch == 'U')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_u_0));
                    }
                    if (ch == 'l' || ch == 'L')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_l_0));
                    }
                    if (ch == 'l' || ch == 'L')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_l_1));
                    }
                    if (ch == 'u' || ch == 'U')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_u_1));
                    }
                    break;

                case integertypesuffixState.letter_u_0:
                    linksToEnd = true;
                    break;

                case integertypesuffixState.letter_l_0:
                    if (ch == 'u' || ch == 'U')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_u_0));
                    }
                    break;

                case integertypesuffixState.letter_l_1:
                    linksToEnd = true;
                    break;

                case integertypesuffixState.letter_u_1:
                    if (ch == 'l' || ch == 'L')
                    {
                        validNextStates.Add((int)(integertypesuffixState.letter_l_1));
                    }
                    break;

                case integertypesuffixState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_integer_type_suffix(string input, ref int i, int nextState)
        {
            switch ((integertypesuffixState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_integer_type_suffix(int state)
        {
            switch ((integertypesuffixState)(state))
            {
                case integertypesuffixState.start:
                    return "start";

                case integertypesuffixState.letter_u_0:
                    return "u";

                case integertypesuffixState.letter_l_0:
                    return "l";

                case integertypesuffixState.letter_l_1:
                    return "l";

                case integertypesuffixState.letter_u_1:
                    return "u";

                case integertypesuffixState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Gethexadecimal_integer_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "hexadecimal-integer-literal", (int)(hexadecimalintegerliteralState.start), (int)(hexadecimalintegerliteralState.end), this.GetValidNextStates_hexadecimal_integer_literal, this.GetStateTag_hexadecimal_integer_literal, this.GetSubSpan_hexadecimal_integer_literal);
        }

        public Int32[] GetValidNextStates_hexadecimal_integer_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((hexadecimalintegerliteralState)(currentState))
            {
                case hexadecimalintegerliteralState.start:
                    if (ch == '0')
                    {
                        validNextStates.Add((int)(hexadecimalintegerliteralState._0x_0));
                    }
                    break;

                case hexadecimalintegerliteralState._0x_0:
                    if (ch == 'x' || ch == 'X')
                    {
                        validNextStates.Add((int)(hexadecimalintegerliteralState._0x_1));
                    }
                    break;

                case hexadecimalintegerliteralState._0x_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalintegerliteralState.hex_digits));
                    }
                    break;

                case hexadecimalintegerliteralState.hex_digits:
                    if (ch == 'u' || ch == 'l' || ch == 'U' || ch == 'L')
                    {
                        validNextStates.Add((int)(hexadecimalintegerliteralState.integer_type_suffix));
                    }
                    linksToEnd = true;
                    break;

                case hexadecimalintegerliteralState.integer_type_suffix:
                    linksToEnd = true;
                    break;

                case hexadecimalintegerliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_hexadecimal_integer_literal(string input, ref int i, int nextState)
        {
            switch ((hexadecimalintegerliteralState)(nextState))
            {
                case hexadecimalintegerliteralState.hex_digits:
                    return Gethex_digits(input, ref i);

                case hexadecimalintegerliteralState.integer_type_suffix:
                    return Getinteger_type_suffix(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_hexadecimal_integer_literal(int state)
        {
            switch ((hexadecimalintegerliteralState)(state))
            {
                case hexadecimalintegerliteralState.start:
                    return "start";

                case hexadecimalintegerliteralState._0x_0:
                    return "0x";

                case hexadecimalintegerliteralState._0x_1:
                    return "0x";

                case hexadecimalintegerliteralState.hex_digits:
                    return "hex-digits";

                case hexadecimalintegerliteralState.integer_type_suffix:
                    return "integer-type-suffix";

                case hexadecimalintegerliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Gethex_digits(string input, ref int i)
        {
            return GetItem(input, ref i, true, "hex-digits", (int)(hexdigitsState.start), (int)(hexdigitsState.end), this.GetValidNextStates_hex_digits, this.GetStateTag_hex_digits, this.GetSubSpan_hex_digits);
        }

        public Int32[] GetValidNextStates_hex_digits(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((hexdigitsState)(currentState))
            {
                case hexdigitsState.start:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexdigitsState.hex_digit));
                    }
                    break;

                case hexdigitsState.hex_digit:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexdigitsState.hex_digit));
                    }
                    linksToEnd = true;
                    break;

                case hexdigitsState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_hex_digits(string input, ref int i, int nextState)
        {
            switch ((hexdigitsState)(nextState))
            {
                case hexdigitsState.hex_digit:
                    return Gethex_digit(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_hex_digits(int state)
        {
            switch ((hexdigitsState)(state))
            {
                case hexdigitsState.start:
                    return "start";

                case hexdigitsState.hex_digit:
                    return "hex-digit";

                case hexdigitsState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Gethex_digit(string input, ref int i)
        {
            return GetItem(input, ref i, true, "hex-digit", (int)(hexdigitState.start), (int)(hexdigitState.end), this.GetValidNextStates_hex_digit, this.GetStateTag_hex_digit, this.GetSubSpan_hex_digit);
        }

        public Int32[] GetValidNextStates_hex_digit(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((hexdigitState)(currentState))
            {
                case hexdigitState.start:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexdigitState.dabcdef));
                    }
                    break;

                case hexdigitState.dabcdef:
                    linksToEnd = true;
                    break;

                case hexdigitState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_hex_digit(string input, ref int i, int nextState)
        {
            switch ((hexdigitState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_hex_digit(int state)
        {
            switch ((hexdigitState)(state))
            {
                case hexdigitState.start:
                    return "start";

                case hexdigitState.dabcdef:
                    return "\\dabcdef";

                case hexdigitState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getreal_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "real-literal", (int)(realliteralState.start), (int)(realliteralState.end), this.GetValidNextStates_real_literal, this.GetStateTag_real_literal, this.GetSubSpan_real_literal);
        }

        public Int32[] GetValidNextStates_real_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((realliteralState)(currentState))
            {
                case realliteralState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(realliteralState.decimal_digits_0));
                    }
                    if (ch == '.')
                    {
                        validNextStates.Add((int)(realliteralState.period_0));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(realliteralState.decimal_digits_1));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(realliteralState.decimal_digits_2));
                    }
                    break;

                case realliteralState.decimal_digits_0:
                    if (ch == '.')
                    {
                        validNextStates.Add((int)(realliteralState.period_1));
                    }
                    break;

                case realliteralState.period_0:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(realliteralState.decimal_digits_3));
                    }
                    break;

                case realliteralState.decimal_digits_1:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(realliteralState.exponent_part_0));
                    }
                    break;

                case realliteralState.decimal_digits_2:
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_0));
                    }
                    break;

                case realliteralState.period_1:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(realliteralState.decimal_digits_4));
                    }
                    break;

                case realliteralState.decimal_digits_3:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(realliteralState.exponent_part_1));
                    }
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_1));
                    }
                    linksToEnd = true;
                    break;

                case realliteralState.exponent_part_0:
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_2));
                    }
                    linksToEnd = true;
                    break;

                case realliteralState.real_type_suffix_0:
                    linksToEnd = true;
                    break;

                case realliteralState.decimal_digits_4:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(realliteralState.exponent_part_2));
                    }
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_3));
                    }
                    linksToEnd = true;
                    break;

                case realliteralState.exponent_part_1:
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_1));
                    }
                    linksToEnd = true;
                    break;

                case realliteralState.real_type_suffix_1:
                    linksToEnd = true;
                    break;

                case realliteralState.end:
                    break;

                case realliteralState.real_type_suffix_2:
                    linksToEnd = true;
                    break;

                case realliteralState.exponent_part_2:
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realliteralState.real_type_suffix_3));
                    }
                    linksToEnd = true;
                    break;

                case realliteralState.real_type_suffix_3:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_real_literal(string input, ref int i, int nextState)
        {
            switch ((realliteralState)(nextState))
            {
                case realliteralState.decimal_digits_0:
                    return Getdecimal_digits(input, ref i);

                case realliteralState.decimal_digits_1:
                    return Getdecimal_digits(input, ref i);

                case realliteralState.decimal_digits_2:
                    return Getdecimal_digits(input, ref i);

                case realliteralState.decimal_digits_3:
                    return Getdecimal_digits(input, ref i);

                case realliteralState.exponent_part_0:
                    return Getexponent_part(input, ref i);

                case realliteralState.real_type_suffix_0:
                    return Getreal_type_suffix(input, ref i);

                case realliteralState.decimal_digits_4:
                    return Getdecimal_digits(input, ref i);

                case realliteralState.exponent_part_1:
                    return Getexponent_part(input, ref i);

                case realliteralState.real_type_suffix_1:
                    return Getreal_type_suffix(input, ref i);

                case realliteralState.real_type_suffix_2:
                    return Getreal_type_suffix(input, ref i);

                case realliteralState.exponent_part_2:
                    return Getexponent_part(input, ref i);

                case realliteralState.real_type_suffix_3:
                    return Getreal_type_suffix(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_real_literal(int state)
        {
            switch ((realliteralState)(state))
            {
                case realliteralState.start:
                    return "start";

                case realliteralState.decimal_digits_0:
                    return "decimal-digits";

                case realliteralState.period_0:
                    return ".";

                case realliteralState.decimal_digits_1:
                    return "decimal-digits";

                case realliteralState.decimal_digits_2:
                    return "decimal-digits";

                case realliteralState.period_1:
                    return ".";

                case realliteralState.decimal_digits_3:
                    return "decimal-digits";

                case realliteralState.exponent_part_0:
                    return "exponent-part";

                case realliteralState.real_type_suffix_0:
                    return "real-type-suffix";

                case realliteralState.decimal_digits_4:
                    return "decimal-digits";

                case realliteralState.exponent_part_1:
                    return "exponent-part";

                case realliteralState.real_type_suffix_1:
                    return "real-type-suffix";

                case realliteralState.end:
                    return "end";

                case realliteralState.real_type_suffix_2:
                    return "real-type-suffix";

                case realliteralState.exponent_part_2:
                    return "exponent-part";

                case realliteralState.real_type_suffix_3:
                    return "real-type-suffix";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getexponent_part(string input, ref int i)
        {
            return GetItem(input, ref i, true, "exponent-part", (int)(exponentpartState.start), (int)(exponentpartState.end), this.GetValidNextStates_exponent_part, this.GetStateTag_exponent_part, this.GetSubSpan_exponent_part);
        }

        public Int32[] GetValidNextStates_exponent_part(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((exponentpartState)(currentState))
            {
                case exponentpartState.start:
                    if (ch == 'e' || ch == 'E')
                    {
                        validNextStates.Add((int)(exponentpartState.letter_e));
                    }
                    break;

                case exponentpartState.letter_e:
                    if (ch == '+' || ch == '-')
                    {
                        validNextStates.Add((int)(exponentpartState.plus_hyphen));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(exponentpartState.class_digit));
                    }
                    break;

                case exponentpartState.plus_hyphen:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(exponentpartState.class_digit));
                    }
                    break;

                case exponentpartState.class_digit:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(exponentpartState.class_digit));
                    }
                    linksToEnd = true;
                    break;

                case exponentpartState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_exponent_part(string input, ref int i, int nextState)
        {
            switch ((exponentpartState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_exponent_part(int state)
        {
            switch ((exponentpartState)(state))
            {
                case exponentpartState.start:
                    return "start";

                case exponentpartState.letter_e:
                    return "e";

                case exponentpartState.plus_hyphen:
                    return "+-";

                case exponentpartState.class_digit:
                    return "\\d";

                case exponentpartState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getreal_type_suffix(string input, ref int i)
        {
            return GetItem(input, ref i, true, "real-type-suffix", (int)(realtypesuffixState.start), (int)(realtypesuffixState.end), this.GetValidNextStates_real_type_suffix, this.GetStateTag_real_type_suffix, this.GetSubSpan_real_type_suffix);
        }

        public Int32[] GetValidNextStates_real_type_suffix(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((realtypesuffixState)(currentState))
            {
                case realtypesuffixState.start:
                    if (ch == 'f' || ch == 'd' || ch == 'm' || ch == 'F' || ch == 'D' || ch == 'M')
                    {
                        validNextStates.Add((int)(realtypesuffixState.fdm));
                    }
                    break;

                case realtypesuffixState.fdm:
                    linksToEnd = true;
                    break;

                case realtypesuffixState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_real_type_suffix(string input, ref int i, int nextState)
        {
            switch ((realtypesuffixState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_real_type_suffix(int state)
        {
            switch ((realtypesuffixState)(state))
            {
                case realtypesuffixState.start:
                    return "start";

                case realtypesuffixState.fdm:
                    return "fdm";

                case realtypesuffixState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getcharacter_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "character-literal", (int)(characterliteralState.start), (int)(characterliteralState.end), this.GetValidNextStates_character_literal, this.GetStateTag_character_literal, this.GetSubSpan_character_literal);
        }

        public Int32[] GetValidNextStates_character_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((characterliteralState)(currentState))
            {
                case characterliteralState.start:
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(characterliteralState.quote_0));
                    }
                    break;

                case characterliteralState.quote_0:
                    if (!(ch == '\'' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(characterliteralState.character));
                    }
                    break;

                case characterliteralState.character:
                    if (ch == '\'')
                    {
                        validNextStates.Add((int)(characterliteralState.quote_1));
                    }
                    break;

                case characterliteralState.quote_1:
                    linksToEnd = true;
                    break;

                case characterliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_character_literal(string input, ref int i, int nextState)
        {
            switch ((characterliteralState)(nextState))
            {
                case characterliteralState.character:
                    return Getcharacter(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_character_literal(int state)
        {
            switch ((characterliteralState)(state))
            {
                case characterliteralState.start:
                    return "start";

                case characterliteralState.quote_0:
                    return "'";

                case characterliteralState.character:
                    return "character";

                case characterliteralState.quote_1:
                    return "'";

                case characterliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getcharacter(string input, ref int i)
        {
            return GetItem(input, ref i, true, "character", (int)(characterState.start), (int)(characterState.end), this.GetValidNextStates_character, this.GetStateTag_character, this.GetSubSpan_character);
        }

        public Int32[] GetValidNextStates_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((characterState)(currentState))
            {
                case characterState.start:
                    if (!(ch == '\\' || ch == '\'' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(characterState.single_character));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(characterState.simple_escape_sequence));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(characterState.hexadecimal_escape_sequence));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(characterState.unicode_escape_sequence));
                    }
                    break;

                case characterState.single_character:
                    linksToEnd = true;
                    break;

                case characterState.simple_escape_sequence:
                    linksToEnd = true;
                    break;

                case characterState.hexadecimal_escape_sequence:
                    linksToEnd = true;
                    break;

                case characterState.unicode_escape_sequence:
                    linksToEnd = true;
                    break;

                case characterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_character(string input, ref int i, int nextState)
        {
            switch ((characterState)(nextState))
            {
                case characterState.single_character:
                    return Getsingle_character(input, ref i);

                case characterState.simple_escape_sequence:
                    return Getsimple_escape_sequence(input, ref i);

                case characterState.hexadecimal_escape_sequence:
                    return Gethexadecimal_escape_sequence(input, ref i);

                case characterState.unicode_escape_sequence:
                    return Getunicode_escape_sequence(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_character(int state)
        {
            switch ((characterState)(state))
            {
                case characterState.start:
                    return "start";

                case characterState.single_character:
                    return "single-character";

                case characterState.simple_escape_sequence:
                    return "simple-escape-sequence";

                case characterState.hexadecimal_escape_sequence:
                    return "hexadecimal-escape-sequence";

                case characterState.unicode_escape_sequence:
                    return "unicode-escape-sequence";

                case characterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getsingle_character(string input, ref int i)
        {
            return GetItem(input, ref i, true, "single-character", (int)(singlecharacterState.start), (int)(singlecharacterState.end), this.GetValidNextStates_single_character, this.GetStateTag_single_character, this.GetSubSpan_single_character);
        }

        public Int32[] GetValidNextStates_single_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((singlecharacterState)(currentState))
            {
                case singlecharacterState.start:
                    if (!(ch == '\\' || ch == '\'' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(singlecharacterState.nrx0085x2028x2029));
                    }
                    break;

                case singlecharacterState.nrx0085x2028x2029:
                    linksToEnd = true;
                    break;

                case singlecharacterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_single_character(string input, ref int i, int nextState)
        {
            switch ((singlecharacterState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_single_character(int state)
        {
            switch ((singlecharacterState)(state))
            {
                case singlecharacterState.start:
                    return "start";

                case singlecharacterState.nrx0085x2028x2029:
                    return "^\\\\'\\n\\r\\x0085\\x2028\\x2029";

                case singlecharacterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getsimple_escape_sequence(string input, ref int i)
        {
            return GetItem(input, ref i, true, "simple-escape-sequence", (int)(simpleescapesequenceState.start), (int)(simpleescapesequenceState.end), this.GetValidNextStates_simple_escape_sequence, this.GetStateTag_simple_escape_sequence, this.GetSubSpan_simple_escape_sequence);
        }

        public Int32[] GetValidNextStates_simple_escape_sequence(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((simpleescapesequenceState)(currentState))
            {
                case simpleescapesequenceState.start:
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(simpleescapesequenceState.bslash));
                    }
                    break;

                case simpleescapesequenceState.bslash:
                    if (ch == '"' || ch == '\\' || ch == '0' || ch == 'a' || ch == 'b' || ch == 'f' || ch == 'n' || ch == 'r' || ch == 't' || ch == 'v')
                    {
                        validNextStates.Add((int)(simpleescapesequenceState._0abfnrtv));
                    }
                    break;

                case simpleescapesequenceState._0abfnrtv:
                    linksToEnd = true;
                    break;

                case simpleescapesequenceState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_simple_escape_sequence(string input, ref int i, int nextState)
        {
            switch ((simpleescapesequenceState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_simple_escape_sequence(int state)
        {
            switch ((simpleescapesequenceState)(state))
            {
                case simpleescapesequenceState.start:
                    return "start";

                case simpleescapesequenceState.bslash:
                    return "\\";

                case simpleescapesequenceState._0abfnrtv:
                    return "\"\\\\0abfnrtv";

                case simpleescapesequenceState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Gethexadecimal_escape_sequence(string input, ref int i)
        {
            return GetItem(input, ref i, true, "hexadecimal-escape-sequence", (int)(hexadecimalescapesequenceState.start), (int)(hexadecimalescapesequenceState.end), this.GetValidNextStates_hexadecimal_escape_sequence, this.GetStateTag_hexadecimal_escape_sequence, this.GetSubSpan_hexadecimal_escape_sequence);
        }

        public Int32[] GetValidNextStates_hexadecimal_escape_sequence(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((hexadecimalescapesequenceState)(currentState))
            {
                case hexadecimalescapesequenceState.start:
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.bslash_letter_x_0));
                    }
                    break;

                case hexadecimalescapesequenceState.bslash_letter_x_0:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.bslash_letter_x_1));
                    }
                    break;

                case hexadecimalescapesequenceState.bslash_letter_x_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_0));
                    }
                    break;

                case hexadecimalescapesequenceState.hex_digit_0:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_1));
                    }
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_2));
                    }
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_3));
                    }
                    linksToEnd = true;
                    break;

                case hexadecimalescapesequenceState.hex_digit_1:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_2));
                    }
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_3));
                    }
                    linksToEnd = true;
                    break;

                case hexadecimalescapesequenceState.hex_digit_2:
                    if (char.IsDigit(ch) || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f' || ch == 'A' || ch == 'B' || ch == 'C' || ch == 'D' || ch == 'E' || ch == 'F')
                    {
                        validNextStates.Add((int)(hexadecimalescapesequenceState.hex_digit_3));
                    }
                    linksToEnd = true;
                    break;

                case hexadecimalescapesequenceState.hex_digit_3:
                    linksToEnd = true;
                    break;

                case hexadecimalescapesequenceState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_hexadecimal_escape_sequence(string input, ref int i, int nextState)
        {
            switch ((hexadecimalescapesequenceState)(nextState))
            {
                case hexadecimalescapesequenceState.hex_digit_0:
                    return Gethex_digit(input, ref i);

                case hexadecimalescapesequenceState.hex_digit_1:
                    return Gethex_digit(input, ref i);

                case hexadecimalescapesequenceState.hex_digit_2:
                    return Gethex_digit(input, ref i);

                case hexadecimalescapesequenceState.hex_digit_3:
                    return Gethex_digit(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_hexadecimal_escape_sequence(int state)
        {
            switch ((hexadecimalescapesequenceState)(state))
            {
                case hexadecimalescapesequenceState.start:
                    return "start";

                case hexadecimalescapesequenceState.bslash_letter_x_0:
                    return "\\x";

                case hexadecimalescapesequenceState.bslash_letter_x_1:
                    return "\\x";

                case hexadecimalescapesequenceState.hex_digit_0:
                    return "hex-digit";

                case hexadecimalescapesequenceState.hex_digit_1:
                    return "hex-digit";

                case hexadecimalescapesequenceState.hex_digit_2:
                    return "hex-digit";

                case hexadecimalescapesequenceState.hex_digit_3:
                    return "hex-digit";

                case hexadecimalescapesequenceState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getstring_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "string-literal", (int)(stringliteralState.start), (int)(stringliteralState.end), this.GetValidNextStates_string_literal, this.GetStateTag_string_literal, this.GetSubSpan_string_literal);
        }

        public Int32[] GetValidNextStates_string_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((stringliteralState)(currentState))
            {
                case stringliteralState.start:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(stringliteralState.regular_string_literal));
                    }
                    if (ch == '@')
                    {
                        validNextStates.Add((int)(stringliteralState.verbatim_string_literal));
                    }
                    break;

                case stringliteralState.regular_string_literal:
                    linksToEnd = true;
                    break;

                case stringliteralState.verbatim_string_literal:
                    linksToEnd = true;
                    break;

                case stringliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_string_literal(string input, ref int i, int nextState)
        {
            switch ((stringliteralState)(nextState))
            {
                case stringliteralState.regular_string_literal:
                    return Getregular_string_literal(input, ref i);

                case stringliteralState.verbatim_string_literal:
                    return Getverbatim_string_literal(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_string_literal(int state)
        {
            switch ((stringliteralState)(state))
            {
                case stringliteralState.start:
                    return "start";

                case stringliteralState.regular_string_literal:
                    return "regular-string-literal";

                case stringliteralState.verbatim_string_literal:
                    return "verbatim-string-literal";

                case stringliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getregular_string_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "regular-string-literal", (int)(regularstringliteralState.start), (int)(regularstringliteralState.end), this.GetValidNextStates_regular_string_literal, this.GetStateTag_regular_string_literal, this.GetSubSpan_regular_string_literal);
        }

        public Int32[] GetValidNextStates_regular_string_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((regularstringliteralState)(currentState))
            {
                case regularstringliteralState.start:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(regularstringliteralState.dquote_0));
                    }
                    break;

                case regularstringliteralState.dquote_0:
                    if (!(ch == '"' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(regularstringliteralState.regular_string_literal_character));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(regularstringliteralState.dquote_1));
                    }
                    break;

                case regularstringliteralState.regular_string_literal_character:
                    if (!(ch == '"' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(regularstringliteralState.regular_string_literal_character));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(regularstringliteralState.dquote_1));
                    }
                    break;

                case regularstringliteralState.dquote_1:
                    linksToEnd = true;
                    break;

                case regularstringliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_regular_string_literal(string input, ref int i, int nextState)
        {
            switch ((regularstringliteralState)(nextState))
            {
                case regularstringliteralState.regular_string_literal_character:
                    return Getregular_string_literal_character(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_regular_string_literal(int state)
        {
            switch ((regularstringliteralState)(state))
            {
                case regularstringliteralState.start:
                    return "start";

                case regularstringliteralState.dquote_0:
                    return "\"";

                case regularstringliteralState.regular_string_literal_character:
                    return "regular-string-literal-character";

                case regularstringliteralState.dquote_1:
                    return "\"";

                case regularstringliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getregular_string_literal_character(string input, ref int i)
        {
            return GetItem(input, ref i, true, "regular-string-literal-character", (int)(regularstringliteralcharacterState.start), (int)(regularstringliteralcharacterState.end), this.GetValidNextStates_regular_string_literal_character, this.GetStateTag_regular_string_literal_character, this.GetSubSpan_regular_string_literal_character);
        }

        public Int32[] GetValidNextStates_regular_string_literal_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((regularstringliteralcharacterState)(currentState))
            {
                case regularstringliteralcharacterState.start:
                    if (!(ch == '"' || ch == '\\' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(regularstringliteralcharacterState.single_regular_string_literal_character));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(regularstringliteralcharacterState.simple_escape_sequence));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(regularstringliteralcharacterState.hexadecimal_escape_sequence));
                    }
                    if (ch == '\\')
                    {
                        validNextStates.Add((int)(regularstringliteralcharacterState.unicode_escape_sequence));
                    }
                    break;

                case regularstringliteralcharacterState.single_regular_string_literal_character:
                    linksToEnd = true;
                    break;

                case regularstringliteralcharacterState.simple_escape_sequence:
                    linksToEnd = true;
                    break;

                case regularstringliteralcharacterState.hexadecimal_escape_sequence:
                    linksToEnd = true;
                    break;

                case regularstringliteralcharacterState.unicode_escape_sequence:
                    linksToEnd = true;
                    break;

                case regularstringliteralcharacterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_regular_string_literal_character(string input, ref int i, int nextState)
        {
            switch ((regularstringliteralcharacterState)(nextState))
            {
                case regularstringliteralcharacterState.single_regular_string_literal_character:
                    return Getsingle_regular_string_literal_character(input, ref i);

                case regularstringliteralcharacterState.simple_escape_sequence:
                    return Getsimple_escape_sequence(input, ref i);

                case regularstringliteralcharacterState.hexadecimal_escape_sequence:
                    return Gethexadecimal_escape_sequence(input, ref i);

                case regularstringliteralcharacterState.unicode_escape_sequence:
                    return Getunicode_escape_sequence(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_regular_string_literal_character(int state)
        {
            switch ((regularstringliteralcharacterState)(state))
            {
                case regularstringliteralcharacterState.start:
                    return "start";

                case regularstringliteralcharacterState.single_regular_string_literal_character:
                    return "single-regular-string-literal-character";

                case regularstringliteralcharacterState.simple_escape_sequence:
                    return "simple-escape-sequence";

                case regularstringliteralcharacterState.hexadecimal_escape_sequence:
                    return "hexadecimal-escape-sequence";

                case regularstringliteralcharacterState.unicode_escape_sequence:
                    return "unicode-escape-sequence";

                case regularstringliteralcharacterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getsingle_regular_string_literal_character(string input, ref int i)
        {
            return GetItem(input, ref i, true, "single-regular-string-literal-character", (int)(singleregularstringliteralcharacterState.start), (int)(singleregularstringliteralcharacterState.end), this.GetValidNextStates_single_regular_string_literal_character, this.GetStateTag_single_regular_string_literal_character, this.GetSubSpan_single_regular_string_literal_character);
        }

        public Int32[] GetValidNextStates_single_regular_string_literal_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((singleregularstringliteralcharacterState)(currentState))
            {
                case singleregularstringliteralcharacterState.start:
                    if (!(ch == '"' || ch == '\\' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(singleregularstringliteralcharacterState.nrx0085x2028x2029));
                    }
                    break;

                case singleregularstringliteralcharacterState.nrx0085x2028x2029:
                    linksToEnd = true;
                    break;

                case singleregularstringliteralcharacterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_single_regular_string_literal_character(string input, ref int i, int nextState)
        {
            switch ((singleregularstringliteralcharacterState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_single_regular_string_literal_character(int state)
        {
            switch ((singleregularstringliteralcharacterState)(state))
            {
                case singleregularstringliteralcharacterState.start:
                    return "start";

                case singleregularstringliteralcharacterState.nrx0085x2028x2029:
                    return "^\"\\\\\\n\\r\\x0085\\x2028\\x2029";

                case singleregularstringliteralcharacterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getverbatim_string_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "verbatim-string-literal", (int)(verbatimstringliteralState.start), (int)(verbatimstringliteralState.end), this.GetValidNextStates_verbatim_string_literal, this.GetStateTag_verbatim_string_literal, this.GetSubSpan_verbatim_string_literal);
        }

        public Int32[] GetValidNextStates_verbatim_string_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((verbatimstringliteralState)(currentState))
            {
                case verbatimstringliteralState.start:
                    if (ch == '@')
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.at_dquote_0));
                    }
                    break;

                case verbatimstringliteralState.at_dquote_0:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.at_dquote_1));
                    }
                    break;

                case verbatimstringliteralState.at_dquote_1:
                    if (false)
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.verbatim_string_literal_character));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.dquote));
                    }
                    break;

                case verbatimstringliteralState.verbatim_string_literal_character:
                    if (false)
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.verbatim_string_literal_character));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(verbatimstringliteralState.dquote));
                    }
                    break;

                case verbatimstringliteralState.dquote:
                    linksToEnd = true;
                    break;

                case verbatimstringliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_verbatim_string_literal(string input, ref int i, int nextState)
        {
            switch ((verbatimstringliteralState)(nextState))
            {
                case verbatimstringliteralState.verbatim_string_literal_character:
                    return Getverbatim_string_literal_character(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_verbatim_string_literal(int state)
        {
            switch ((verbatimstringliteralState)(state))
            {
                case verbatimstringliteralState.start:
                    return "start";

                case verbatimstringliteralState.at_dquote_0:
                    return "@\"";

                case verbatimstringliteralState.at_dquote_1:
                    return "@\"";

                case verbatimstringliteralState.verbatim_string_literal_character:
                    return "verbatim-string-literal-character";

                case verbatimstringliteralState.dquote:
                    return "\"";

                case verbatimstringliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getverbatim_string_literal_character(string input, ref int i)
        {
            return GetItem(input, ref i, true, "verbatim-string-literal-character", (int)(verbatimstringliteralcharacterState.start), (int)(verbatimstringliteralcharacterState.end), this.GetValidNextStates_verbatim_string_literal_character, this.GetStateTag_verbatim_string_literal_character, this.GetSubSpan_verbatim_string_literal_character);
        }

        public Int32[] GetValidNextStates_verbatim_string_literal_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((verbatimstringliteralcharacterState)(currentState))
            {
                case verbatimstringliteralcharacterState.start:
                    if (!(ch == '"'))
                    {
                        validNextStates.Add((int)(verbatimstringliteralcharacterState.chevron_dquote));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(verbatimstringliteralcharacterState.dquote_dquote_0));
                    }
                    break;

                case verbatimstringliteralcharacterState.chevron_dquote:
                    linksToEnd = true;
                    break;

                case verbatimstringliteralcharacterState.dquote_dquote_0:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(verbatimstringliteralcharacterState.dquote_dquote_1));
                    }
                    break;

                case verbatimstringliteralcharacterState.dquote_dquote_1:
                    linksToEnd = true;
                    break;

                case verbatimstringliteralcharacterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_verbatim_string_literal_character(string input, ref int i, int nextState)
        {
            switch ((verbatimstringliteralcharacterState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_verbatim_string_literal_character(int state)
        {
            switch ((verbatimstringliteralcharacterState)(state))
            {
                case verbatimstringliteralcharacterState.start:
                    return "start";

                case verbatimstringliteralcharacterState.chevron_dquote:
                    return "^\"";

                case verbatimstringliteralcharacterState.dquote_dquote_0:
                    return "\"\"";

                case verbatimstringliteralcharacterState.dquote_dquote_1:
                    return "\"\"";

                case verbatimstringliteralcharacterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getnull_literal(string input, ref int i)
        {
            return GetItem(input, ref i, true, "null-literal", (int)(nullliteralState.start), (int)(nullliteralState.end), this.GetValidNextStates_null_literal, this.GetStateTag_null_literal, this.GetSubSpan_null_literal);
        }

        public Int32[] GetValidNextStates_null_literal(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((nullliteralState)(currentState))
            {
                case nullliteralState.start:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(nullliteralState.null_0));
                    }
                    break;

                case nullliteralState.null_0:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(nullliteralState.null_1));
                    }
                    break;

                case nullliteralState.null_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(nullliteralState.null_2));
                    }
                    break;

                case nullliteralState.null_2:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(nullliteralState.null_3));
                    }
                    break;

                case nullliteralState.null_3:
                    linksToEnd = true;
                    break;

                case nullliteralState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_null_literal(string input, ref int i, int nextState)
        {
            switch ((nullliteralState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_null_literal(int state)
        {
            switch ((nullliteralState)(state))
            {
                case nullliteralState.start:
                    return "start";

                case nullliteralState.null_0:
                    return "null";

                case nullliteralState.null_1:
                    return "null";

                case nullliteralState.null_2:
                    return "null";

                case nullliteralState.null_3:
                    return "null";

                case nullliteralState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getoperator_or_punctuator(string input, ref int i)
        {
            return GetItem(input, ref i, true, "operator-or-punctuator", (int)(operatororpunctuatorState.start), (int)(operatororpunctuatorState.end), this.GetValidNextStates_operator_or_punctuator, this.GetStateTag_operator_or_punctuator, this.GetSubSpan_operator_or_punctuator);
        }

        public Int32[] GetValidNextStates_operator_or_punctuator(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((operatororpunctuatorState)(currentState))
            {
                case operatororpunctuatorState.start:
                    if (ch == '{')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.obrace));
                    }
                    if (ch == '}')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.cbrace));
                    }
                    if (ch == '[')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.obracket));
                    }
                    if (ch == ']')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.cbracket));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.oparen));
                    }
                    if (ch == ')')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.cparen));
                    }
                    if (ch == '.')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.period));
                    }
                    if (ch == ',')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.comma));
                    }
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.colon));
                    }
                    if (ch == ';')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.semi));
                    }
                    if (ch == '+')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.plus));
                    }
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.star));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.slash));
                    }
                    if (ch == '%')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.percent));
                    }
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.amp));
                    }
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.pipe));
                    }
                    if (ch == '^')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.chevron));
                    }
                    if (ch == '!')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.exclamation));
                    }
                    if (ch == '~')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.tilde));
                    }
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.equal));
                    }
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less));
                    }
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.greater));
                    }
                    if (ch == '?')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.question));
                    }
                    if (ch == '?')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.question_question_0));
                    }
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.colon_colon_0));
                    }
                    if (ch == '+')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.plus_plus_0));
                    }
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_hyphen_0));
                    }
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.amp_amp_0));
                    }
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.pipe_pipe_0));
                    }
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_greater_0));
                    }
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.equal_equal_0));
                    }
                    if (ch == '!')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.exclamation_equal_0));
                    }
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_equal_0));
                    }
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.greater_equal_0));
                    }
                    if (ch == '+')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.plus_equal_0));
                    }
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_equal_0));
                    }
                    if (ch == '*')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.star_equal_0));
                    }
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.slash_equal_0));
                    }
                    if (ch == '%')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.percent_equal_0));
                    }
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.amp_equal_0));
                    }
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.pipe_equal_0));
                    }
                    if (ch == '^')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.chevron_equal_0));
                    }
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_less_0));
                    }
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_less_equal_0));
                    }
                    break;

                case operatororpunctuatorState.obrace:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.cbrace:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.obracket:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.cbracket:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.oparen:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.cparen:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.period:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.comma:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.colon:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.semi:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.plus:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.hyphen:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.star:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.slash:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.percent:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.amp:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.pipe:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.chevron:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.exclamation:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.tilde:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.equal:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.less:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.greater:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.question:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.question_question_0:
                    if (ch == '?')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.question_question_1));
                    }
                    break;

                case operatororpunctuatorState.question_question_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.colon_colon_0:
                    if (ch == ':')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.colon_colon_1));
                    }
                    break;

                case operatororpunctuatorState.colon_colon_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.plus_plus_0:
                    if (ch == '+')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.plus_plus_1));
                    }
                    break;

                case operatororpunctuatorState.plus_plus_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.hyphen_hyphen_0:
                    if (ch == '-')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_hyphen_1));
                    }
                    break;

                case operatororpunctuatorState.hyphen_hyphen_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.amp_amp_0:
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.amp_amp_1));
                    }
                    break;

                case operatororpunctuatorState.amp_amp_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.pipe_pipe_0:
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.pipe_pipe_1));
                    }
                    break;

                case operatororpunctuatorState.pipe_pipe_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.hyphen_greater_0:
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_greater_1));
                    }
                    break;

                case operatororpunctuatorState.hyphen_greater_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.equal_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.equal_equal_1));
                    }
                    break;

                case operatororpunctuatorState.equal_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.exclamation_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.exclamation_equal_1));
                    }
                    break;

                case operatororpunctuatorState.exclamation_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.less_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_equal_1));
                    }
                    break;

                case operatororpunctuatorState.less_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.greater_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.greater_equal_1));
                    }
                    break;

                case operatororpunctuatorState.greater_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.plus_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.plus_equal_1));
                    }
                    break;

                case operatororpunctuatorState.plus_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.hyphen_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.hyphen_equal_1));
                    }
                    break;

                case operatororpunctuatorState.hyphen_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.star_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.star_equal_1));
                    }
                    break;

                case operatororpunctuatorState.star_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.slash_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.slash_equal_1));
                    }
                    break;

                case operatororpunctuatorState.slash_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.percent_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.percent_equal_1));
                    }
                    break;

                case operatororpunctuatorState.percent_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.amp_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.amp_equal_1));
                    }
                    break;

                case operatororpunctuatorState.amp_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.pipe_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.pipe_equal_1));
                    }
                    break;

                case operatororpunctuatorState.pipe_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.chevron_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.chevron_equal_1));
                    }
                    break;

                case operatororpunctuatorState.chevron_equal_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.less_less_0:
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_less_1));
                    }
                    break;

                case operatororpunctuatorState.less_less_1:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.less_less_equal_0:
                    if (ch == '<')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_less_equal_1));
                    }
                    break;

                case operatororpunctuatorState.less_less_equal_1:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(operatororpunctuatorState.less_less_equal_2));
                    }
                    break;

                case operatororpunctuatorState.less_less_equal_2:
                    linksToEnd = true;
                    break;

                case operatororpunctuatorState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_operator_or_punctuator(string input, ref int i, int nextState)
        {
            switch ((operatororpunctuatorState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_operator_or_punctuator(int state)
        {
            switch ((operatororpunctuatorState)(state))
            {
                case operatororpunctuatorState.start:
                    return "start";

                case operatororpunctuatorState.obrace:
                    return "{";

                case operatororpunctuatorState.cbrace:
                    return "}";

                case operatororpunctuatorState.obracket:
                    return "[";

                case operatororpunctuatorState.cbracket:
                    return "]";

                case operatororpunctuatorState.oparen:
                    return "(";

                case operatororpunctuatorState.cparen:
                    return ")";

                case operatororpunctuatorState.period:
                    return ".";

                case operatororpunctuatorState.comma:
                    return ",";

                case operatororpunctuatorState.colon:
                    return ":";

                case operatororpunctuatorState.semi:
                    return ";";

                case operatororpunctuatorState.plus:
                    return "+";

                case operatororpunctuatorState.hyphen:
                    return "-";

                case operatororpunctuatorState.star:
                    return "*";

                case operatororpunctuatorState.slash:
                    return "/";

                case operatororpunctuatorState.percent:
                    return "%";

                case operatororpunctuatorState.amp:
                    return "&";

                case operatororpunctuatorState.pipe:
                    return "|";

                case operatororpunctuatorState.chevron:
                    return "^";

                case operatororpunctuatorState.exclamation:
                    return "!";

                case operatororpunctuatorState.tilde:
                    return "~";

                case operatororpunctuatorState.equal:
                    return "=";

                case operatororpunctuatorState.less:
                    return "<";

                case operatororpunctuatorState.greater:
                    return ">";

                case operatororpunctuatorState.question:
                    return "?";

                case operatororpunctuatorState.question_question_0:
                    return "??";

                case operatororpunctuatorState.question_question_1:
                    return "??";

                case operatororpunctuatorState.colon_colon_0:
                    return "::";

                case operatororpunctuatorState.colon_colon_1:
                    return "::";

                case operatororpunctuatorState.plus_plus_0:
                    return "++";

                case operatororpunctuatorState.plus_plus_1:
                    return "++";

                case operatororpunctuatorState.hyphen_hyphen_0:
                    return "--";

                case operatororpunctuatorState.hyphen_hyphen_1:
                    return "--";

                case operatororpunctuatorState.amp_amp_0:
                    return "&&";

                case operatororpunctuatorState.amp_amp_1:
                    return "&&";

                case operatororpunctuatorState.pipe_pipe_0:
                    return "||";

                case operatororpunctuatorState.pipe_pipe_1:
                    return "||";

                case operatororpunctuatorState.hyphen_greater_0:
                    return "->";

                case operatororpunctuatorState.hyphen_greater_1:
                    return "->";

                case operatororpunctuatorState.equal_equal_0:
                    return "==";

                case operatororpunctuatorState.equal_equal_1:
                    return "==";

                case operatororpunctuatorState.exclamation_equal_0:
                    return "!=";

                case operatororpunctuatorState.exclamation_equal_1:
                    return "!=";

                case operatororpunctuatorState.less_equal_0:
                    return "<=";

                case operatororpunctuatorState.less_equal_1:
                    return "<=";

                case operatororpunctuatorState.greater_equal_0:
                    return ">=";

                case operatororpunctuatorState.greater_equal_1:
                    return ">=";

                case operatororpunctuatorState.plus_equal_0:
                    return "+=";

                case operatororpunctuatorState.plus_equal_1:
                    return "+=";

                case operatororpunctuatorState.hyphen_equal_0:
                    return "-=";

                case operatororpunctuatorState.hyphen_equal_1:
                    return "-=";

                case operatororpunctuatorState.star_equal_0:
                    return "*=";

                case operatororpunctuatorState.star_equal_1:
                    return "*=";

                case operatororpunctuatorState.slash_equal_0:
                    return "/=";

                case operatororpunctuatorState.slash_equal_1:
                    return "/=";

                case operatororpunctuatorState.percent_equal_0:
                    return "%=";

                case operatororpunctuatorState.percent_equal_1:
                    return "%=";

                case operatororpunctuatorState.amp_equal_0:
                    return "&=";

                case operatororpunctuatorState.amp_equal_1:
                    return "&=";

                case operatororpunctuatorState.pipe_equal_0:
                    return "|=";

                case operatororpunctuatorState.pipe_equal_1:
                    return "|=";

                case operatororpunctuatorState.chevron_equal_0:
                    return "^=";

                case operatororpunctuatorState.chevron_equal_1:
                    return "^=";

                case operatororpunctuatorState.less_less_0:
                    return "<<";

                case operatororpunctuatorState.less_less_1:
                    return "<<";

                case operatororpunctuatorState.less_less_equal_0:
                    return "<<=";

                case operatororpunctuatorState.less_less_equal_1:
                    return "<<=";

                case operatororpunctuatorState.less_less_equal_2:
                    return "<<=";

                case operatororpunctuatorState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getright_shift(string input, ref int i)
        {
            return GetItem(input, ref i, true, "right-shift", (int)(rightshiftState.start), (int)(rightshiftState.end), this.GetValidNextStates_right_shift, this.GetStateTag_right_shift, this.GetSubSpan_right_shift);
        }

        public Int32[] GetValidNextStates_right_shift(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((rightshiftState)(currentState))
            {
                case rightshiftState.start:
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(rightshiftState.greater_0));
                    }
                    break;

                case rightshiftState.greater_0:
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(rightshiftState.greater_1));
                    }
                    break;

                case rightshiftState.greater_1:
                    linksToEnd = true;
                    break;

                case rightshiftState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_right_shift(string input, ref int i, int nextState)
        {
            switch ((rightshiftState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_right_shift(int state)
        {
            switch ((rightshiftState)(state))
            {
                case rightshiftState.start:
                    return "start";

                case rightshiftState.greater_0:
                    return ">";

                case rightshiftState.greater_1:
                    return ">";

                case rightshiftState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getright_shift_assignment(string input, ref int i)
        {
            return GetItem(input, ref i, true, "right-shift-assignment", (int)(rightshiftassignmentState.start), (int)(rightshiftassignmentState.end), this.GetValidNextStates_right_shift_assignment, this.GetStateTag_right_shift_assignment, this.GetSubSpan_right_shift_assignment);
        }

        public Int32[] GetValidNextStates_right_shift_assignment(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((rightshiftassignmentState)(currentState))
            {
                case rightshiftassignmentState.start:
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(rightshiftassignmentState.greater));
                    }
                    break;

                case rightshiftassignmentState.greater:
                    if (ch == '>')
                    {
                        validNextStates.Add((int)(rightshiftassignmentState.greater_equal_0));
                    }
                    break;

                case rightshiftassignmentState.greater_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(rightshiftassignmentState.greater_equal_1));
                    }
                    break;

                case rightshiftassignmentState.greater_equal_1:
                    linksToEnd = true;
                    break;

                case rightshiftassignmentState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_right_shift_assignment(string input, ref int i, int nextState)
        {
            switch ((rightshiftassignmentState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_right_shift_assignment(int state)
        {
            switch ((rightshiftassignmentState)(state))
            {
                case rightshiftassignmentState.start:
                    return "start";

                case rightshiftassignmentState.greater:
                    return ">";

                case rightshiftassignmentState.greater_equal_0:
                    return ">=";

                case rightshiftassignmentState.greater_equal_1:
                    return ">=";

                case rightshiftassignmentState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_directive(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-directive", (int)(ppdirectiveState.start), (int)(ppdirectiveState.end), this.GetValidNextStates_pp_directive, this.GetStateTag_pp_directive, this.GetSubSpan_pp_directive);
        }

        public Int32[] GetValidNextStates_pp_directive(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppdirectiveState)(currentState))
            {
                case ppdirectiveState.start:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_declaration));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_conditional));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_line));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_diagnostic));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_region));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdirectiveState.pp_pragma));
                    }
                    break;

                case ppdirectiveState.pp_declaration:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.pp_conditional:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.pp_line:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.pp_diagnostic:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.pp_region:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.pp_pragma:
                    linksToEnd = true;
                    break;

                case ppdirectiveState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_directive(string input, ref int i, int nextState)
        {
            switch ((ppdirectiveState)(nextState))
            {
                case ppdirectiveState.pp_declaration:
                    return Getpp_declaration(input, ref i);

                case ppdirectiveState.pp_conditional:
                    return Getpp_conditional(input, ref i);

                case ppdirectiveState.pp_line:
                    return Getpp_line(input, ref i);

                case ppdirectiveState.pp_diagnostic:
                    return Getpp_diagnostic(input, ref i);

                case ppdirectiveState.pp_region:
                    return Getpp_region(input, ref i);

                case ppdirectiveState.pp_pragma:
                    return Getpp_pragma(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_directive(int state)
        {
            switch ((ppdirectiveState)(state))
            {
                case ppdirectiveState.start:
                    return "start";

                case ppdirectiveState.pp_declaration:
                    return "pp-declaration";

                case ppdirectiveState.pp_conditional:
                    return "pp-conditional";

                case ppdirectiveState.pp_line:
                    return "pp-line";

                case ppdirectiveState.pp_diagnostic:
                    return "pp-diagnostic";

                case ppdirectiveState.pp_region:
                    return "pp-region";

                case ppdirectiveState.pp_pragma:
                    return "pp-pragma";

                case ppdirectiveState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getconditional_symbol(string input, ref int i)
        {
            return GetItem(input, ref i, true, "conditional-symbol", (int)(conditionalsymbolState.start), (int)(conditionalsymbolState.end), this.GetValidNextStates_conditional_symbol, this.GetStateTag_conditional_symbol, this.GetSubSpan_conditional_symbol);
        }

        public Int32[] GetValidNextStates_conditional_symbol(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((conditionalsymbolState)(currentState))
            {
                case conditionalsymbolState.start:
                    if (char.IsLetter(ch) || ch == '_' || ch == '@')
                    {
                        validNextStates.Add((int)(conditionalsymbolState.identifier));
                    }
                    break;

                case conditionalsymbolState.identifier:
                    linksToEnd = true;
                    break;

                case conditionalsymbolState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_conditional_symbol(string input, ref int i, int nextState)
        {
            switch ((conditionalsymbolState)(nextState))
            {
                case conditionalsymbolState.identifier:
                    return Getidentifier(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_conditional_symbol(int state)
        {
            switch ((conditionalsymbolState)(state))
            {
                case conditionalsymbolState.start:
                    return "start";

                case conditionalsymbolState.identifier:
                    return "identifier";

                case conditionalsymbolState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_expression(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-expression", (int)(ppexpressionState.start), (int)(ppexpressionState.end), this.GetValidNextStates_pp_expression, this.GetStateTag_pp_expression, this.GetSubSpan_pp_expression);
        }

        public Int32[] GetValidNextStates_pp_expression(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppexpressionState)(currentState))
            {
                case ppexpressionState.start:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_sub_expression_0));
                    }
                    break;

                case ppexpressionState.pp_sub_expression_0:
                    if (ch == '&' || ch == '|' || ch == '=' || ch == '!')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_operator));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppexpressionState.whitespace_0));
                    }
                    linksToEnd = true;
                    break;

                case ppexpressionState.pp_operator:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppexpressionState.whitespace_1));
                    }
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_sub_expression_1));
                    }
                    break;

                case ppexpressionState.whitespace_0:
                    if (ch == '&' || ch == '|' || ch == '=' || ch == '!')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_operator));
                    }
                    break;

                case ppexpressionState.end:
                    break;

                case ppexpressionState.whitespace_1:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_sub_expression_1));
                    }
                    break;

                case ppexpressionState.pp_sub_expression_1:
                    if (ch == '&' || ch == '|' || ch == '=' || ch == '!')
                    {
                        validNextStates.Add((int)(ppexpressionState.pp_operator));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppexpressionState.whitespace_0));
                    }
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_expression(string input, ref int i, int nextState)
        {
            switch ((ppexpressionState)(nextState))
            {
                case ppexpressionState.pp_sub_expression_0:
                    return Getpp_sub_expression(input, ref i);

                case ppexpressionState.pp_operator:
                    return Getpp_operator(input, ref i);

                case ppexpressionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppexpressionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppexpressionState.pp_sub_expression_1:
                    return Getpp_sub_expression(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_expression(int state)
        {
            switch ((ppexpressionState)(state))
            {
                case ppexpressionState.start:
                    return "start";

                case ppexpressionState.pp_sub_expression_0:
                    return "pp-sub-expression";

                case ppexpressionState.pp_operator:
                    return "pp-operator";

                case ppexpressionState.whitespace_0:
                    return "whitespace";

                case ppexpressionState.end:
                    return "end";

                case ppexpressionState.whitespace_1:
                    return "whitespace";

                case ppexpressionState.pp_sub_expression_1:
                    return "pp-sub-expression";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_sub_expression(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-sub-expression", (int)(ppsubexpressionState.start), (int)(ppsubexpressionState.end), this.GetValidNextStates_pp_sub_expression, this.GetStateTag_pp_sub_expression, this.GetSubSpan_pp_sub_expression);
        }

        public Int32[] GetValidNextStates_pp_sub_expression(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppsubexpressionState)(currentState))
            {
                case ppsubexpressionState.start:
                    if (ch == '!')
                    {
                        validNextStates.Add((int)(ppsubexpressionState.pp_unary_expression));
                    }
                    if (char.IsLetter(ch) || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppsubexpressionState.pp_primary_expression));
                    }
                    break;

                case ppsubexpressionState.pp_unary_expression:
                    linksToEnd = true;
                    break;

                case ppsubexpressionState.pp_primary_expression:
                    linksToEnd = true;
                    break;

                case ppsubexpressionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_sub_expression(string input, ref int i, int nextState)
        {
            switch ((ppsubexpressionState)(nextState))
            {
                case ppsubexpressionState.pp_unary_expression:
                    return Getpp_unary_expression(input, ref i);

                case ppsubexpressionState.pp_primary_expression:
                    return Getpp_primary_expression(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_sub_expression(int state)
        {
            switch ((ppsubexpressionState)(state))
            {
                case ppsubexpressionState.start:
                    return "start";

                case ppsubexpressionState.pp_unary_expression:
                    return "pp-unary-expression";

                case ppsubexpressionState.pp_primary_expression:
                    return "pp-primary-expression";

                case ppsubexpressionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_operator(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-operator", (int)(ppoperatorState.start), (int)(ppoperatorState.end), this.GetValidNextStates_pp_operator, this.GetStateTag_pp_operator, this.GetSubSpan_pp_operator);
        }

        public Int32[] GetValidNextStates_pp_operator(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppoperatorState)(currentState))
            {
                case ppoperatorState.start:
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(ppoperatorState.amp_amp_0));
                    }
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(ppoperatorState.pipe_pipe_0));
                    }
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(ppoperatorState.equal_equal_0));
                    }
                    if (ch == '!')
                    {
                        validNextStates.Add((int)(ppoperatorState.exclamation_equal_0));
                    }
                    break;

                case ppoperatorState.amp_amp_0:
                    if (ch == '&')
                    {
                        validNextStates.Add((int)(ppoperatorState.amp_amp_1));
                    }
                    break;

                case ppoperatorState.amp_amp_1:
                    linksToEnd = true;
                    break;

                case ppoperatorState.pipe_pipe_0:
                    if (ch == '|')
                    {
                        validNextStates.Add((int)(ppoperatorState.pipe_pipe_1));
                    }
                    break;

                case ppoperatorState.pipe_pipe_1:
                    linksToEnd = true;
                    break;

                case ppoperatorState.equal_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(ppoperatorState.equal_equal_1));
                    }
                    break;

                case ppoperatorState.equal_equal_1:
                    linksToEnd = true;
                    break;

                case ppoperatorState.exclamation_equal_0:
                    if (ch == '=')
                    {
                        validNextStates.Add((int)(ppoperatorState.exclamation_equal_1));
                    }
                    break;

                case ppoperatorState.exclamation_equal_1:
                    linksToEnd = true;
                    break;

                case ppoperatorState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_operator(string input, ref int i, int nextState)
        {
            switch ((ppoperatorState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_operator(int state)
        {
            switch ((ppoperatorState)(state))
            {
                case ppoperatorState.start:
                    return "start";

                case ppoperatorState.amp_amp_0:
                    return "&&";

                case ppoperatorState.amp_amp_1:
                    return "&&";

                case ppoperatorState.pipe_pipe_0:
                    return "||";

                case ppoperatorState.pipe_pipe_1:
                    return "||";

                case ppoperatorState.equal_equal_0:
                    return "==";

                case ppoperatorState.equal_equal_1:
                    return "==";

                case ppoperatorState.exclamation_equal_0:
                    return "!=";

                case ppoperatorState.exclamation_equal_1:
                    return "!=";

                case ppoperatorState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_unary_expression(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-unary-expression", (int)(ppunaryexpressionState.start), (int)(ppunaryexpressionState.end), this.GetValidNextStates_pp_unary_expression, this.GetStateTag_pp_unary_expression, this.GetSubSpan_pp_unary_expression);
        }

        public Int32[] GetValidNextStates_pp_unary_expression(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppunaryexpressionState)(currentState))
            {
                case ppunaryexpressionState.start:
                    if (ch == '!')
                    {
                        validNextStates.Add((int)(ppunaryexpressionState.exclamation));
                    }
                    break;

                case ppunaryexpressionState.exclamation:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppunaryexpressionState.whitespace));
                    }
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppunaryexpressionState.pp_sub_expression));
                    }
                    break;

                case ppunaryexpressionState.whitespace:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppunaryexpressionState.pp_sub_expression));
                    }
                    break;

                case ppunaryexpressionState.pp_sub_expression:
                    linksToEnd = true;
                    break;

                case ppunaryexpressionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_unary_expression(string input, ref int i, int nextState)
        {
            switch ((ppunaryexpressionState)(nextState))
            {
                case ppunaryexpressionState.whitespace:
                    return Getwhitespace(input, ref i);

                case ppunaryexpressionState.pp_sub_expression:
                    return Getpp_sub_expression(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_unary_expression(int state)
        {
            switch ((ppunaryexpressionState)(state))
            {
                case ppunaryexpressionState.start:
                    return "start";

                case ppunaryexpressionState.exclamation:
                    return "!";

                case ppunaryexpressionState.whitespace:
                    return "whitespace";

                case ppunaryexpressionState.pp_sub_expression:
                    return "pp-sub-expression";

                case ppunaryexpressionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_primary_expression(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-primary-expression", (int)(ppprimaryexpressionState.start), (int)(ppprimaryexpressionState.end), this.GetValidNextStates_pp_primary_expression, this.GetStateTag_pp_primary_expression, this.GetSubSpan_pp_primary_expression);
        }

        public Int32[] GetValidNextStates_pp_primary_expression(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppprimaryexpressionState)(currentState))
            {
                case ppprimaryexpressionState.start:
                    if (ch == 't')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.true_0));
                    }
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.false_0));
                    }
                    if (char.IsLetter(ch) || ch == '_' || ch == '@')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.conditional_symbol));
                    }
                    if (ch == '(')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.oparen));
                    }
                    break;

                case ppprimaryexpressionState.true_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.true_1));
                    }
                    break;

                case ppprimaryexpressionState.true_1:
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.true_2));
                    }
                    break;

                case ppprimaryexpressionState.true_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.true_3));
                    }
                    break;

                case ppprimaryexpressionState.true_3:
                    linksToEnd = true;
                    break;

                case ppprimaryexpressionState.false_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.false_1));
                    }
                    break;

                case ppprimaryexpressionState.false_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.false_2));
                    }
                    break;

                case ppprimaryexpressionState.false_2:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.false_3));
                    }
                    break;

                case ppprimaryexpressionState.false_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.false_4));
                    }
                    break;

                case ppprimaryexpressionState.false_4:
                    linksToEnd = true;
                    break;

                case ppprimaryexpressionState.conditional_symbol:
                    linksToEnd = true;
                    break;

                case ppprimaryexpressionState.oparen:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.whitespace_0));
                    }
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.pp_expression));
                    }
                    break;

                case ppprimaryexpressionState.end:
                    break;

                case ppprimaryexpressionState.whitespace_0:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.pp_expression));
                    }
                    break;

                case ppprimaryexpressionState.pp_expression:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.whitespace_1));
                    }
                    if (ch == ')')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.cparen));
                    }
                    break;

                case ppprimaryexpressionState.whitespace_1:
                    if (ch == ')')
                    {
                        validNextStates.Add((int)(ppprimaryexpressionState.cparen));
                    }
                    break;

                case ppprimaryexpressionState.cparen:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_primary_expression(string input, ref int i, int nextState)
        {
            switch ((ppprimaryexpressionState)(nextState))
            {
                case ppprimaryexpressionState.conditional_symbol:
                    return Getconditional_symbol(input, ref i);

                case ppprimaryexpressionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppprimaryexpressionState.pp_expression:
                    return Getpp_expression(input, ref i);

                case ppprimaryexpressionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_primary_expression(int state)
        {
            switch ((ppprimaryexpressionState)(state))
            {
                case ppprimaryexpressionState.start:
                    return "start";

                case ppprimaryexpressionState.true_0:
                    return "true";

                case ppprimaryexpressionState.true_1:
                    return "true";

                case ppprimaryexpressionState.true_2:
                    return "true";

                case ppprimaryexpressionState.true_3:
                    return "true";

                case ppprimaryexpressionState.false_0:
                    return "false";

                case ppprimaryexpressionState.false_1:
                    return "false";

                case ppprimaryexpressionState.false_2:
                    return "false";

                case ppprimaryexpressionState.false_3:
                    return "false";

                case ppprimaryexpressionState.false_4:
                    return "false";

                case ppprimaryexpressionState.conditional_symbol:
                    return "conditional-symbol";

                case ppprimaryexpressionState.oparen:
                    return "(";

                case ppprimaryexpressionState.end:
                    return "end";

                case ppprimaryexpressionState.whitespace_0:
                    return "whitespace";

                case ppprimaryexpressionState.pp_expression:
                    return "pp-expression";

                case ppprimaryexpressionState.whitespace_1:
                    return "whitespace";

                case ppprimaryexpressionState.cparen:
                    return ")";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_declaration(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-declaration", (int)(ppdeclarationState.start), (int)(ppdeclarationState.end), this.GetValidNextStates_pp_declaration, this.GetStateTag_pp_declaration, this.GetSubSpan_pp_declaration);
        }

        public Int32[] GetValidNextStates_pp_declaration(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppdeclarationState)(currentState))
            {
                case ppdeclarationState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppdeclarationState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdeclarationState.whitespace_0));
                    }
                    break;

                case ppdeclarationState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdeclarationState.whitespace_1));
                    }
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_0));
                    }
                    break;

                case ppdeclarationState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppdeclarationState.hash));
                    }
                    break;

                case ppdeclarationState.whitespace_1:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_0));
                    }
                    if (ch == 'u')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_0));
                    }
                    break;

                case ppdeclarationState.define_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_1));
                    }
                    break;

                case ppdeclarationState.define_1:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_2));
                    }
                    break;

                case ppdeclarationState.define_2:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_3));
                    }
                    break;

                case ppdeclarationState.define_3:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_4));
                    }
                    break;

                case ppdeclarationState.define_4:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppdeclarationState.define_5));
                    }
                    break;

                case ppdeclarationState.define_5:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdeclarationState.whitespace_2));
                    }
                    break;

                case ppdeclarationState.undef_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_1));
                    }
                    break;

                case ppdeclarationState.undef_1:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_2));
                    }
                    break;

                case ppdeclarationState.undef_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_3));
                    }
                    break;

                case ppdeclarationState.undef_3:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppdeclarationState.undef_4));
                    }
                    break;

                case ppdeclarationState.undef_4:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdeclarationState.whitespace_2));
                    }
                    break;

                case ppdeclarationState.whitespace_2:
                    if (char.IsLetter(ch) || ch == '_' || ch == '@')
                    {
                        validNextStates.Add((int)(ppdeclarationState.conditional_symbol));
                    }
                    break;

                case ppdeclarationState.conditional_symbol:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdeclarationState.pp_new_line));
                    }
                    break;

                case ppdeclarationState.pp_new_line:
                    linksToEnd = true;
                    break;

                case ppdeclarationState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_declaration(string input, ref int i, int nextState)
        {
            switch ((ppdeclarationState)(nextState))
            {
                case ppdeclarationState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppdeclarationState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppdeclarationState.whitespace_2:
                    return Getwhitespace(input, ref i);

                case ppdeclarationState.conditional_symbol:
                    return Getconditional_symbol(input, ref i);

                case ppdeclarationState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_declaration(int state)
        {
            switch ((ppdeclarationState)(state))
            {
                case ppdeclarationState.start:
                    return "start";

                case ppdeclarationState.hash:
                    return "#";

                case ppdeclarationState.whitespace_0:
                    return "whitespace";

                case ppdeclarationState.whitespace_1:
                    return "whitespace";

                case ppdeclarationState.define_0:
                    return "define";

                case ppdeclarationState.define_1:
                    return "define";

                case ppdeclarationState.define_2:
                    return "define";

                case ppdeclarationState.define_3:
                    return "define";

                case ppdeclarationState.define_4:
                    return "define";

                case ppdeclarationState.define_5:
                    return "define";

                case ppdeclarationState.undef_0:
                    return "undef";

                case ppdeclarationState.undef_1:
                    return "undef";

                case ppdeclarationState.undef_2:
                    return "undef";

                case ppdeclarationState.undef_3:
                    return "undef";

                case ppdeclarationState.undef_4:
                    return "undef";

                case ppdeclarationState.whitespace_2:
                    return "whitespace";

                case ppdeclarationState.conditional_symbol:
                    return "conditional-symbol";

                case ppdeclarationState.pp_new_line:
                    return "pp-new-line";

                case ppdeclarationState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_new_line(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-new-line", (int)(ppnewlineState.start), (int)(ppnewlineState.end), this.GetValidNextStates_pp_new_line, this.GetStateTag_pp_new_line, this.GetSubSpan_pp_new_line);
        }

        public Int32[] GetValidNextStates_pp_new_line(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppnewlineState)(currentState))
            {
                case ppnewlineState.start:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(ppnewlineState.single_line_comment));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppnewlineState.new_line));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppnewlineState.whitespace));
                    }
                    break;

                case ppnewlineState.single_line_comment:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppnewlineState.new_line));
                    }
                    break;

                case ppnewlineState.new_line:
                    linksToEnd = true;
                    break;

                case ppnewlineState.whitespace:
                    if (ch == '/')
                    {
                        validNextStates.Add((int)(ppnewlineState.single_line_comment));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppnewlineState.new_line));
                    }
                    break;

                case ppnewlineState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_new_line(string input, ref int i, int nextState)
        {
            switch ((ppnewlineState)(nextState))
            {
                case ppnewlineState.single_line_comment:
                    return Getsingle_line_comment(input, ref i);

                case ppnewlineState.new_line:
                    return Getnew_line(input, ref i);

                case ppnewlineState.whitespace:
                    return Getwhitespace(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_new_line(int state)
        {
            switch ((ppnewlineState)(state))
            {
                case ppnewlineState.start:
                    return "start";

                case ppnewlineState.single_line_comment:
                    return "single-line-comment";

                case ppnewlineState.new_line:
                    return "new-line";

                case ppnewlineState.whitespace:
                    return "whitespace";

                case ppnewlineState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_conditional(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-conditional", (int)(ppconditionalState.start), (int)(ppconditionalState.end), this.GetValidNextStates_pp_conditional, this.GetStateTag_pp_conditional, this.GetSubSpan_pp_conditional);
        }

        public Int32[] GetValidNextStates_pp_conditional(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppconditionalState)(currentState))
            {
                case ppconditionalState.start:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_if_section));
                    }
                    break;

                case ppconditionalState.pp_if_section:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_elif_sections));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_else_section));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_endif));
                    }
                    break;

                case ppconditionalState.pp_elif_sections:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_else_section));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_endif));
                    }
                    break;

                case ppconditionalState.pp_else_section:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_else_section));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppconditionalState.pp_endif));
                    }
                    break;

                case ppconditionalState.pp_endif:
                    linksToEnd = true;
                    break;

                case ppconditionalState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_conditional(string input, ref int i, int nextState)
        {
            switch ((ppconditionalState)(nextState))
            {
                case ppconditionalState.pp_if_section:
                    return Getpp_if_section(input, ref i);

                case ppconditionalState.pp_elif_sections:
                    return Getpp_elif_sections(input, ref i);

                case ppconditionalState.pp_else_section:
                    return Getpp_else_section(input, ref i);

                case ppconditionalState.pp_endif:
                    return Getpp_endif(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_conditional(int state)
        {
            switch ((ppconditionalState)(state))
            {
                case ppconditionalState.start:
                    return "start";

                case ppconditionalState.pp_if_section:
                    return "pp-if-section";

                case ppconditionalState.pp_elif_sections:
                    return "pp-elif-sections";

                case ppconditionalState.pp_else_section:
                    return "pp-else-section";

                case ppconditionalState.pp_endif:
                    return "pp-endif";

                case ppconditionalState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_if_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-if-section", (int)(ppifsectionState.start), (int)(ppifsectionState.end), this.GetValidNextStates_pp_if_section, this.GetStateTag_pp_if_section, this.GetSubSpan_pp_if_section);
        }

        public Int32[] GetValidNextStates_pp_if_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppifsectionState)(currentState))
            {
                case ppifsectionState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppifsectionState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppifsectionState.whitespace_0));
                    }
                    break;

                case ppifsectionState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppifsectionState.whitespace_1));
                    }
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppifsectionState.if_0));
                    }
                    break;

                case ppifsectionState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppifsectionState.hash));
                    }
                    break;

                case ppifsectionState.whitespace_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppifsectionState.if_0));
                    }
                    break;

                case ppifsectionState.if_0:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppifsectionState.if_1));
                    }
                    break;

                case ppifsectionState.if_1:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppifsectionState.whitespace_2));
                    }
                    break;

                case ppifsectionState.whitespace_2:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppifsectionState.pp_expression));
                    }
                    break;

                case ppifsectionState.pp_expression:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppifsectionState.pp_new_line));
                    }
                    break;

                case ppifsectionState.pp_new_line:
                    if (false)
                    {
                        validNextStates.Add((int)(ppifsectionState.conditional_section));
                    }
                    linksToEnd = true;
                    break;

                case ppifsectionState.conditional_section:
                    linksToEnd = true;
                    break;

                case ppifsectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_if_section(string input, ref int i, int nextState)
        {
            switch ((ppifsectionState)(nextState))
            {
                case ppifsectionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppifsectionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppifsectionState.whitespace_2:
                    return Getwhitespace(input, ref i);

                case ppifsectionState.pp_expression:
                    return Getpp_expression(input, ref i);

                case ppifsectionState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                case ppifsectionState.conditional_section:
                    return Getconditional_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_if_section(int state)
        {
            switch ((ppifsectionState)(state))
            {
                case ppifsectionState.start:
                    return "start";

                case ppifsectionState.hash:
                    return "#";

                case ppifsectionState.whitespace_0:
                    return "whitespace";

                case ppifsectionState.whitespace_1:
                    return "whitespace";

                case ppifsectionState.if_0:
                    return "if";

                case ppifsectionState.if_1:
                    return "if";

                case ppifsectionState.whitespace_2:
                    return "whitespace";

                case ppifsectionState.pp_expression:
                    return "pp-expression";

                case ppifsectionState.pp_new_line:
                    return "pp-new-line";

                case ppifsectionState.conditional_section:
                    return "conditional-section";

                case ppifsectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_elif_sections(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-elif-sections", (int)(ppelifsectionsState.start), (int)(ppelifsectionsState.end), this.GetValidNextStates_pp_elif_sections, this.GetStateTag_pp_elif_sections, this.GetSubSpan_pp_elif_sections);
        }

        public Int32[] GetValidNextStates_pp_elif_sections(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppelifsectionsState)(currentState))
            {
                case ppelifsectionsState.start:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionsState.pp_elif_section));
                    }
                    break;

                case ppelifsectionsState.pp_elif_section:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionsState.pp_elif_section));
                    }
                    linksToEnd = true;
                    break;

                case ppelifsectionsState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_elif_sections(string input, ref int i, int nextState)
        {
            switch ((ppelifsectionsState)(nextState))
            {
                case ppelifsectionsState.pp_elif_section:
                    return Getpp_elif_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_elif_sections(int state)
        {
            switch ((ppelifsectionsState)(state))
            {
                case ppelifsectionsState.start:
                    return "start";

                case ppelifsectionsState.pp_elif_section:
                    return "pp-elif-section";

                case ppelifsectionsState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_elif_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-elif-section", (int)(ppelifsectionState.start), (int)(ppelifsectionState.end), this.GetValidNextStates_pp_elif_section, this.GetStateTag_pp_elif_section, this.GetSubSpan_pp_elif_section);
        }

        public Int32[] GetValidNextStates_pp_elif_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppelifsectionState)(currentState))
            {
                case ppelifsectionState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppelifsectionState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionState.whitespace_0));
                    }
                    break;

                case ppelifsectionState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionState.whitespace_1));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppelifsectionState.elif_0));
                    }
                    break;

                case ppelifsectionState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppelifsectionState.hash));
                    }
                    break;

                case ppelifsectionState.whitespace_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppelifsectionState.elif_0));
                    }
                    break;

                case ppelifsectionState.elif_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(ppelifsectionState.elif_1));
                    }
                    break;

                case ppelifsectionState.elif_1:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppelifsectionState.elif_2));
                    }
                    break;

                case ppelifsectionState.elif_2:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppelifsectionState.elif_3));
                    }
                    break;

                case ppelifsectionState.elif_3:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionState.whitespace_2));
                    }
                    break;

                case ppelifsectionState.whitespace_2:
                    if (char.IsLetter(ch) || ch == '!' || ch == '_' || ch == '@' || ch == '(')
                    {
                        validNextStates.Add((int)(ppelifsectionState.pp_expression));
                    }
                    break;

                case ppelifsectionState.pp_expression:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelifsectionState.pp_new_line));
                    }
                    break;

                case ppelifsectionState.pp_new_line:
                    if (false)
                    {
                        validNextStates.Add((int)(ppelifsectionState.conditional_section));
                    }
                    linksToEnd = true;
                    break;

                case ppelifsectionState.conditional_section:
                    linksToEnd = true;
                    break;

                case ppelifsectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_elif_section(string input, ref int i, int nextState)
        {
            switch ((ppelifsectionState)(nextState))
            {
                case ppelifsectionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppelifsectionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppelifsectionState.whitespace_2:
                    return Getwhitespace(input, ref i);

                case ppelifsectionState.pp_expression:
                    return Getpp_expression(input, ref i);

                case ppelifsectionState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                case ppelifsectionState.conditional_section:
                    return Getconditional_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_elif_section(int state)
        {
            switch ((ppelifsectionState)(state))
            {
                case ppelifsectionState.start:
                    return "start";

                case ppelifsectionState.hash:
                    return "#";

                case ppelifsectionState.whitespace_0:
                    return "whitespace";

                case ppelifsectionState.whitespace_1:
                    return "whitespace";

                case ppelifsectionState.elif_0:
                    return "elif";

                case ppelifsectionState.elif_1:
                    return "elif";

                case ppelifsectionState.elif_2:
                    return "elif";

                case ppelifsectionState.elif_3:
                    return "elif";

                case ppelifsectionState.whitespace_2:
                    return "whitespace";

                case ppelifsectionState.pp_expression:
                    return "pp-expression";

                case ppelifsectionState.pp_new_line:
                    return "pp-new-line";

                case ppelifsectionState.conditional_section:
                    return "conditional-section";

                case ppelifsectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_else_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-else-section", (int)(ppelsesectionState.start), (int)(ppelsesectionState.end), this.GetValidNextStates_pp_else_section, this.GetStateTag_pp_else_section, this.GetSubSpan_pp_else_section);
        }

        public Int32[] GetValidNextStates_pp_else_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppelsesectionState)(currentState))
            {
                case ppelsesectionState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppelsesectionState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelsesectionState.whitespace_0));
                    }
                    break;

                case ppelsesectionState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelsesectionState.whitespace_1));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppelsesectionState.else_0));
                    }
                    break;

                case ppelsesectionState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppelsesectionState.hash));
                    }
                    break;

                case ppelsesectionState.whitespace_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppelsesectionState.else_0));
                    }
                    break;

                case ppelsesectionState.else_0:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(ppelsesectionState.else_1));
                    }
                    break;

                case ppelsesectionState.else_1:
                    if (ch == 's')
                    {
                        validNextStates.Add((int)(ppelsesectionState.else_2));
                    }
                    break;

                case ppelsesectionState.else_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppelsesectionState.else_3));
                    }
                    break;

                case ppelsesectionState.else_3:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppelsesectionState.pp_new_line));
                    }
                    break;

                case ppelsesectionState.pp_new_line:
                    if (false)
                    {
                        validNextStates.Add((int)(ppelsesectionState.conditional_section));
                    }
                    linksToEnd = true;
                    break;

                case ppelsesectionState.conditional_section:
                    linksToEnd = true;
                    break;

                case ppelsesectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_else_section(string input, ref int i, int nextState)
        {
            switch ((ppelsesectionState)(nextState))
            {
                case ppelsesectionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppelsesectionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppelsesectionState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                case ppelsesectionState.conditional_section:
                    return Getconditional_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_else_section(int state)
        {
            switch ((ppelsesectionState)(state))
            {
                case ppelsesectionState.start:
                    return "start";

                case ppelsesectionState.hash:
                    return "#";

                case ppelsesectionState.whitespace_0:
                    return "whitespace";

                case ppelsesectionState.whitespace_1:
                    return "whitespace";

                case ppelsesectionState.else_0:
                    return "else";

                case ppelsesectionState.else_1:
                    return "else";

                case ppelsesectionState.else_2:
                    return "else";

                case ppelsesectionState.else_3:
                    return "else";

                case ppelsesectionState.pp_new_line:
                    return "pp-new-line";

                case ppelsesectionState.conditional_section:
                    return "conditional-section";

                case ppelsesectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_endif(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-endif", (int)(ppendifState.start), (int)(ppendifState.end), this.GetValidNextStates_pp_endif, this.GetStateTag_pp_endif, this.GetSubSpan_pp_endif);
        }

        public Int32[] GetValidNextStates_pp_endif(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppendifState)(currentState))
            {
                case ppendifState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppendifState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendifState.whitespace_0));
                    }
                    break;

                case ppendifState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendifState.whitespace_1));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppendifState.endif_0));
                    }
                    break;

                case ppendifState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppendifState.hash));
                    }
                    break;

                case ppendifState.whitespace_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppendifState.endif_0));
                    }
                    break;

                case ppendifState.endif_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppendifState.endif_1));
                    }
                    break;

                case ppendifState.endif_1:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(ppendifState.endif_2));
                    }
                    break;

                case ppendifState.endif_2:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppendifState.endif_3));
                    }
                    break;

                case ppendifState.endif_3:
                    if (ch == 'f')
                    {
                        validNextStates.Add((int)(ppendifState.endif_4));
                    }
                    break;

                case ppendifState.endif_4:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendifState.pp_new_line));
                    }
                    break;

                case ppendifState.pp_new_line:
                    linksToEnd = true;
                    break;

                case ppendifState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_endif(string input, ref int i, int nextState)
        {
            switch ((ppendifState)(nextState))
            {
                case ppendifState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppendifState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppendifState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_endif(int state)
        {
            switch ((ppendifState)(state))
            {
                case ppendifState.start:
                    return "start";

                case ppendifState.hash:
                    return "#";

                case ppendifState.whitespace_0:
                    return "whitespace";

                case ppendifState.whitespace_1:
                    return "whitespace";

                case ppendifState.endif_0:
                    return "endif";

                case ppendifState.endif_1:
                    return "endif";

                case ppendifState.endif_2:
                    return "endif";

                case ppendifState.endif_3:
                    return "endif";

                case ppendifState.endif_4:
                    return "endif";

                case ppendifState.pp_new_line:
                    return "pp-new-line";

                case ppendifState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getconditional_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "conditional-section", (int)(conditionalsectionState.start), (int)(conditionalsectionState.end), this.GetValidNextStates_conditional_section, this.GetStateTag_conditional_section, this.GetSubSpan_conditional_section);
        }

        public Int32[] GetValidNextStates_conditional_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((conditionalsectionState)(currentState))
            {
                case conditionalsectionState.start:
                    if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '\t' || ch == '/' || ch == '_' || ch == '@' || ch == '.' || ch == '\'' || ch == '"' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == ',' || ch == ':' || ch == ';' || ch == '+' || ch == '-' || ch == '*' || ch == '%' || ch == '&' || ch == '|' || ch == '^' || ch == '!' || ch == '~' || ch == '=' || ch == '<' || ch == '>' || ch == '?' || ch == '#')
                    {
                        validNextStates.Add((int)(conditionalsectionState.input_section));
                    }
                    if (false)
                    {
                        validNextStates.Add((int)(conditionalsectionState.skipped_section));
                    }
                    break;

                case conditionalsectionState.input_section:
                    linksToEnd = true;
                    break;

                case conditionalsectionState.skipped_section:
                    linksToEnd = true;
                    break;

                case conditionalsectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_conditional_section(string input, ref int i, int nextState)
        {
            switch ((conditionalsectionState)(nextState))
            {
                case conditionalsectionState.input_section:
                    return Getinput_section(input, ref i);

                case conditionalsectionState.skipped_section:
                    return Getskipped_section(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_conditional_section(int state)
        {
            switch ((conditionalsectionState)(state))
            {
                case conditionalsectionState.start:
                    return "start";

                case conditionalsectionState.input_section:
                    return "input-section";

                case conditionalsectionState.skipped_section:
                    return "skipped-section";

                case conditionalsectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getskipped_section(string input, ref int i)
        {
            return GetItem(input, ref i, true, "skipped-section", (int)(skippedsectionState.start), (int)(skippedsectionState.end), this.GetValidNextStates_skipped_section, this.GetStateTag_skipped_section, this.GetSubSpan_skipped_section);
        }

        public Int32[] GetValidNextStates_skipped_section(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((skippedsectionState)(currentState))
            {
                case skippedsectionState.start:
                    if (false)
                    {
                        validNextStates.Add((int)(skippedsectionState.skipped_section_part));
                    }
                    break;

                case skippedsectionState.skipped_section_part:
                    if (false)
                    {
                        validNextStates.Add((int)(skippedsectionState.skipped_section_part));
                    }
                    linksToEnd = true;
                    break;

                case skippedsectionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_skipped_section(string input, ref int i, int nextState)
        {
            switch ((skippedsectionState)(nextState))
            {
                case skippedsectionState.skipped_section_part:
                    return Getskipped_section_part(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_skipped_section(int state)
        {
            switch ((skippedsectionState)(state))
            {
                case skippedsectionState.start:
                    return "start";

                case skippedsectionState.skipped_section_part:
                    return "skipped-section-part";

                case skippedsectionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getskipped_section_part(string input, ref int i)
        {
            return GetItem(input, ref i, true, "skipped-section-part", (int)(skippedsectionpartState.start), (int)(skippedsectionpartState.end), this.GetValidNextStates_skipped_section_part, this.GetStateTag_skipped_section_part, this.GetSubSpan_skipped_section_part);
        }

        public Int32[] GetValidNextStates_skipped_section_part(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((skippedsectionpartState)(currentState))
            {
                case skippedsectionpartState.start:
                    if (!(ch == '#'))
                    {
                        validNextStates.Add((int)(skippedsectionpartState.skipped_characters));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(skippedsectionpartState.new_line));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(skippedsectionpartState.whitespace));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(skippedsectionpartState.pp_directive));
                    }
                    break;

                case skippedsectionpartState.skipped_characters:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(skippedsectionpartState.new_line));
                    }
                    break;

                case skippedsectionpartState.new_line:
                    linksToEnd = true;
                    break;

                case skippedsectionpartState.whitespace:
                    if (!(ch == '#'))
                    {
                        validNextStates.Add((int)(skippedsectionpartState.skipped_characters));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(skippedsectionpartState.new_line));
                    }
                    break;

                case skippedsectionpartState.pp_directive:
                    linksToEnd = true;
                    break;

                case skippedsectionpartState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_skipped_section_part(string input, ref int i, int nextState)
        {
            switch ((skippedsectionpartState)(nextState))
            {
                case skippedsectionpartState.skipped_characters:
                    return Getskipped_characters(input, ref i);

                case skippedsectionpartState.new_line:
                    return Getnew_line(input, ref i);

                case skippedsectionpartState.whitespace:
                    return Getwhitespace(input, ref i);

                case skippedsectionpartState.pp_directive:
                    return Getpp_directive(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_skipped_section_part(int state)
        {
            switch ((skippedsectionpartState)(state))
            {
                case skippedsectionpartState.start:
                    return "start";

                case skippedsectionpartState.skipped_characters:
                    return "skipped-characters";

                case skippedsectionpartState.new_line:
                    return "new-line";

                case skippedsectionpartState.whitespace:
                    return "whitespace";

                case skippedsectionpartState.pp_directive:
                    return "pp-directive";

                case skippedsectionpartState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getskipped_characters(string input, ref int i)
        {
            return GetItem(input, ref i, true, "skipped-characters", (int)(skippedcharactersState.start), (int)(skippedcharactersState.end), this.GetValidNextStates_skipped_characters, this.GetStateTag_skipped_characters, this.GetSubSpan_skipped_characters);
        }

        public Int32[] GetValidNextStates_skipped_characters(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((skippedcharactersState)(currentState))
            {
                case skippedcharactersState.start:
                    if (!(ch == '#'))
                    {
                        validNextStates.Add((int)(skippedcharactersState.chevron_hash));
                    }
                    break;

                case skippedcharactersState.chevron_hash:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(skippedcharactersState.input_characters));
                    }
                    linksToEnd = true;
                    break;

                case skippedcharactersState.input_characters:
                    linksToEnd = true;
                    break;

                case skippedcharactersState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_skipped_characters(string input, ref int i, int nextState)
        {
            switch ((skippedcharactersState)(nextState))
            {
                case skippedcharactersState.input_characters:
                    return Getinput_characters(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_skipped_characters(int state)
        {
            switch ((skippedcharactersState)(state))
            {
                case skippedcharactersState.start:
                    return "start";

                case skippedcharactersState.chevron_hash:
                    return "^#";

                case skippedcharactersState.input_characters:
                    return "input-characters";

                case skippedcharactersState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_line(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-line", (int)(pplineState.start), (int)(pplineState.end), this.GetValidNextStates_pp_line, this.GetStateTag_pp_line, this.GetSubSpan_pp_line);
        }

        public Int32[] GetValidNextStates_pp_line(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((pplineState)(currentState))
            {
                case pplineState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(pplineState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pplineState.whitespace_0));
                    }
                    break;

                case pplineState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pplineState.whitespace_1));
                    }
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(pplineState.line_0));
                    }
                    break;

                case pplineState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(pplineState.hash));
                    }
                    break;

                case pplineState.whitespace_1:
                    if (ch == 'l')
                    {
                        validNextStates.Add((int)(pplineState.line_0));
                    }
                    break;

                case pplineState.line_0:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(pplineState.line_1));
                    }
                    break;

                case pplineState.line_1:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(pplineState.line_2));
                    }
                    break;

                case pplineState.line_2:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(pplineState.line_3));
                    }
                    break;

                case pplineState.line_3:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pplineState.whitespace_2));
                    }
                    break;

                case pplineState.whitespace_2:
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(pplineState.line_indicator));
                    }
                    break;

                case pplineState.line_indicator:
                    if (ch == '/' || ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pplineState.pp_new_line));
                    }
                    break;

                case pplineState.pp_new_line:
                    linksToEnd = true;
                    break;

                case pplineState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_line(string input, ref int i, int nextState)
        {
            switch ((pplineState)(nextState))
            {
                case pplineState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case pplineState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case pplineState.whitespace_2:
                    return Getwhitespace(input, ref i);

                case pplineState.line_indicator:
                    return Getline_indicator(input, ref i);

                case pplineState.pp_new_line:
                    return Getpp_new_line(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_line(int state)
        {
            switch ((pplineState)(state))
            {
                case pplineState.start:
                    return "start";

                case pplineState.hash:
                    return "#";

                case pplineState.whitespace_0:
                    return "whitespace";

                case pplineState.whitespace_1:
                    return "whitespace";

                case pplineState.line_0:
                    return "line";

                case pplineState.line_1:
                    return "line";

                case pplineState.line_2:
                    return "line";

                case pplineState.line_3:
                    return "line";

                case pplineState.whitespace_2:
                    return "whitespace";

                case pplineState.line_indicator:
                    return "line-indicator";

                case pplineState.pp_new_line:
                    return "pp-new-line";

                case pplineState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getline_indicator(string input, ref int i)
        {
            return GetItem(input, ref i, true, "line-indicator", (int)(lineindicatorState.start), (int)(lineindicatorState.end), this.GetValidNextStates_line_indicator, this.GetStateTag_line_indicator, this.GetSubSpan_line_indicator);
        }

        public Int32[] GetValidNextStates_line_indicator(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((lineindicatorState)(currentState))
            {
                case lineindicatorState.start:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(lineindicatorState.class_digit_0));
                    }
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(lineindicatorState.class_digit_1));
                    }
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        validNextStates.Add((int)(lineindicatorState.identifier_or_keyword));
                    }
                    break;

                case lineindicatorState.class_digit_0:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(lineindicatorState.class_digit_0));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(lineindicatorState.whitespace));
                    }
                    break;

                case lineindicatorState.class_digit_1:
                    if (char.IsDigit(ch))
                    {
                        validNextStates.Add((int)(lineindicatorState.class_digit_1));
                    }
                    linksToEnd = true;
                    break;

                case lineindicatorState.identifier_or_keyword:
                    linksToEnd = true;
                    break;

                case lineindicatorState.whitespace:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(lineindicatorState.file_name));
                    }
                    break;

                case lineindicatorState.end:
                    break;

                case lineindicatorState.file_name:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_line_indicator(string input, ref int i, int nextState)
        {
            switch ((lineindicatorState)(nextState))
            {
                case lineindicatorState.identifier_or_keyword:
                    return Getidentifier_or_keyword(input, ref i);

                case lineindicatorState.whitespace:
                    return Getwhitespace(input, ref i);

                case lineindicatorState.file_name:
                    return Getfile_name(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_line_indicator(int state)
        {
            switch ((lineindicatorState)(state))
            {
                case lineindicatorState.start:
                    return "start";

                case lineindicatorState.class_digit_0:
                    return "\\d";

                case lineindicatorState.class_digit_1:
                    return "\\d";

                case lineindicatorState.identifier_or_keyword:
                    return "identifier-or-keyword";

                case lineindicatorState.whitespace:
                    return "whitespace";

                case lineindicatorState.end:
                    return "end";

                case lineindicatorState.file_name:
                    return "file-name";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getfile_name(string input, ref int i)
        {
            return GetItem(input, ref i, true, "file-name", (int)(filenameState.start), (int)(filenameState.end), this.GetValidNextStates_file_name, this.GetStateTag_file_name, this.GetSubSpan_file_name);
        }

        public Int32[] GetValidNextStates_file_name(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((filenameState)(currentState))
            {
                case filenameState.start:
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(filenameState.dquote_0));
                    }
                    break;

                case filenameState.dquote_0:
                    if (!(ch == '"' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(filenameState.file_name_character));
                    }
                    break;

                case filenameState.file_name_character:
                    if (!(ch == '"' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(filenameState.file_name_character));
                    }
                    if (ch == '"')
                    {
                        validNextStates.Add((int)(filenameState.dquote_1));
                    }
                    break;

                case filenameState.dquote_1:
                    linksToEnd = true;
                    break;

                case filenameState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_file_name(string input, ref int i, int nextState)
        {
            switch ((filenameState)(nextState))
            {
                case filenameState.file_name_character:
                    return Getfile_name_character(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_file_name(int state)
        {
            switch ((filenameState)(state))
            {
                case filenameState.start:
                    return "start";

                case filenameState.dquote_0:
                    return "\"";

                case filenameState.file_name_character:
                    return "file-name-character";

                case filenameState.dquote_1:
                    return "\"";

                case filenameState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getfile_name_character(string input, ref int i)
        {
            return GetItem(input, ref i, true, "file-name-character", (int)(filenamecharacterState.start), (int)(filenamecharacterState.end), this.GetValidNextStates_file_name_character, this.GetStateTag_file_name_character, this.GetSubSpan_file_name_character);
        }

        public Int32[] GetValidNextStates_file_name_character(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((filenamecharacterState)(currentState))
            {
                case filenamecharacterState.start:
                    if (!(ch == '"' || ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(filenamecharacterState.nrx0085x2028x2029));
                    }
                    break;

                case filenamecharacterState.nrx0085x2028x2029:
                    linksToEnd = true;
                    break;

                case filenamecharacterState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_file_name_character(string input, ref int i, int nextState)
        {
            switch ((filenamecharacterState)(nextState))
            {
                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_file_name_character(int state)
        {
            switch ((filenamecharacterState)(state))
            {
                case filenamecharacterState.start:
                    return "start";

                case filenamecharacterState.nrx0085x2028x2029:
                    return "^\"\\n\\r\\x0085\\x2028\\x2029";

                case filenamecharacterState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_diagnostic(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-diagnostic", (int)(ppdiagnosticState.start), (int)(ppdiagnosticState.end), this.GetValidNextStates_pp_diagnostic, this.GetStateTag_pp_diagnostic, this.GetSubSpan_pp_diagnostic);
        }

        public Int32[] GetValidNextStates_pp_diagnostic(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppdiagnosticState)(currentState))
            {
                case ppdiagnosticState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.whitespace_0));
                    }
                    break;

                case ppdiagnosticState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.whitespace_1));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_0));
                    }
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_0));
                    }
                    break;

                case ppdiagnosticState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.hash));
                    }
                    break;

                case ppdiagnosticState.whitespace_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_0));
                    }
                    if (ch == 'w')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_0));
                    }
                    break;

                case ppdiagnosticState.error_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_1));
                    }
                    break;

                case ppdiagnosticState.error_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_2));
                    }
                    break;

                case ppdiagnosticState.error_2:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_3));
                    }
                    break;

                case ppdiagnosticState.error_3:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.error_4));
                    }
                    break;

                case ppdiagnosticState.error_4:
                    if (ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.pp_message));
                    }
                    break;

                case ppdiagnosticState.warning_0:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_1));
                    }
                    break;

                case ppdiagnosticState.warning_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_2));
                    }
                    break;

                case ppdiagnosticState.warning_2:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_3));
                    }
                    break;

                case ppdiagnosticState.warning_3:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_4));
                    }
                    break;

                case ppdiagnosticState.warning_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_5));
                    }
                    break;

                case ppdiagnosticState.warning_5:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.warning_6));
                    }
                    break;

                case ppdiagnosticState.warning_6:
                    if (ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppdiagnosticState.pp_message));
                    }
                    break;

                case ppdiagnosticState.pp_message:
                    linksToEnd = true;
                    break;

                case ppdiagnosticState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_diagnostic(string input, ref int i, int nextState)
        {
            switch ((ppdiagnosticState)(nextState))
            {
                case ppdiagnosticState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppdiagnosticState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppdiagnosticState.pp_message:
                    return Getpp_message(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_diagnostic(int state)
        {
            switch ((ppdiagnosticState)(state))
            {
                case ppdiagnosticState.start:
                    return "start";

                case ppdiagnosticState.hash:
                    return "#";

                case ppdiagnosticState.whitespace_0:
                    return "whitespace";

                case ppdiagnosticState.whitespace_1:
                    return "whitespace";

                case ppdiagnosticState.error_0:
                    return "error";

                case ppdiagnosticState.error_1:
                    return "error";

                case ppdiagnosticState.error_2:
                    return "error";

                case ppdiagnosticState.error_3:
                    return "error";

                case ppdiagnosticState.error_4:
                    return "error";

                case ppdiagnosticState.warning_0:
                    return "warning";

                case ppdiagnosticState.warning_1:
                    return "warning";

                case ppdiagnosticState.warning_2:
                    return "warning";

                case ppdiagnosticState.warning_3:
                    return "warning";

                case ppdiagnosticState.warning_4:
                    return "warning";

                case ppdiagnosticState.warning_5:
                    return "warning";

                case ppdiagnosticState.warning_6:
                    return "warning";

                case ppdiagnosticState.pp_message:
                    return "pp-message";

                case ppdiagnosticState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_message(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-message", (int)(ppmessageState.start), (int)(ppmessageState.end), this.GetValidNextStates_pp_message, this.GetStateTag_pp_message, this.GetSubSpan_pp_message);
        }

        public Int32[] GetValidNextStates_pp_message(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppmessageState)(currentState))
            {
                case ppmessageState.start:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppmessageState.new_line_0));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppmessageState.whitespace));
                    }
                    break;

                case ppmessageState.new_line_0:
                    linksToEnd = true;
                    break;

                case ppmessageState.whitespace:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(ppmessageState.input_characters));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppmessageState.new_line_1));
                    }
                    break;

                case ppmessageState.end:
                    break;

                case ppmessageState.input_characters:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(ppmessageState.new_line_1));
                    }
                    break;

                case ppmessageState.new_line_1:
                    linksToEnd = true;
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_message(string input, ref int i, int nextState)
        {
            switch ((ppmessageState)(nextState))
            {
                case ppmessageState.new_line_0:
                    return Getnew_line(input, ref i);

                case ppmessageState.whitespace:
                    return Getwhitespace(input, ref i);

                case ppmessageState.input_characters:
                    return Getinput_characters(input, ref i);

                case ppmessageState.new_line_1:
                    return Getnew_line(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_message(int state)
        {
            switch ((ppmessageState)(state))
            {
                case ppmessageState.start:
                    return "start";

                case ppmessageState.new_line_0:
                    return "new-line";

                case ppmessageState.whitespace:
                    return "whitespace";

                case ppmessageState.end:
                    return "end";

                case ppmessageState.input_characters:
                    return "input-characters";

                case ppmessageState.new_line_1:
                    return "new-line";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_region(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-region", (int)(ppregionState.start), (int)(ppregionState.end), this.GetValidNextStates_pp_region, this.GetStateTag_pp_region, this.GetSubSpan_pp_region);
        }

        public Int32[] GetValidNextStates_pp_region(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppregionState)(currentState))
            {
                case ppregionState.start:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppregionState.pp_start_region));
                    }
                    break;

                case ppregionState.pp_start_region:
                    if (false)
                    {
                        validNextStates.Add((int)(ppregionState.conditional_section));
                    }
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppregionState.pp_end_region));
                    }
                    break;

                case ppregionState.conditional_section:
                    if (ch == '#' || ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppregionState.pp_end_region));
                    }
                    break;

                case ppregionState.pp_end_region:
                    linksToEnd = true;
                    break;

                case ppregionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_region(string input, ref int i, int nextState)
        {
            switch ((ppregionState)(nextState))
            {
                case ppregionState.pp_start_region:
                    return Getpp_start_region(input, ref i);

                case ppregionState.conditional_section:
                    return Getconditional_section(input, ref i);

                case ppregionState.pp_end_region:
                    return Getpp_end_region(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_region(int state)
        {
            switch ((ppregionState)(state))
            {
                case ppregionState.start:
                    return "start";

                case ppregionState.pp_start_region:
                    return "pp-start-region";

                case ppregionState.conditional_section:
                    return "conditional-section";

                case ppregionState.pp_end_region:
                    return "pp-end-region";

                case ppregionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_start_region(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-start-region", (int)(ppstartregionState.start), (int)(ppstartregionState.end), this.GetValidNextStates_pp_start_region, this.GetStateTag_pp_start_region, this.GetSubSpan_pp_start_region);
        }

        public Int32[] GetValidNextStates_pp_start_region(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppstartregionState)(currentState))
            {
                case ppstartregionState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppstartregionState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppstartregionState.whitespace_0));
                    }
                    break;

                case ppstartregionState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppstartregionState.whitespace_1));
                    }
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_0));
                    }
                    break;

                case ppstartregionState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppstartregionState.hash));
                    }
                    break;

                case ppstartregionState.whitespace_1:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_0));
                    }
                    break;

                case ppstartregionState.region_0:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_1));
                    }
                    break;

                case ppstartregionState.region_1:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_2));
                    }
                    break;

                case ppstartregionState.region_2:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_3));
                    }
                    break;

                case ppstartregionState.region_3:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_4));
                    }
                    break;

                case ppstartregionState.region_4:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppstartregionState.region_5));
                    }
                    break;

                case ppstartregionState.region_5:
                    if (ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppstartregionState.pp_message));
                    }
                    break;

                case ppstartregionState.pp_message:
                    linksToEnd = true;
                    break;

                case ppstartregionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_start_region(string input, ref int i, int nextState)
        {
            switch ((ppstartregionState)(nextState))
            {
                case ppstartregionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppstartregionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppstartregionState.pp_message:
                    return Getpp_message(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_start_region(int state)
        {
            switch ((ppstartregionState)(state))
            {
                case ppstartregionState.start:
                    return "start";

                case ppstartregionState.hash:
                    return "#";

                case ppstartregionState.whitespace_0:
                    return "whitespace";

                case ppstartregionState.whitespace_1:
                    return "whitespace";

                case ppstartregionState.region_0:
                    return "region";

                case ppstartregionState.region_1:
                    return "region";

                case ppstartregionState.region_2:
                    return "region";

                case ppstartregionState.region_3:
                    return "region";

                case ppstartregionState.region_4:
                    return "region";

                case ppstartregionState.region_5:
                    return "region";

                case ppstartregionState.pp_message:
                    return "pp-message";

                case ppstartregionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_end_region(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-end-region", (int)(ppendregionState.start), (int)(ppendregionState.end), this.GetValidNextStates_pp_end_region, this.GetStateTag_pp_end_region, this.GetSubSpan_pp_end_region);
        }

        public Int32[] GetValidNextStates_pp_end_region(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((ppendregionState)(currentState))
            {
                case ppendregionState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppendregionState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendregionState.whitespace_0));
                    }
                    break;

                case ppendregionState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendregionState.whitespace_1));
                    }
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_0));
                    }
                    break;

                case ppendregionState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(ppendregionState.hash));
                    }
                    break;

                case ppendregionState.whitespace_1:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_0));
                    }
                    break;

                case ppendregionState.endregion_0:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_1));
                    }
                    break;

                case ppendregionState.endregion_1:
                    if (ch == 'd')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_2));
                    }
                    break;

                case ppendregionState.endregion_2:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_3));
                    }
                    break;

                case ppendregionState.endregion_3:
                    if (ch == 'e')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_4));
                    }
                    break;

                case ppendregionState.endregion_4:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_5));
                    }
                    break;

                case ppendregionState.endregion_5:
                    if (ch == 'i')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_6));
                    }
                    break;

                case ppendregionState.endregion_6:
                    if (ch == 'o')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_7));
                    }
                    break;

                case ppendregionState.endregion_7:
                    if (ch == 'n')
                    {
                        validNextStates.Add((int)(ppendregionState.endregion_8));
                    }
                    break;

                case ppendregionState.endregion_8:
                    if (ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(ppendregionState.pp_message));
                    }
                    break;

                case ppendregionState.pp_message:
                    linksToEnd = true;
                    break;

                case ppendregionState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_end_region(string input, ref int i, int nextState)
        {
            switch ((ppendregionState)(nextState))
            {
                case ppendregionState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case ppendregionState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case ppendregionState.pp_message:
                    return Getpp_message(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_end_region(int state)
        {
            switch ((ppendregionState)(state))
            {
                case ppendregionState.start:
                    return "start";

                case ppendregionState.hash:
                    return "#";

                case ppendregionState.whitespace_0:
                    return "whitespace";

                case ppendregionState.whitespace_1:
                    return "whitespace";

                case ppendregionState.endregion_0:
                    return "endregion";

                case ppendregionState.endregion_1:
                    return "endregion";

                case ppendregionState.endregion_2:
                    return "endregion";

                case ppendregionState.endregion_3:
                    return "endregion";

                case ppendregionState.endregion_4:
                    return "endregion";

                case ppendregionState.endregion_5:
                    return "endregion";

                case ppendregionState.endregion_6:
                    return "endregion";

                case ppendregionState.endregion_7:
                    return "endregion";

                case ppendregionState.endregion_8:
                    return "endregion";

                case ppendregionState.pp_message:
                    return "pp-message";

                case ppendregionState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_pragma(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-pragma", (int)(pppragmaState.start), (int)(pppragmaState.end), this.GetValidNextStates_pp_pragma, this.GetStateTag_pp_pragma, this.GetSubSpan_pp_pragma);
        }

        public Int32[] GetValidNextStates_pp_pragma(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((pppragmaState)(currentState))
            {
                case pppragmaState.start:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(pppragmaState.hash));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pppragmaState.whitespace_0));
                    }
                    break;

                case pppragmaState.hash:
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pppragmaState.whitespace_1));
                    }
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_0));
                    }
                    break;

                case pppragmaState.whitespace_0:
                    if (ch == '#')
                    {
                        validNextStates.Add((int)(pppragmaState.hash));
                    }
                    break;

                case pppragmaState.whitespace_1:
                    if (ch == 'p')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_0));
                    }
                    break;

                case pppragmaState.pragma_0:
                    if (ch == 'r')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_1));
                    }
                    break;

                case pppragmaState.pragma_1:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_2));
                    }
                    break;

                case pppragmaState.pragma_2:
                    if (ch == 'g')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_3));
                    }
                    break;

                case pppragmaState.pragma_3:
                    if (ch == 'm')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_4));
                    }
                    break;

                case pppragmaState.pragma_4:
                    if (ch == 'a')
                    {
                        validNextStates.Add((int)(pppragmaState.pragma_5));
                    }
                    break;

                case pppragmaState.pragma_5:
                    if (ch == 'x' || ch == ' ' || ch == '\t' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pppragmaState.pp_pragma_text));
                    }
                    break;

                case pppragmaState.pp_pragma_text:
                    linksToEnd = true;
                    break;

                case pppragmaState.end:
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_pragma(string input, ref int i, int nextState)
        {
            switch ((pppragmaState)(nextState))
            {
                case pppragmaState.whitespace_0:
                    return Getwhitespace(input, ref i);

                case pppragmaState.whitespace_1:
                    return Getwhitespace(input, ref i);

                case pppragmaState.pp_pragma_text:
                    return Getpp_pragma_text(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_pragma(int state)
        {
            switch ((pppragmaState)(state))
            {
                case pppragmaState.start:
                    return "start";

                case pppragmaState.hash:
                    return "#";

                case pppragmaState.whitespace_0:
                    return "whitespace";

                case pppragmaState.whitespace_1:
                    return "whitespace";

                case pppragmaState.pragma_0:
                    return "pragma";

                case pppragmaState.pragma_1:
                    return "pragma";

                case pppragmaState.pragma_2:
                    return "pragma";

                case pppragmaState.pragma_3:
                    return "pragma";

                case pppragmaState.pragma_4:
                    return "pragma";

                case pppragmaState.pragma_5:
                    return "pragma";

                case pppragmaState.pp_pragma_text:
                    return "pp-pragma-text";

                case pppragmaState.end:
                    return "end";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }

        public MetaphysicsIndustries.Giza.ParseSpan Getpp_pragma_text(string input, ref int i)
        {
            return GetItem(input, ref i, true, "pp-pragma-text", (int)(pppragmatextState.start), (int)(pppragmatextState.end), this.GetValidNextStates_pp_pragma_text, this.GetStateTag_pp_pragma_text, this.GetSubSpan_pp_pragma_text);
        }

        public Int32[] GetValidNextStates_pp_pragma_text(int currentState, char ch, out bool linksToEnd)
        {
            System.Collections.Generic.List<int> validNextStates = new System.Collections.Generic.List<int>();

            linksToEnd = false;
            switch ((pppragmatextState)(currentState))
            {
                case pppragmatextState.start:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(pppragmatextState.new_line));
                    }
                    if (ch == ' ' || ch == '\t' || ch == 'x' || ch == '0' || ch == '9' || ch == '8' || ch == 'c')
                    {
                        validNextStates.Add((int)(pppragmatextState.whitespace));
                    }
                    break;

                case pppragmatextState.new_line:
                    linksToEnd = true;
                    break;

                case pppragmatextState.whitespace:
                    if (!(ch == '\n' || ch == '\r' || ch == 'x' || ch == '0' || ch == '8' || ch == '5' || ch == '2' || ch == '9'))
                    {
                        validNextStates.Add((int)(pppragmatextState.input_characters));
                    }
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(pppragmatextState.new_line));
                    }
                    break;

                case pppragmatextState.end:
                    break;

                case pppragmatextState.input_characters:
                    if (ch == 'x')
                    {
                        validNextStates.Add((int)(pppragmatextState.new_line));
                    }
                    break;

                default:
                    throw new System.InvalidOperationException("Invalid state");
            }
            return validNextStates.ToArray();
        }

        public MetaphysicsIndustries.Giza.ParseSpan GetSubSpan_pp_pragma_text(string input, ref int i, int nextState)
        {
            switch ((pppragmatextState)(nextState))
            {
                case pppragmatextState.new_line:
                    return Getnew_line(input, ref i);

                case pppragmatextState.whitespace:
                    return Getwhitespace(input, ref i);

                case pppragmatextState.input_characters:
                    return Getinput_characters(input, ref i);

                default:
                    return new MetaphysicsIndustries.Giza.ParseSpan(i, 1, input);
            }
        }

        public string GetStateTag_pp_pragma_text(int state)
        {
            switch ((pppragmatextState)(state))
            {
                case pppragmatextState.start:
                    return "start";

                case pppragmatextState.new_line:
                    return "new-line";

                case pppragmatextState.whitespace:
                    return "whitespace";

                case pppragmatextState.end:
                    return "end";

                case pppragmatextState.input_characters:
                    return "input-characters";
            }
            throw new System.ArgumentOutOfRangeException("state");
        }
    }
}


