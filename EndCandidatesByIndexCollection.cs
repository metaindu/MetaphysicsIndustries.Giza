
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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
    public class EndCandidatesByIndexCollection<T> : IDictionary<int, Queue<NodeMatch<T>>>
        where T : IInputElement
    {
        readonly Dictionary<int, Queue<NodeMatch<T>>> _dictionary = new Dictionary<int, Queue<NodeMatch<T>>>();

        public void Add(int key, Queue<NodeMatch<T>> value)
        {
            _dictionary.Add(key, value);
        }
        public bool ContainsKey(int key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool Remove(int key)
        {
            return _dictionary.Remove(key);
        }
        public bool TryGetValue(int key, out Queue<NodeMatch<T>> value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        public Queue<NodeMatch<T>> this[int index]
        {
            get
            {
                if (!ContainsKey(index))
                {
                    _dictionary[index] = new Queue<NodeMatch<T>>();
                }

                return _dictionary[index];
            }
            set
            {
                _dictionary[index] = value;
            }
        }
        public ICollection<int> Keys
        {
            get { return _dictionary.Keys; }
        }
        public ICollection<Queue<NodeMatch<T>>> Values
        {
            get { return _dictionary.Values; }
        }
        public void Clear()
        {
            throw new NotImplementedException();
        }
        public int Count
        {
            get { return _dictionary.Count; }
        }
        public IEnumerator<KeyValuePair<int, Queue<NodeMatch<T>>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        void ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>.Add(KeyValuePair<int, Queue<NodeMatch<T>>> item)
        {
            ((ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>)_dictionary).Add(item);
        }
        bool ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>.Contains(KeyValuePair<int, Queue<NodeMatch<T>>> item)
        {
            return ((ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>)_dictionary).Contains(item);
        }
        void ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>.CopyTo(KeyValuePair<int, Queue<NodeMatch<T>>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>)_dictionary).CopyTo(array, arrayIndex);
        }
        bool ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>.Remove(KeyValuePair<int, Queue<NodeMatch<T>>> item)
        {
            return ((ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>)_dictionary).Remove(item);
        }
        bool ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<int, Queue<NodeMatch<T>>>>)_dictionary).IsReadOnly;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

