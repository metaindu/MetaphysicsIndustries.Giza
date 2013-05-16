
/*****************************************************************************
 *                                                                           *
 *  GrammarDefinitionParentChildrenCollection.cs                             *
 *  15 May 2013                                                              *
 *  Project: MetaphysicsIndustries.Giza                                      *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2013 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  An unordered collection of Definition objects.                           *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class GrammarDefinitionParentChildrenCollection : ICollection<Definition>, IDisposable
    {
        public GrammarDefinitionParentChildrenCollection(Grammar container)
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
                _set.Add(item);
                item.ParentGrammar = _container;
            }
        }

        public virtual bool Contains(Definition item)
        {
            return _set.Contains(item);
        }

        public virtual bool Remove(Definition item)
        {
            if (Contains(item))
            {
                bool ret = _set.Remove(item);
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

            _set.Clear();
        }

        public virtual void CopyTo(Definition[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<Definition> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        //ICollection<Definition>
        public virtual int Count
        {
            get { return _set.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return (_set as ICollection<Definition>).IsReadOnly; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Grammar _container;
        private Set<Definition> _set = new Set<Definition>();
    }
}
