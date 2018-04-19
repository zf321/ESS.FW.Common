using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ESS.FW.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static bool Contains<T>(this IEnumerable<T> collection, Predicate<T> condition)
        {
            return collection.Any(x => condition(x));
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return true;
            var coll = collection as ICollection;
            if (coll != null)
                return coll.Count == 0;
            return !collection.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> collection)
        {
            return !IsEmpty(collection);
        }

        public static Dictionary<TKey, TElement> ToDistinctDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var result = new Dictionary<TKey,TElement>();
            foreach(var c in collection)
            {
                if (!result.ContainsKey(keySelector.Invoke(c)))
                {
                    result.Add(keySelector.Invoke(c), elementSelector.Invoke(c));
                }
            }

            return result;

        }
    }
}
