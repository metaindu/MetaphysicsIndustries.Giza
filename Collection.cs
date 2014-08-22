using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public static class Collection
    {
        public static bool ContainsAll<T>(this IEnumerable<T> collection, params T[] items)
        {
            var collection2 = new HashSet<T>(collection);

            foreach (T item in items)
            {
                if (!collection2.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}

