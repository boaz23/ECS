using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Range
    {
        public Range(int start, int end, int count)
        {
            Start = start;
            End = end;
            Count = count;
        }

        public int Start { get; }
        public int End { get; }
        public int Count { get; }
    }
}
