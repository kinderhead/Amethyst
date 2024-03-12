using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class NBTCompound : NBTType, IDictionary<string, NBTType>, INegatable<NBTCompound>
    {
        public Dictionary<string, NBTType> Values = [];

        public NBTType this[string key] { get => Values[key]; set => Values[key] = value; }

        public ICollection<string> Keys => Values.Keys;

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        ICollection<NBTType> IDictionary<string, NBTType>.Values => Values.Values;

        public void Add(string key, NBTType value)
        {
            Values.Add(key, value);
        }

        public void Add(KeyValuePair<string, NBTType> item)
        {
            Values.Add(item.Key, item.Value);
        }

        public override void Build(StringBuilder sb)
        {
            sb.Append('{');
            foreach (var i in Values)
            {
                sb.Append('"');
                sb.Append(NBTString.Escape(i.Key));
                sb.Append('"');
                sb.Append(':');
                i.Value.Build(sb);
                sb.Append(',');
            }
            if (Values.Count > 0) sb.Length--;
            sb.Append('}');
        }

        public void Clear()
        {
            Values.Clear();
        }

        public bool Contains(KeyValuePair<string, NBTType> item)
        {
            return Values.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Values.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, NBTType>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, NBTType>> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public Negatable<NBTCompound> Negate()
        {
            return new(this, true);
        }

        public bool Remove(string key)
        {
            return Values.Remove(key);
        }

        public bool Remove(KeyValuePair<string, NBTType> item)
        {
            return Values.Remove(item.Key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out NBTType value)
        {
            return Values.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public static Negatable<NBTCompound> operator !(NBTCompound nbt) => nbt.Negate();
        public static implicit operator Negatable<NBTCompound>(NBTCompound nbt) => new(nbt);
    }
}
