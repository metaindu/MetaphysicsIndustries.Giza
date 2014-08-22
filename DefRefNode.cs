using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{

    public class DefRefNode : Node
    {
        public DefRefNode(Definition def)
            : this(def, def == null ? string.Empty : def.Name)
        {
        }
        public DefRefNode(Definition def, string tag)
            : base(tag)
        {
            if (def == null) { throw new ArgumentNullException("def"); }

            _defRef = def;
        }

        Definition _defRef;
        public Definition DefRef
        {
            get { return _defRef; }
        }

        public override string ToString()
        {
            return string.Format("[{0}] \"[{1}] {2}\", {3}", ID, DefRef.ID, DefRef.Name, Tag);
        }
    }
    
}
