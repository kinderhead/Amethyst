using System.Text.RegularExpressions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Geode.Types
{
    public partial class UnsafeStringType : TypeSpecifier
    {
        public override LiteralValue DefaultValue => new("", this);
        public override NamespacedID ID => "minecraft:unsafe_string";
        public override NBTType EffectiveType => NBTType.String;
        public override bool WrapInQuotesForMacro => true;

        public override object Clone() => new UnsafeStringType();
        public override string ToString() => "unsafe_string";
        protected override bool EqualsImpl(TypeSpecifier obj) => obj is UnsafeStringType;

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type == PrimitiveType.String && val.Value is IConstantValue { Value: NBTString str })
            {
                if (InvalidUnsafeString().IsMatch(str.Value)) throw new UnsafeStringError();

                recorder.Record(LiteralValue.Raw(str.Value));
            }
        }

        public override void ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
            if (to == PrimitiveType.String) recorder.Record(val);
        }

        public override void ExplicitCastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type == PrimitiveType.String && val.Value is IConstantValue { Value: NBTString str } && !InvalidUnsafeString().IsMatch(str.Value))
                recorder.Record(LiteralValue.Raw(str.Value));

            else if (val.Type == PrimitiveType.String) recorder.Record(val);
        }

        [GeneratedRegex(@"[^a-zA-Z0-9\-_\+\.]")]
        private static partial Regex InvalidUnsafeString();
    }
}