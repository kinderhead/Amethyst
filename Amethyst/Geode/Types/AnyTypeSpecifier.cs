using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.Types
{
	public class AnyTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new(new NBTCompound());
		public override NBTType EffectiveType => NBTType.Compound;
		public override string ToString() => "any";
		protected override bool AreEqual(TypeSpecifier obj) => obj is AnyTypeSpecifier;
		public override object Clone() => new AnyTypeSpecifier();
	}
}
