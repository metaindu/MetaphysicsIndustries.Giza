using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class NodeMatchNodeMatchPreviousNextsCollection : ICollection<NodeMatch>
    {
        public NodeMatchNodeMatchPreviousNextsCollection(NodeMatch container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
        }

        NodeMatch _container;
        Set<NodeMatch> _collection = new Set<NodeMatch>();

        #region ICollection implementation

        public void Add(NodeMatch item)
        {
            if (!this.Contains(item))
            {
                _collection.Add(item);
                item.Previous = _container;
            }
        }

        public void Clear()
        {
            foreach (NodeMatch item in this.ToArray())
            {
                this.Remove(item);
            }
        }

        public bool Contains(NodeMatch item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(NodeMatch[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(NodeMatch item)
        {
            if (_collection.Remove(item))
            {
                item.Previous = null;

                return true;
            }

            return false;
        }

        public int Count
        {
            get
            {
                return _collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<NodeMatch> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

