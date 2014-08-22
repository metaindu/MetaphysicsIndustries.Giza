using System;
using System.Collections.Generic;


namespace MetaphysicsIndustries.Giza
{
    public class BranchTipsByIndexCollection<T> : IDictionary<int, Queue<BranchTip<T>>>
        where T : IInputElement
    {
        readonly Dictionary<int, Queue<BranchTip<T>>> _dictionary = new Dictionary<int, Queue<BranchTip<T>>>();

        public void Add(int key, Queue<BranchTip<T>> value)
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
        public bool TryGetValue(int key, out Queue<BranchTip<T>> value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        public Queue<BranchTip<T>> this[int index]
        {
            get
            {
                if (!ContainsKey(index))
                {
                    _dictionary[index] = new Queue<BranchTip<T>>();
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
        public ICollection<Queue<BranchTip<T>>> Values
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
        public IEnumerator<KeyValuePair<int, Queue<BranchTip<T>>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        void ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>.Add(KeyValuePair<int, Queue<BranchTip<T>>> item)
        {
            ((ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>)_dictionary).Add(item);
        }
        bool ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>.Contains(KeyValuePair<int, Queue<BranchTip<T>>> item)
        {
            return ((ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>)_dictionary).Contains(item);
        }
        void ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>.CopyTo(KeyValuePair<int, Queue<BranchTip<T>>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>)_dictionary).CopyTo(array, arrayIndex);
        }
        bool ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>.Remove(KeyValuePair<int, Queue<BranchTip<T>>> item)
        {
            return ((ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>)_dictionary).Remove(item);
        }
        bool ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<int, Queue<BranchTip<T>>>>)_dictionary).IsReadOnly;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

