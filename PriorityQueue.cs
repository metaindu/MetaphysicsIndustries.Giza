
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class PriorityQueue<TValue, TPriority>
    {
        public PriorityQueue(bool lowToHigh=false)
            : this(Comparer<TPriority>.Default, lowToHigh: lowToHigh)
        {
        }
        public PriorityQueue(IComparer<TPriority> comparer, bool lowToHigh=false)
        {
            if (lowToHigh)
            {
                _comparer = comparer;
            }
            else
            {
                _comparer = new ReverseComparer(comparer);
            }
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

