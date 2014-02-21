using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public class SupergrammarRenderer
    {
        public string RenderGrammar(Span grammar)
        {
            StringBuilder sb = new StringBuilder();
            RenderGrammar(grammar, sb);
            return sb.ToString();
        }

        private void RenderGrammar(Span grammar, StringBuilder sb)
        {
            //if (grammar.Tag != "grammar") throw new InvalidOperationException();

            foreach (Span def in grammar.Subspans)
            {
                RenderDefinition(def, sb);
                sb.AppendLine();
                sb.AppendLine();
            }
        }

        private void RenderDefinition(Span def, StringBuilder sb)
        {
            if (def.Tag != "definition") throw new InvalidOperationException();

            foreach (Span sub in def.Subspans)
            {
                if (sub.Tag == "defmod")
                {
                    sb.Append(sub.Value);
                    sb.Append(" ");
                }
                else if (sub.Tag == "identifier")
                {
                    sb.Append(sub.Value);
                    sb.Append(" = ");
                }
                else if (sub.Tag == "expr")
                {
                    RenderExpr(sub, sb);
                    sb.Append(";");
                }
                else
                {
                }
            }
        }

        private void RenderExpr(Span expr, StringBuilder sb)
        {
            if (expr.Tag != "expr") throw new InvalidOperationException();

            bool first = true;
            foreach (Span sub in expr.Subspans)
            {
                if (first) first = false; else sb.Append(" ");
                if (sub.Tag == "subexpr")
                {
                    RenderSubexpr(sub, sb);
                }
                else if (sub.Tag == "orexpr")
                {
                    RenderOrexpr(sub, sb);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private void RenderSubexpr(Span subexpr, StringBuilder sb)
        {
            if (subexpr.Tag != "subexpr") throw new InvalidOperationException();

            bool first = true;
            foreach (Span sub in subexpr.Subspans)
            {
                if (sub.Tag == "identifier")
                {
                    if (first) first = false; else sb.Append(" ");
                    sb.Append(sub.Value);
                }
                else if (sub.Tag == "literal")
                {
                    if (first) first = false; else sb.Append(" ");
                    RenderLiteral(sub, sb);
                }
                else if (sub.Tag == "charclass")
                {
                    if (first) first = false; else sb.Append(" ");
                    RenderCharClass(sub, sb);
                }
                else if (sub.Tag == "modifier")
                {
                    sb.Append(sub.Value);
                }
                else if (sub.Tag == "tag")
                {
                    sb.Append(":");
                    sb.Append(sub.Value);
                }
                else
                {
                }
            }
        }

        private void RenderCharClass(Span charclass, StringBuilder sb)
        {
            if (charclass.Tag != "charclass") throw new InvalidOperationException();

            sb.Append(charclass.Value);
        }

        private void RenderLiteral(Span literal, StringBuilder sb)
        {
            if (literal.Tag != "literal") throw new InvalidOperationException();

            sb.Append(literal.Value);
        }

        private void RenderOrexpr(Span orexpr, StringBuilder sb)
        {
            if (orexpr.Tag != "orexpr") throw new InvalidOperationException();

            sb.Append("( ");
            bool first = true;
            Span modifier = null;
            foreach (Span expr in orexpr.Subspans)
            {
                if (expr.Tag == "expr")
                {
                    if (first) first = false; else sb.Append(" | ");
                    RenderExpr(expr, sb);
                }
                else if (expr.Tag == "modifier")
                {
                    modifier = expr;
                }
                //else if (expr.Tag != "oparen" && expr.Tag != "cparen")
                //{
                //}
            }
            sb.Append(" )");

            if (modifier != null)
            {
                sb.Append(modifier.Value);
            }
        }
    }
}
