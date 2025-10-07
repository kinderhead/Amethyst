using Amethyst.Errors;
using Amethyst.IR.Instructions;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Types
{
	public class WeakReferenceTypeSpecifier(TypeSpecifier inner) : ReferenceTypeSpecifier(inner)
	{
		public override string ToString() => $"{Inner}^";
		public override NamespacedID ID => "amethyst:weak_ref";

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
		{
			if (val.Type.Implements(Inner))
			{
				if (val.IsLiteral)
				{
					throw new ReferenceError(val.Name);
				}

				return ctx.Add(new WeakReferenceInsn(val));
			}

			if (val.Type is ReferenceTypeSpecifier r && r.Inner.Implements(Inner))
			{
				return val;
			}
			else
			{
				return base.CastToOverload(val, ctx);
			}
		}

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is WeakReferenceTypeSpecifier p && p.Inner == Inner;
		public override object Clone() => new WeakReferenceTypeSpecifier((TypeSpecifier)Inner.Clone());

		public static new LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new WeakReferenceTypeSpecifier(val.Type));
	}
}
