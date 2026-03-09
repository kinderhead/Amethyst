using Amethyst.IR.Instructions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Types
{
#pragma warning disable CS9107
	public class EntityType(NamespacedID id, TypeSpecifier? baseClass, Dictionary<string, TypeSpecifier> props, Dictionary<string, FunctionValue> methods) : StructType(id, baseClass, props, methods, false)
#pragma warning restore CS9107
	{
		public override LiteralValue DefaultValue => new(0, this);
		public override NBTType EffectiveType => NBTType.Int;

#pragma warning disable IDE0028
		public override object Clone() => new EntityType(ID, BaseClass, new(props.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, i.Value))), Methods);
#pragma warning restore IDE0028
		public override string ToString() => ID.ToString();
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is EntityType other && other.ID == ID;

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
		{
			if (val.Type is TargetSelectorType)
			{
				return ctx.Add(new EntityRefInsn(val));
			}
			else if (val.Type is EntityType other && other.ID == "amethyst:dummy")
			{
				return val;
			}

			return base.CastToOverload(val, ctx);
		}

		public override ValueRef? CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
		{
			if (to is TargetSelectorType)
			{
				return ctx.Add(new EntityToTargetInsn(val));
			}

			return base.CastFromOverload(val, to, ctx);
		}

		public override void ExecuteChainOverload(ValueRef val, ExecuteChain chain, FunctionContext ctx, bool invert = false) => chain.Add(new IfEntityChain(ctx.ImplicitCast(val, new TargetSelectorType()), invert));

		public static readonly EntityType Dummy = new("amethyst:dummy", null, [], []);
	}
}
