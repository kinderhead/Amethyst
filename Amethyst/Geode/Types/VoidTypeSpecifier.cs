using Amethyst.Geode.Values;

namespace Amethyst.Geode.Types
{
	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new(0, this);

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;
		public override string ToString() => "void";
		public override object Clone() => new VoidTypeSpecifier();
	}
}
