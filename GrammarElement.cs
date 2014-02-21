using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Giza
{
    public abstract class GrammarElement
    {
    }

    public class Grammar : GrammarElement
    {
        public List<Definition> definition = new List<Definition>();
    }

    public class Definition : GrammarElement
    {
        public List<DefMod> defmod = new List<DefMod>();
        public string identifier;
        public Expr expr;
    }

    public class DefMod : GrammarElement
    {
        public string value;
    }

    public class Expr : GrammarElement
    {
        public List<ExprItem> items = new List<ExprItem>();
    }

    public abstract class ExprItem : GrammarElement
    {
    }

    public class OrExpr : ExprItem
    {
        public List<Expr> expr = new List<Expr>();
        public Modifier modifier;
    }

    public class SubExpr : ExprItem
    {
        public string identifier;
        public string literal;
        public string charclass;
        public Modifier modifier;
        public string tag;
        public string collection;
    }

    public class Modifier : GrammarElement
    {
        public string value;
    }

}
