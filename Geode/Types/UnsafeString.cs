using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Values;
using System;

namespace Geode.Types
{
	public class UnsafeString : TypeSpecifier
	{
		public override LiteralValue DefaultValue => new("", this);
		public override NamespacedID ID => "minecraft:unsafe_string";
		public override NBTType EffectiveType => NBTType.String;
		public override bool WrapInQuotesForMacro => true;

		public override object Clone() => new UnsafeString();
		public override string ToString() => "unsafe_string";
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is UnsafeString;

		public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
        {
            if (val.Type == PrimitiveType.String && val.Value is IConstantValue c && c.Value is NBTString str)
            {
                if (str.Value.Contains('"') || str.Value.Contains('\'') || str.Value.Contains(' '))
                {
                    throw new UnsafeStringError();
                }

                return val;
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
            if (val.Type == PrimitiveType.String)
            {
                return val;
            }

            return null;
        }
	}
}
