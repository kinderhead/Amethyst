using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Chains;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Geode.Types
{
	public class TargetSelectorType : TypeSpecifier
	{
		public override LiteralValue DefaultValue => new("@r", this);
		public override NamespacedID ID => "minecraft:target";
		public override NBTType EffectiveType => NBTType.String;
		public override TypeSpecifier BaseClass => PrimitiveType.String;

		public override object Clone() => new TargetSelectorType();
		public override string ToString() => ID.ToString();
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is TargetSelectorType;

		public override ValueRef? CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
		{
			if (to == PrimitiveType.Bool)
			{
				return ctx.Add(new TargetExistsInsn(val));
			}

			return null;
		}

		public override void ExecuteChainOverload(ValueRef val, ExecuteChain chain, FunctionContext ctx, bool invert = false) => chain.Add(new EntityChain(val, invert));
	}
}
