using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
    public class RangeExpression(LocationRange loc, Expression? min, Expression? max) : Expression(loc)
    {
        public readonly Expression? Max = max;
        public readonly Expression? Min = min;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var type = expected as RangeType ?? new IntRangeType();

            var min = Min?.Execute(ctx, null) ?? new LiteralValue(new NBTRawString(""), new UnsafeStringType());
            var max = Max?.Execute(ctx, null) ?? new LiteralValue(new NBTRawString(""), new UnsafeStringType());

            if (min.Type == PrimitiveType.Float || min.Type == PrimitiveType.Double || max.Type == PrimitiveType.Float || max.Type == PrimitiveType.Double)
                type = new FloatRangeType();

            return ctx.Add(new RangeInsn(min, max, type));
        }
    }
}