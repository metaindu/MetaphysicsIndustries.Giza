using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Span2
    {
        public List<Span2> Subspans = new List<Span2>();
        public Definition Definition;
        public Node Node;
        public string Value;

        public string CollectValue()
        {
            if (Value == null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Span2 sub in Subspans)
                {
                    sb.Append(sub.CollectValue());
                }

                Value = sb.ToString();
            }

            return Value;
        }

        public override string ToString()
        {
            return string.Format("Def{{{0}}}, Node{{{1}}}, \"{2}\", {3} subspans", Definition, Node, Value, Subspans.Count);
        }
    }


}
