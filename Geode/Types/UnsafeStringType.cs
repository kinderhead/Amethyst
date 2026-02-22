using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Values;
using System.Text.RegularExpressions;

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

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
        {
            if (val.Type == PrimitiveType.String && val.Value is IConstantValue c && c.Value is NBTString str)
            {
                if (InvalidUnsafeString().IsMatch(str.Value))
                {
                    throw new UnsafeStringError();
                }

                return LiteralValue.Raw(str.Value);
            }

            return null;
        }

		public override ValueRef? ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx)
        {
            if (to == PrimitiveType.String)
            {
                return val;
            }

            return null;
        }

		public override ValueRef? ExplicitCastToOverload(ValueRef val, FunctionContext ctx)
        {
			if (val.Type == PrimitiveType.String && val.Value is IConstantValue c && c.Value is NBTString str && !InvalidUnsafeString().IsMatch(str.Value))
			{
				return LiteralValue.Raw(str.Value);
			}
			else if (val.Type == PrimitiveType.String)
            {
                return val;
            }

            return null;
        }

		[GeneratedRegex(@"[^a-zA-Z0-9\-_\+\.]")]
		private static partial Regex InvalidUnsafeString();
	}
}
