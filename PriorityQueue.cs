using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class PriorityQueue<TValue, TPriority>
    {
        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }
        public PriorityQueue(IComparer<TPriority> comparer)
        {
            _comparer = new ReverseComparer(comparer);
        }

        class ReverseComparer : IComparer<TPriority>
        {
            public ReverseComparer(IComparer<TPriority> comparer)
            {
                _comparer = comparer;
            }

            readonly IComparer<TPriority> _comparer;

            public int Compare(TPriority x, TPriority y)
            {
                return _comparer.Compare(y, x);
            }
        }

        readonly IComparer<TPriority> _comparer;
        readonly List<TValue> _values = new List<TValue>();
        readonly List<TPriority> _priorities = new List<TPriority>();

        public int Count
        {
            get { return _values.Count; }
        }

        public void Enqueue(TValue value, TPriority priority)
        {
            var index = _priorities.BinarySearch(priority, _comparer);
            if (index < 0)
            {
                index = ~index;
            }
            else
            {
                while (index < Count &&
                       _comparer.Compare(_priorities[index], priority) == 0)
                {
                    index++;
                }
            }

            _values.Insert(index, value);
            _priorities.Insert(index, priority);
        }

        public TValue Dequeue()
        {
            TValue value = _values[0];

            _values.RemoveAt(0);
            _priorities.RemoveAt(0);

            return value;
        }

        public TValue Peek()
        {
            return _values[0];
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(
                    "The number of elements in the queue is greater than the " +
                    "available space from arrayIndex to the end of the " +
                    "destination array.");
            }

            int i;
            for (i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = _values[i];
            }
        }
    }
}

