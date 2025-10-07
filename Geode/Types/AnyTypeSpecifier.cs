using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Values;

namespace Geode.Types
{
	public class AnyTypeSpecifier : TypeSpecifier
	{
		public override LiteralValue DefaultValue => new(new NBTCompound());
		public override NBTType EffectiveType => NBTType.Compound;
		public override NamespacedID ID => "amethyst:any";

		public override string ToString() => "any";
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is AnyTypeSpecifier;
		public override object Clone() => new AnyTypeSpecifier();
	}
}
