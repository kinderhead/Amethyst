using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.IR;
using Geode.Values;

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

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type == Inner)
            {
                // This probably won't cause issues
                recorder.Record(val);
            }
        }
    }

    public class IntRangeType() : RangeType(PrimitiveType.Int)
    {
        public override NamespacedID ID => "minecraft:int_range";

        public override void CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
            if (to == new FloatRangeType()) recorder.Record(val);
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