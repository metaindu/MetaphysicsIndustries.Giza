
/*****************************************************************************
 *                                                                           *
 *  GrammarDefinitionOrderedParentChildrenCollection.cs                      *
 *  18 June 2013                                                             *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2013 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  An ordered collection of Definition objects.                             *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class GrammarDefinitionOrderedParentChildrenCollection : IList<Definition>, IDisposable
    {
        public GrammarDefinitionOrderedParentChildrenCollection(Grammar container)
        {
            _container = container;
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public void AddRange(params Definition[] items)
        {
            AddRange((IEnumerable<Definition>)items);
        }
        public void AddRange(IEnumerable<Definition> items)
        {
            foreach (Definition item in items)
            {
                Add(item);
            }
        }
        public void RemoveRange(params Definition[] items)
        {
            RemoveRange((IEnumerable<Definition>)items);
        }
        public void RemoveRange(IEnumerable<Definition> items)
        {
            foreach (Definition item in items)
            {
                Remove(item);
            }
        }

        //ICollection<Definition>
        public virtual void Add(Definition item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                item.ParentGrammar = _container;
            }
        }

        public virtual bool Contains(Definition item)
        {
            return _list.Contains(item);
        }

        public virtual bool Remove(Definition item)
        {
            if (Contains(item))
            {
                bool ret = _list.Remove(item);
                item.ParentGrammar = null;
                return ret;
            }

            return false;
        }

        public virtual void Clear()
        {
            Definition[] array = new Definition[Count];

            CopyTo(array, 0);

            foreach (Definition item in array)
            {
                Remove(item);
            }

            _list.Clear();
        }

        public virtual void CopyTo(Definition[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<Definition> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        //IList<Definition>
        public virtual int IndexOf(Definition item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, Definition item)
        {
            if (Contains(item))
            {
                if (IndexOf(item) < index)
                {
                    index--;
                }

                Remove(item);
            }

            item.ParentGrammar = null;
            _list.Insert(index, item);
            item.ParentGrammar = _container;
        }

        public virtual void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        //ICollection<Definition>
        public virtual int Count
        {
            get { return _list.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return (_list as ICollection<Definition>).IsReadOnly; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //IList<Definition>
        public virtual Definition this [ int index ]
        {
            get { return _list[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        private Grammar _container;
        private List<Definition> _list = new List<Definition>();
    }
}
