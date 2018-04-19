using System.Collections.Concurrent;

namespace ESS.FW.Common.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static TValue Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            dict.TryRemove(key, out value);
            return value;
        }
    }
}
