using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Span
    {
        public List<Span> Subspans = new List<Span>();
        public Definition Definition;
        public Node Node;
        public string Value;

        public string CollectValue()
        {
            if (Value == null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Span sub in Subspans)
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
