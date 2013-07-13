using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class Span
    {
        public Span()
        {
            _subspans = new SpanSpanOrderedParentChildrenCollection(this);
        }

        private SpanSpanOrderedParentChildrenCollection _subspans;
        public SpanSpanOrderedParentChildrenCollection Subspans
        {
            get { return _subspans; }
        }

        private Span _parentSpan;
        public Span ParentSpan
        {
            get { return _parentSpan; }
            set
            {
                if (value != _parentSpan)
                {
                    if (_parentSpan != null)
                    {
                        _parentSpan.Subspans.Remove(this);
                    }

                    _parentSpan = value;

                    if (_parentSpan != null)
                    {
                        _parentSpan.Subspans.Add(this);
                    }
                }
            }
        }

        public Node Node;
        public Definition DefRef
        {
            get
            {
                if (Node is DefRefNode)
                {
                    return (Node as DefRefNode).DefRef;
                }

                return null;
            }
        }
        public string Value;

        public string CollectValue()
        {
            if (Value == null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Span sub in Subspans)
                {
                    sb.Append(sub.CollectValue());
                }

                Value = sb.ToString();
            }

            return Value;
        }

        public override string ToString()
        {
            return string.Format("Node{{{0}}}, \"{1}\", {2} subspans", Node, Value, Subspans.Count);
        }

        public string RenderSpanHierarchy()
        {
            StringBuilder sb = new StringBuilder();
            this.RenderSpanHierarchy(sb);
            return sb.ToString();
        }
        void RenderSpanHierarchy(StringBuilder sb, string indent="")
        {
            sb.Append(indent);
            sb.Append(this.ToString());
            sb.AppendLine();

            string indent2 = indent + "  ";
            foreach (var sub in this.Subspans)
            {
                sub.RenderSpanHierarchy(sb, indent2);
            }
        }
     }

    public class SpanSpanOrderedParentChildrenCollection : IList<Span>, IDisposable
    {
        public SpanSpanOrderedParentChildrenCollection(Span container)
        {
            _container = container;
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public void AddRange(params Span[] items)
        {
            AddRange((IEnumerable<Span>)items);
        }
        public void AddRange(IEnumerable<Span> items)
        {
            foreach (Span item in items)
            {
                Add(item);
            }
        }
        public void RemoveRange(params Span[] items)
        {
            RemoveRange((IEnumerable<Span>)items);
        }
        public void RemoveRange(IEnumerable<Span> items)
        {
            foreach (Span item in items)
            {
                Remove(item);
            }
        }

        //ICollection<Span>
        public virtual void Add(Span item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                item.ParentSpan = _container;
            }
        }

        public virtual bool Contains(Span item)
        {
            return _list.Contains(item);
        }

        public virtual bool Remove(Span item)
        {
            if (Contains(item))
            {
                bool ret = _list.Remove(item);
                item.ParentSpan = null;
                return ret;
            }

            return false;
        }

        public virtual void Clear()
        {
            Span[] array = new Span[Count];

            CopyTo(array, 0);

            foreach (Span item in array)
            {
                Remove(item);
            }

            _list.Clear();
        }

        public virtual void CopyTo(Span[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<Span> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        //IList<Span>
        public virtual int IndexOf(Span item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, Span item)
        {
            if (Contains(item))
            {
                if (IndexOf(item) < index)
                {
                    index--;
                }

                Remove(item);
            }

            item.ParentSpan = null;
            _list.Insert(index, item);
            item.ParentSpan = _container;
        }

        public virtual void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        //ICollection<Span>
        public virtual int Count
        {
            get { return _list.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return (_list as ICollection<Span>).IsReadOnly; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //IList<Span>
        public virtual Span this [ int index ]
        {
            get { return _list[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        private Span _container;
        private List<Span> _list = new List<Span>();
    }
}
