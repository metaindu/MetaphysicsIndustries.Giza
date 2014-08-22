using System;

namespace MetaphysicsIndustries.Giza
{
    public struct InputPosition
    {
        public InputPosition(int index=0, int line=0, int column=0)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        public int Index;
        public int Line;
        public int Column;
    }
}

