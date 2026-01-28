using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;
using System;

namespace Amethyst.AST.Expressions
{
	public class RangeExpression(LocationRange loc, Expression? min, Expression? max) : Expression(loc)
	{
        public readonly Expression? Min = min;
        public readonly Expression? Max = max;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var type = expected is RangeType t ? t : new IntRangeType();
            var min = Min?.Execute(ctx, type.Inner) ?? new LiteralValue(new NBTRawString(""), new UnsafeStringType());
            var max = Max?.Execute(ctx, type.Inner) ?? new LiteralValue(new NBTRawString(""), new UnsafeStringType());

            return ctx.Add(new RangeInsn(min, max, type));
        }
	}
}
