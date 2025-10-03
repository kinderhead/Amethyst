using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Datapack.Net.Data
{
	public class NBTCompound : NBTValue, IDictionary<string, NBTValue>, INegatable<NBTCompound>
	{
		public override NBTType Type => NBTType.Compound;
		public Dictionary<string, NBTValue> Values = [];
		public NBTValue this[string key] { get => Values[key]; set => Values[key] = value; }
		public ICollection<string> Keys => Values.Keys;
		public int Count => Values.Count;
		public bool IsReadOnly => false;

		public NBTCompound()
		{
		}

		public NBTCompound(IEnumerable<KeyValuePair<string, NBTValue>> values)
		{
			foreach (var i in values)
			{
				Values.Add(i.Key, i.Value);
			}
		}

		public override NBTValue Cast(NBTNumberType type) => throw new InvalidOperationException();

		ICollection<NBTValue> IDictionary<string, NBTValue>.Values => Values.Values;

		public void Add(string key, NBTValue value) => Values.Add(key, value);

		public void Add(KeyValuePair<string, NBTValue> item) => Values.Add(item.Key, item.Value);

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

			if (Values.Count > 0)
			{
				sb.Length--;
			}

			sb.Append('}');
		}

		public void Clear() => Values.Clear();

		public bool Contains(KeyValuePair<string, NBTValue> item) => Values.Contains(item);

		public bool ContainsKey(string key) => Values.ContainsKey(key);

		public void CopyTo(KeyValuePair<string, NBTValue>[] array, int arrayIndex) => throw new NotImplementedException();

		public IEnumerator<KeyValuePair<string, NBTValue>> GetEnumerator() => Values.GetEnumerator();

		public Negatable<NBTCompound> Negate() => new(this, true);

		public bool Remove(string key) => Values.Remove(key);

		public bool Remove(KeyValuePair<string, NBTValue> item) => Values.Remove(item.Key);

		public bool TryGetValue(string key, [MaybeNullWhen(false)] out NBTValue value) => Values.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();

		public static Negatable<NBTCompound> operator !(NBTCompound nbt) => nbt.Negate();
		public static implicit operator Negatable<NBTCompound>(NBTCompound nbt) => new(nbt);
	}
}
