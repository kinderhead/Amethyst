using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class NBTList<T>(string prefix = "") : NBTValue, IList<T> where T : NBTValue
    {
		public override NBTType Type => NBTType.List;
		public List<T> Values = [];

        protected readonly string Prefix = prefix;

        public T this[int index] { get => Values[index]; set => Values[index] = value; }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Values.Add(item);
        }

		public override NBTValue Cast(NBTNumberType type) => throw new InvalidOperationException();

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

    public class NBTList : NBTList<NBTValue> { }

    public class NBTIntArray() : NBTList<NBTInt>("I;")
    {
		public override NBTType Type => NBTType.IntArray;
	}

    public class NBTByteArray() : NBTList<NBTByte>("B;")
    {
		public override NBTType Type => NBTType.ByteArray;
	}

    public class NBTLongArray() : NBTList<NBTLong>("L;")
    {
		public override NBTType Type => NBTType.LongArray;
	}
}
