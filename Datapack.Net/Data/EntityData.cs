using Datapack.Net.Utils;

namespace Datapack.Net.Data
{
	public class EntityData(NamespacedID id) : INegatable<EntityData>
	{
		public readonly NamespacedID ID = id;

		public override string ToString() => ID.ToString();

		public Negatable<EntityData> Negate() => new(this, true);

		public static Negatable<EntityData> operator !(EntityData type) => type.Negate();
		public static implicit operator Negatable<EntityData>(EntityData type) => new(type);
	}
}
