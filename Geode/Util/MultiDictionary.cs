using System;

namespace Geode.Util
{
    public class MultiDictionary<TKey, T> : List<KeyValuePair<TKey, T>> where TKey : IEquatable<TKey>
    {
        public IEnumerable<TKey> Keys => this.Select(i => i.Key);
        public IEnumerable<T> Values => this.Select(i => i.Value);

        public void Add(TKey key, T value) => Add(new(key, value));

        public IEnumerable<T> this[TKey key]
        {
            get => this.Where(i => i.Key.Equals(key)).Select(i => i.Value);
        }
    }
}
