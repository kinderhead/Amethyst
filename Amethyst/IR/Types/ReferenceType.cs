using Amethyst.Errors;
using Amethyst.IR.Instructions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Amethyst.IR.Types
{
	public class ReferenceType(TypeSpecifier inner) : TypeSpecifier
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
			if (val.Type is ReferenceType)
			{
				ctx.Call("amethyst:core/ref/set-ref", dest, ctx.ImplicitCast(val, this));
			}
			else
			{
				ctx.Add(new StoreRefInsn(dest, ctx.ImplicitCast(val, Inner)));
			}
		}

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
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
			else if (val.Type is WeakReferenceType weak && weak.Inner.Implements(Inner))
			{
				return ctx.Add(new ResolveWeakRefInsn(val));
			}

			return null;
		}

		public override ValueRef? CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
		{
			if (Inner.Implements(to))
			{
				return Deref(val, ctx);
			}

			return null;
		}

		public override ValueRef? ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
		{
			if (to.EffectiveType == NBTType.Int)
			{
				return ctx.Add(new LoadInsn(Deref(val, ctx), to));
			}

			return null;
		}

		public override TypeSpecifier? DefaultPropertyType => Inner.DefaultPropertyType;
		public override Dictionary<string, TypeSpecifier> Properties => Inner.Properties;

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is ReferenceType p && p.Inner == Inner;
		public override object Clone() => new ReferenceType((TypeSpecifier)Inner.Clone());

		public virtual ValueRef Deref(ValueRef src, FunctionContext ctx) => ctx.Add(new DereferenceInsn(src));
		public static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new ReferenceType(val.Type));

		public static ValueRef TryDeref(ValueRef src, FunctionContext ctx)
		{
			if (src.Type is ReferenceType r)
			{
				return r.Deref(src, ctx);
			}

			return src;
		}
	}
}
