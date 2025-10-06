using Amethyst.Errors;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class ReferenceTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner]; // Shouldn't need to unecessarily include the base subtypes here
		public override LiteralValue DefaultValue => new("");
		public override string ToString() => $"{Inner}&";
		public override NBTType EffectiveType => NBTType.String;
		public override TypeSpecifier BaseClass => this;
		public override NamespacedID ID => "amethyst:ref";

		public override TypeSpecifier AssignmentOverloadType => Inner;

		public override void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx)
		{
			if (val.Type is ReferenceTypeSpecifier)
			{
				ctx.Call("amethyst:core/ref/set-ref", dest, ctx.ImplicitCast(val, this));
			}
			else
			{
				ctx.Add(new StoreRefInsn(dest, ctx.ImplicitCast(val, Inner)));
			}
		}

		public override ValueRef? CastOverload(ValueRef val, FunctionContext ctx)
		{
			if (val.Type.Implements(Inner))
			{
				if (val.IsLiteral)
				{
					throw new ReferenceError(val.Name);
				}

				if (val.Value is Variable v)
				{
					return v.ToReference(ctx);
				}

				return ctx.Add(new ReferenceInsn(val));
			}
			else if (val.Type is WeakReferenceTypeSpecifier weak && weak.Inner.Implements(Inner))
			{
				return ctx.Add(new ResolveWeakRefInsn(val));
			}

			return null;
		}

		public override TypeSpecifier? DefaultPropertyType => Inner.DefaultPropertyType;
		public override Dictionary<string, TypeSpecifier> Properties => Inner.Properties;

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is ReferenceTypeSpecifier p && p.Inner == Inner;
		public override object Clone() => new ReferenceTypeSpecifier((TypeSpecifier)Inner.Clone());

		public virtual ValueRef Deref(ValueRef src, FunctionContext ctx) => ctx.Add(new DereferenceInsn(src));
		public static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new ReferenceTypeSpecifier(val.Type));
	}
}
