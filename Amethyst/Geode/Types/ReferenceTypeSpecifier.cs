using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.Types
{
	public class ReferenceTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner]; // Shouldn't need to unecessarily include the base subtypes here
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new("");
		public override string ToString() => $"{Inner}&";
		public override NBTType EffectiveType => NBTType.String;
		public override string BasePath => "amethyst";

		protected override bool AreEqual(TypeSpecifier obj) => obj is ReferenceTypeSpecifier p && p.Inner == Inner;
		public static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new ReferenceTypeSpecifier(val.Type));
		public override object Clone() => new ReferenceTypeSpecifier((TypeSpecifier)Inner.Clone());

		public override void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx)
		{
			ctx.Add(new StoreRefInsn(dest, ctx.ImplicitCast(val, Inner)));
		}
	}
}
