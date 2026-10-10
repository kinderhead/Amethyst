using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Geode.Types
{
    public class PrimitiveType(NBTType type) : TypeSpecifier
    {
        public readonly NBTType Type = type;

        public override bool IsList =>
            Type is NBTType.List or NBTType.IntArray or NBTType.LongArray or NBTType.ByteArray;

        public override NBTType EffectiveType => Type;
        public override NamespacedID ID => $"minecraft:{this}";

        public override TypeSpecifier? DefaultPropertyType => Type == NBTType.Compound ? Compound : null;

        public override LiteralValue DefaultValue => Type switch
        {
            NBTType.Boolean => new(false),
            NBTType.Byte => new(new NBTByte(0)),
            NBTType.Short => new(new NBTShort(0)),
            NBTType.Int => new(new NBTInt(0)),
            NBTType.Long => new(new NBTLong(0)),
            NBTType.Float => new(new NBTFloat(0)),
            NBTType.Double => new(new NBTDouble(0)),
            NBTType.String => new(new NBTString("")),
            NBTType.List => new(new NBTList()),
            NBTType.Compound => new(new NBTCompound()),
            _ => throw new NotImplementedException()
        };

        public override TypeSpecifier BaseClass => Type switch
        {
            NBTType.Boolean => Byte,
            NBTType.Byte => Int,
            NBTType.Short => Int,
            _ => Compound
        };

        public static PrimitiveType Int => new(NBTType.Int);
        public static PrimitiveType Long => new(NBTType.Long);
        public static PrimitiveType Float => new(NBTType.Float);
        public static PrimitiveType Double => new(NBTType.Double);
        public static PrimitiveType Bool => new(NBTType.Boolean);
        public static PrimitiveType Short => new(NBTType.Short);
        public static PrimitiveType Byte => new(NBTType.Byte);
        public static PrimitiveType String => new(NBTType.String);
        public static PrimitiveType Compound => new(NBTType.Compound);
        public static PrimitiveType List => new(NBTType.List);

        protected override bool EqualsImpl(TypeSpecifier obj) => obj is PrimitiveType p && p.Type == Type;

        public override string ToString() => Type switch
        {
            NBTType.Compound => "nbt",
            NBTType.Boolean => "bool",
            _ => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException()
        };

        public override object Clone() => new PrimitiveType(Type);

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Value is LiteralValue { Value.NumberType: not null } literal && EffectiveNumberType is { } destType)
                recorder.Record(new LiteralValue(literal.Value.Cast(destType)));
            else if (val.Type == Int && Type == NBTType.Float) recorder.Record(new LoadNBTInsn(val, this));
            // What was I on about? If something breaks then I'll fix it then
            // else if (Type is NBTType.Double or NBTType.Float && recorder.TryImplicitCast(val, Int) is { } toFloat) return toFloat;
        }

        public override void ExplicitCastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (Type is NBTType.Int)
            {
                recorder.Record(new LoadInsn(val, this));
                return;
            }

            if (EffectiveNumberType is not null && val.Type.EffectiveNumberType is not null) recorder.Record(new LoadNBTInsn(val, this));
        }
    }
}