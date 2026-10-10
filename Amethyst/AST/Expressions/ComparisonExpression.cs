using Datapack.Net.Data;
using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
    public class ComparisonExpression(LocationRange loc, Expression left, ComparisonOperator op, Expression right) : Expression(loc)
    {
        public readonly Expression Left = left;
        public readonly ComparisonOperator Op = op;
        public readonly Expression Right = right;

        public override void ExecuteChain(ExecuteChain chain, FunctionContext ctx, bool invert = false)
        {
            var left = Left.Execute(ctx, null, false);
            var right = Right.Execute(ctx, null, false);

            if (IsFloatComparison(left, right) && Op is ComparisonOperator.Lt or ComparisonOperator.Lte or ComparisonOperator.Gt or ComparisonOperator.Gte)
            {
                ctx.Compiler.IR.RequireCompute();
                chain.Add(IfValueChain.With(ctx.Add(new FloatComparisonInsn(left, right, Op)), ctx, invert));
                return;
            }

            if ((!NBTValue.IsOperableType(left.Type.EffectiveType) || !NBTValue.IsOperableType(right.Type.EffectiveType)) && Op is ComparisonOperator.Eq or ComparisonOperator.Neq)
            {
                switch (Op)
                {
                    case ComparisonOperator.Eq:
                        new NotExpression(Location, new ValueRefExpression(Location, ctx.Add(new NBTNotEqualsInsn(left, right)))).ExecuteChain(chain, ctx, invert);
                        return;
                    case ComparisonOperator.Neq:
                        new ValueRefExpression(Location, ctx.Add(new NBTNotEqualsInsn(left, right))).ExecuteChain(chain, ctx, invert);
                        return;
                }
            }

            left = ctx.AddLoad(ctx.ImplicitCast(left, PrimitiveType.Int));
            right = ctx.AddLoad(ctx.ImplicitCast(right, PrimitiveType.Int));

            chain.Add(new IfScoreChain(left, Op, right, invert));
        }

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var left = Left.Execute(ctx, null, false);
            var right = Right.Execute(ctx, null, false);

            if (IsFloatComparison(left, right) && Op is ComparisonOperator.Lt or ComparisonOperator.Lte or ComparisonOperator.Gt or ComparisonOperator.Gte)
            {
                ctx.Compiler.IR.RequireCompute();
                return ctx.Add(new FloatComparisonInsn(left, right, Op));
            }

            if ((!NBTValue.IsOperableType(left.Type.EffectiveType) || !NBTValue.IsOperableType(right.Type.EffectiveType)) && Op is ComparisonOperator.Eq or ComparisonOperator.Neq)
            {
                switch (Op)
                {
                    case ComparisonOperator.Eq:
                        return new NotExpression(Location, new ValueRefExpression(Location, ctx.Add(new NBTNotEqualsInsn(left, right)))).Execute(ctx, null);
                    case ComparisonOperator.Neq:
                        return ctx.Add(new NBTNotEqualsInsn(left, right));
                }
            }

            left = ctx.AddLoad(ctx.ImplicitCast(left, PrimitiveType.Int));
            right = ctx.AddLoad(ctx.ImplicitCast(right, PrimitiveType.Int));

            return Op switch
            {
                ComparisonOperator.Eq => ctx.Add(new EqInsn(left, right)),
                ComparisonOperator.Neq => ctx.Add(new NeqInsn(left, right)),
                ComparisonOperator.Lt => ctx.Add(new LtInsn(left, right)),
                ComparisonOperator.Lte => ctx.Add(new LteInsn(left, right)),
                ComparisonOperator.Gt => ctx.Add(new GtInsn(left, right)),
                ComparisonOperator.Gte => ctx.Add(new GteInsn(left, right)),
                _ => throw new NotImplementedException()
            };
        }

        private static bool IsFloatComparison(ValueRef left, ValueRef right) => left.Type.EffectiveType is NBTType.Float || right.Type.EffectiveType is NBTType.Float;
    }
}