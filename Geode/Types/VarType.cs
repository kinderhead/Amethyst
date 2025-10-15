using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.Values;

namespace Geode.Types
{
	public class VarType : TypeSpecifier
	{
		public override LiteralValue DefaultValue => throw new InvalidTypeError("var");
		public override NBTType EffectiveType => NBTType.Compound;

		public override NamespacedID ID => "amethyst:var";

		public override string ToString() => "var";
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is VarType;
		public override object Clone() => new VarType();
	}
}
