using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Span
    {
        public List<Span> Subspans = new List<Span>();
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
            return string.Format("Node{{{0}}}, \"{1}\", {2} subspans", Node, Value, Subspans.Count);
        }
     }


}
