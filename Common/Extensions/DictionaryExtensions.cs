using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static TElement SafeGet<TKey, TElement>(this Dictionary<TKey, TElement> dict, TKey key)
        {
            TElement element;
            if(dict.TryGetValue(key, out element))
            {
                return element;
            }
            return default(TElement);
        }
    }
}
