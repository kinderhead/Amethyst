using Datapack.Net.Utils;
using Geode.Errors;
using Geode.Values;

namespace Geode.Types
{
	public class VarTypeSpecifier : TypeSpecifier
	{
		public override LiteralValue DefaultValue => throw new InvalidTypeError("var");

		public override NamespacedID ID => "amethyst:var";

		public override string ToString() => "var";
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is VarTypeSpecifier;
		public override object Clone() => new VarTypeSpecifier();
	}
}
