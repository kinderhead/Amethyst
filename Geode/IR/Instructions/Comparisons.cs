using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.NumberProviders;
using Geode.Chains;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    public abstract class ComparisonInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTBool>(left, right)
    {
        public override TypeSpecifier ReturnType => PrimitiveType.Bool;
        public abstract Comparison Op { get; }
        public virtual bool Invert => false;

        public override void Render(RenderContext ctx)
        {
            var left = Arg<ValueRef>(0).Expect().AsScore(ctx);
            var right = Arg<ValueRef>(1).Expect().AsScore(ctx);

            ReturnValue.Expect<LValue>().Store(new LiteralValue(false), ctx);

            var cmd = new Execute();
            (Invert ? cmd.Unless : cmd.If).Score(left.Target, left.Score, Op, right.Target, right.Score).Run(
                ctx.WithFaux(ctx => ReturnValue.Expect<LValue>().Store(new LiteralValue(true), ctx)).Single());
            ctx.Add(cmd);
        }

        public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
        {
            markOverlap(ReturnValue, Arg<ValueRef>(0));
            markOverlap(ReturnValue, Arg<ValueRef>(1));
        }
    }

    public class EqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "eq";
        public override Comparison Op => Comparison.Equal;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left == right;
    }

    public class NeqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "neq";
        public override Comparison Op => Comparison.Equal;
        public override bool Invert => true;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left != right;
    }

    public class LtInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "lt";
        public override Comparison Op => Comparison.LessThan;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left < right;
    }

    public class LteInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "lte";
        public override Comparison Op => Comparison.LessThanOrEqual;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left <= right;
    }

    public class GtInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "gt";
        public override Comparison Op => Comparison.GreaterThan;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left > right;
    }

    public class GteInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "gte";
        public override Comparison Op => Comparison.GreaterThanOrEqual;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left >= right;
    }

    public class FloatComparisonInsn(ValueRef left, ValueRef right, ComparisonOperator op) : Instruction([left, right])
    {
        public override NBTType?[] ArgTypes => [null, null];
        public override string Name => $"float_{op}";
        public override TypeSpecifier ReturnType => PrimitiveType.Bool;

        public override void Render(RenderContext ctx)
        {
            var leftProvider = ToFloatProvider(Arg<ValueRef>(0).Expect().ToCompute(ctx), ctx);
            var rightProvider = ToFloatProvider(Arg<ValueRef>(1).Expect().ToCompute(ctx), ctx);
            var leftValue = StoreProvider(leftProvider, ctx);
            var rightValue = StoreProvider(rightProvider, ctx);
            var result = ReturnValue.Expect<LValue>().AsScore(ctx);

            switch (op)
            {
                case ComparisonOperator.Lt:
                case ComparisonOperator.Lte:
                case ComparisonOperator.Gt:
                case ComparisonOperator.Gte:
                    var absoluteDifference = StoreProvider(new AbsProvider(
                        new SubProvider(leftProvider, rightProvider, ctx.Builder.RandomID), ctx.Builder.RandomID), ctx);
                    var signedDifference = op is ComparisonOperator.Lt or ComparisonOperator.Lte
                        ? StoreProvider(new SubProvider(rightProvider, leftProvider, ctx.Builder.RandomID), ctx)
                        : StoreProvider(new SubProvider(leftProvider, rightProvider, ctx.Builder.RandomID), ctx);
                    StoreNotEquals(absoluteDifference, signedDifference, result, ctx);
                    Invert(result, ctx);

                    if (op is ComparisonOperator.Lt or ComparisonOperator.Gt)
                    {
                        var notEqual = ctx.Builder.Temp(0, PrimitiveType.Bool);
                        StoreNotEquals(leftValue, rightValue, notEqual, ctx);
                        ctx.Add(new Scoreboard.Players.Operation(result.Target, result.Score, ScoreOperation.Mul,
                            notEqual.Target, notEqual.Score));
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx)
        {
            if (!AreArgsLiteral(out var args) || args[0].Value is not INBTNumber left || args[1].Value is not INBTNumber right) return null;

            var a = Convert.ToSingle(left.RawValue);
            var b = Convert.ToSingle(right.RawValue);

            return new LiteralValue(op switch
            {
                ComparisonOperator.Lt => a < b,
                ComparisonOperator.Lte => a <= b,
                ComparisonOperator.Gt => a > b,
                ComparisonOperator.Gte => a >= b,
                _ => throw new NotImplementedException()
            });
        }

        private static NumberProvider ToFloatProvider(NumberProvider provider, RenderContext ctx) =>
            provider.NumberType == ProviderNumberType.Float
                ? provider
                : new FromIntProvider(provider, ctx.Builder.RandomID);

        private static StorageValue StoreProvider(NumberProvider provider, RenderContext ctx)
        {
            ctx.Builder.Datapack.FloatProviders.Add(provider);
            var value = ctx.Builder.TempStorage(PrimitiveType.Float);
            value.Store(provider, ctx);
            return value;
        }

        private static void StoreNotEquals(DataTargetValue left, DataTargetValue right, ScoreValue result, RenderContext ctx)
        {
            var test = ctx.Builder.TempStorage(PrimitiveType.Float);
            test.Store(left, ctx);
            ctx.Add(result.StoreExecute(false).Run(ctx.WithFaux(ctx => test.Store(right, ctx)).Single()));
        }

        private static void Invert(ScoreValue value, RenderContext ctx)
        {
            var one = ctx.Builder.Constant(1);
            var original = ctx.Builder.Temp(0, PrimitiveType.Bool);
            original.Store(value, ctx);
            value.Store(one, ctx);
            ctx.Add(new Scoreboard.Players.Operation(value.Target, value.Score, ScoreOperation.Sub, original.Target, original.Score));
        }
    }
}