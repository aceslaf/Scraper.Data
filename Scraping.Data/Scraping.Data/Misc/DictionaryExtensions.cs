using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Common
{
    public static class DictionaryExtensions
    {
        public static void AddOrOvewrite<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal val)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, val);
            }
            else
            {
                dict[key] = val;
            }
        }

        public static void AddOrOvewrite<TKey, TVal>(this Dictionary<TKey, TVal> dict, KeyValuePair<TKey,TVal> pair)
        {
            dict.AddOrOvewrite(pair.Key, pair.Value);
        }
    }
}
