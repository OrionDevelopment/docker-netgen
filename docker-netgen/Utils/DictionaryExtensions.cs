using System;
using System.Collections.Generic;

namespace docker_netgen.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> producer)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, producer());

            return dictionary[key];
        }
    }
}