using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Values;

namespace Geode.Types
{
	public class VoidType : TypeSpecifier
	{
		public override LiteralValue DefaultValue => new(0, this);
		public override NBTType EffectiveType => NBTType.Int;

		public override NamespacedID ID => "amethyst:void";

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is VoidType;
		public override string ToString() => "void";
		public override object Clone() => new VoidType();
	}
}
