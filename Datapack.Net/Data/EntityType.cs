using Datapack.Net.Utils;

namespace Datapack.Net.Data
{
	public class EntityType(NamespacedID id) : INegatable<EntityType>
	{
		public readonly NamespacedID ID = id;

		public override string ToString() => ID.ToString();

		public Negatable<EntityType> Negate() => new(this, true);

		public static Negatable<EntityType> operator !(EntityType type) => type.Negate();
		public static implicit operator Negatable<EntityType>(EntityType type) => new(type);
	}
}
