
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
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


namespace MetaphysicsIndustries.Giza
{
    /// <summary>
    /// An ordered collection of NDefinition objects.
    /// </summary>
    public class GrammarDefinitionOrderedParentChildrenCollection : 
        IList<NDefinition>, IDisposable
    {
        public GrammarDefinitionOrderedParentChildrenCollection(NGrammar container)
        {
            _container = container;
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public void AddRange(params NDefinition[] items)
        {
            AddRange((IEnumerable<NDefinition>)items);
        }
        public void AddRange(IEnumerable<NDefinition> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
        public void RemoveRange(params NDefinition[] items)
        {
            RemoveRange((IEnumerable<NDefinition>)items);
        }
        public void RemoveRange(IEnumerable<NDefinition> items)
        {
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        //ICollection<NDefinition>
        public virtual void Add(NDefinition item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                item.ParentGrammar = _container;
            }
        }

        public virtual bool Contains(NDefinition item)
        {
            return _list.Contains(item);
        }

        public virtual bool Remove(NDefinition item)
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
            var array = new NDefinition[Count];

            CopyTo(array, 0);

            foreach (var item in array)
            {
                Remove(item);
            }

            _list.Clear();
        }

        public virtual void CopyTo(NDefinition[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<NDefinition> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        //IList<NDefinition>
        public virtual int IndexOf(NDefinition item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, NDefinition item)
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

        //ICollection<NDefinition>
        public virtual int Count
        {
            get { return _list.Count; }
        }

        public virtual bool IsReadOnly => 
            (_list as ICollection<NDefinition>).IsReadOnly;

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //IList<NDefinition>
        public virtual NDefinition this [ int index ]
        {
            get { return _list[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        private readonly NGrammar _container;
        private readonly List<NDefinition> _list = new List<NDefinition>();
    }
}
