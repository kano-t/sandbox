using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GrpcHelper.Server
{
    class CompositeDictionary<K1, K2, V>
    {
        private readonly ConcurrentDictionary<K1, ConcurrentDictionary<K2, V>> _dict = new ConcurrentDictionary<K1, ConcurrentDictionary<K2, V>>();

        public IEnumerable<K1> Keys
        {
            get { return _dict.Keys; }
        }

        public bool TryAdd(K1 key1, K2 key2, V value)
        {
            var sub = _dict.GetOrAdd(key1, k => new ConcurrentDictionary<K2, V>());
            return sub.TryAdd(key2, value);
        }

        public bool TryRemove(K1 key1, K2 key2)
        {
            ConcurrentDictionary<K2, V> dict;
            V val;
            return _dict.TryGetValue(key1, out dict) && dict.TryRemove(key2, out val)
                && dict.Count == 0 && _dict.TryRemove(key1, out dict);
        }

        public IEnumerable<V> GetValues(K1 key1)
        {
            ConcurrentDictionary<K2, V> dict;
            return _dict.TryGetValue(key1, out dict) ? dict.Values : new V[0];
        }
    }
}
