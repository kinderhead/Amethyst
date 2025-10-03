using Datapack.Net.Data;

namespace Datapack.Net.Function
{
	public class EntityTag(string name) : INegatable<EntityTag>
	{
		public readonly string Name = name;

		public Negatable<EntityTag> Negate() => new(this, true);

		public override string ToString() => Name;

		public static Negatable<EntityTag> operator !(EntityTag entityTag) => entityTag.Negate();
		public static implicit operator Negatable<EntityTag>(EntityTag entityTag) => new(entityTag);
	}
}
