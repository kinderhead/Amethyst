using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.Types
{
	public class PointerTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner]; // Shouldn't need to unecessarily include the base subtypes here
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new("");
		public override string ToString() => $"{Inner}*";
		public override NBTType EffectiveType => NBTType.String;
		public override string BasePath => "amethyst";

		protected override bool AreEqual(TypeSpecifier obj) => obj is PointerTypeSpecifier p && p.Inner == Inner;
		public static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new PointerTypeSpecifier(val.Type));
		public override object Clone() => new PointerTypeSpecifier((TypeSpecifier)Inner.Clone());
	}
}
