using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class ListTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override NBTType EffectiveType => NBTType.List;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner];
		public override bool IsList => true;
		public override TypeSpecifier BaseClass => PrimitiveTypeSpecifier.List;
		public override NamespacedID ID => "amethyst:list";

		public override LiteralValue DefaultValue => new(new NBTList(), this);

		// public override bool IsAssignableTo(TypeSpecifier other) => other.EffectiveType == NBTType.List || base.IsAssignableTo(other);
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is ListTypeSpecifier arr && arr.Inner == Inner;
		public override string ToString() => $"{Inner}[]";
		public override object Clone() => new ListTypeSpecifier((TypeSpecifier)Inner.Clone());
	}
}
