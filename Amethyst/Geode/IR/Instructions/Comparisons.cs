using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
    public abstract class ComparisonInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTBool>(left, right)
    {
        public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Bool;
        public abstract Comparison Op { get; }

        public override void Render(RenderContext ctx)
        {
            var left = Arg<ValueRef>(0).Expect().AsScore(ctx, 0);
            var right = Arg<ValueRef>(1).Expect().AsScore(ctx, 1);
            var ret = ReturnValue.Expect<ScoreValue>();

            ret.Store(new LiteralValue(0), ctx);
            ctx.Add(new Execute().If.Score(left.Target, left.Score, Op, right.Target, right.Score).Run(new Scoreboard.Players.Set(ret.Target, ret.Score, 1)));
        }
    }

    public class EqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "eq";
        public override Comparison Op => Comparison.Equal;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left == right;
    }
}
