using Amethyst.Errors;
using Amethyst.Geode.Values;

namespace Amethyst.Geode.Types
{
	public class VarTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => throw new InvalidTypeError("var");

		public override string ToString() => "var";
		protected override bool AreEqual(TypeSpecifier obj) => obj is VarTypeSpecifier;
		public override object Clone() => new VarTypeSpecifier();
	}
}
