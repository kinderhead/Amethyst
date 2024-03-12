using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class NBTList<T>(string prefix = "") : NBTType, IList<T> where T : NBTType
    {
        public List<T> Values = [];

        protected readonly string Prefix = prefix;

        public T this[int index] { get => Values[index]; set => Values[index] = value; }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Values.Add(item);
        }

        public override void Build(StringBuilder sb)
        {
            sb.Append('[');
            sb.Append(Prefix);
            foreach (var i in Values)
            {
                i.Build(sb);
                sb.Append(',');
            }
            if (Values.Count > 0) sb.Length--;
            sb.Append(']');
        }

        public void Clear()
        {
            Values.Clear();
        }

        public bool Contains(T item)
        {
            return Values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Values.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Values.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return Values.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }

    public class NBTList : NBTList<NBTType> { }
    public class NBTIntArray() : NBTList<NBTInt>("I;") { }
    public class NBTByteArray() : NBTList<NBTByte>("B;") { }
    public class NBTLongArray() : NBTList<NBTLong>("L;") { }
}
