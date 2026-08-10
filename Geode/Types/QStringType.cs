using System.Text.RegularExpressions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Geode.Types
{
    public partial class QStringType : TypeSpecifier
    {
        public override LiteralValue DefaultValue => new("", this);
        public override NamespacedID ID => "minecraft:qstring";
        public override NBTType EffectiveType => NBTType.String;
        public override bool WrapInQuotesForMacro => true;
        public override TypeSpecifier BaseClass => PrimitiveType.String;

        public override object Clone() => new QStringType();
        public override string ToString() => "qstring";
        protected override bool EqualsImpl(TypeSpecifier obj) => obj is QStringType;

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type == PrimitiveType.String && val.Value is IConstantValue { Value: NBTString str })
            {
                if (InvalidQString().IsMatch(str.Value)) throw new UnsafeStringError();
                recorder.Record(val);
            }
        }

        [GeneratedRegex(@"""")]
        private static partial Regex InvalidQString();
    }
}