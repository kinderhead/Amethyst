using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.IR;
using Geode.Values;
using System;

namespace Geode.Types
{
	public abstract class RangeType<T> : TypeSpecifier where T : TypeSpecifier
	{
		public override LiteralValue DefaultValue => new("", this);
		public override NBTType EffectiveType => NBTType.String;

		public override object Clone() => throw new NotImplementedException();
		public override string ToString() => ID.Path;
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is RangeType<T>;

		// public override ValueRef? CastToOverload(ValueRef val, FunctionContext ctx)
        // {
        //     if ()
        // }
	}
}
