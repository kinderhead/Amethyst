using Datapack.Net.Utils;

namespace Datapack.Net.Function
{
	public class Storage(NamespacedID id)
	{
		public readonly NamespacedID ID = id;

		public override string ToString() => ID.ToString();

		public static implicit operator Storage(NamespacedID id) => new(id);
	}

	public class StorageMacro(string raw) : Storage(new())
	{
		public readonly string Value = raw;

		public override string ToString() => Value;
	}
}
