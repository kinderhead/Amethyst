using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.IR;
using Geode.Values;
using System;

namespace Geode.Types
{
	public abstract class RangeType(TypeSpecifier inner) : TypeSpecifier
	{
        public readonly TypeSpecifier Inner = inner;
		public override LiteralValue DefaultValue => new("", this);
		public override NBTType EffectiveType => NBTType.String;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner];

		public override bool WrapInQuotesForMacro => true;

		public override string ToString() => ID.Path;
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is RangeType r && r.Inner == Inner;

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
        {
            if (val.Type == Inner)
            {
                // This probably won't cause issues
                return val;
            }

            return null;
        }
	}

	public class IntRangeType() : RangeType(PrimitiveType.Int)
	{
		public override NamespacedID ID => "minecraft:int_range";

        public override ValueRef? CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
        {
            if (to == new FloatRangeType())
            {
                return val;
            }

            return null;
        }

		public override object Clone() => new IntRangeType();
	}

    // Use doubles to get rid of the f postfix
    public class FloatRangeType() : RangeType(PrimitiveType.Double)
    {
        public override NamespacedID ID => "minecraft:float_range";

		public override object Clone() => new FloatRangeType();
	}
}
