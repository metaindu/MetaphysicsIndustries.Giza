
/*****************************************************************************
 *                                                                           *
 *  DefinitionNodeOrderedParentChildrenCollection.cs                         *
 *  16 May 2013                                                              *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2013 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  An ordered collection of Node objects.                                   *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;


namespace MetaphysicsIndustries.Giza
{
    public class DefinitionNodeOrderedParentChildrenCollection : IList<Node>, IDisposable
    {
        public DefinitionNodeOrderedParentChildrenCollection(Definition container)
        {
            _container = container;
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public void AddRange(params Node[] items)
        {
            AddRange((IEnumerable<Node>)items);
        }
        public void AddRange(IEnumerable<Node> items)
        {
            foreach (Node item in items)
            {
                Add(item);
            }
        }
        public void RemoveRange(params Node[] items)
        {
            RemoveRange((IEnumerable<Node>)items);
        }
        public void RemoveRange(IEnumerable<Node> items)
        {
            foreach (Node item in items)
            {
                Remove(item);
            }
        }

        //ICollection<Node>
        public virtual void Add(Node item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                item.ParentDefinition = _container;
            }
        }

        public virtual bool Contains(Node item)
        {
            return _list.Contains(item);
        }

        public virtual bool Remove(Node item)
        {
            if (Contains(item))
            {
                bool ret = _list.Remove(item);
                item.ParentDefinition = null;
                return ret;
            }

            return false;
        }

        public virtual void Clear()
        {
            Node[] array = new Node[Count];

            CopyTo(array, 0);

            foreach (Node item in array)
            {
                Remove(item);
            }

            _list.Clear();
        }

        public virtual void CopyTo(Node[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<Node> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        //IList<Node>
        public virtual int IndexOf(Node item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, Node item)
        {
            if (Contains(item))
            {
                if (IndexOf(item) < index)
                {
                    index--;
                }

                Remove(item);
            }

            item.ParentDefinition = null;
            _list.Insert(index, item);
            item.ParentDefinition = _container;
        }

        public virtual void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        //ICollection<Node>
        public virtual int Count
        {
            get { return _list.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return (_list as ICollection<Node>).IsReadOnly; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //IList<Node>
        public virtual Node this [ int index ]
        {
            get { return _list[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        private Definition _container;
        private List<Node> _list = new List<Node>();
    }
}