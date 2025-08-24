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
            var left = Arg<ValueRef>(0).Expect().AsScore(ctx);
            var right = Arg<ValueRef>(1).Expect().AsScore(ctx);
            //var ret = ReturnValue.Expect<ScoreValue>();

            //ret.Store(new LiteralValue(0), ctx);
            //ctx.Add(new Execute().If.Score(left.Target, left.Score, Op, right.Target, right.Score).Run(new Scoreboard.Players.Set(ret.Target, ret.Score, 1)));
            ReturnValue.SetValue(new ConditionalValue(cmd => cmd.If.Score(left.Target, left.Score, Op, right.Target, right.Score)));
        }

		protected override Value? ComputeReturnValue()
		{
            if (Arguments[0] is ValueRef arg1) ReturnValue.Dependencies.Add(arg1);
            if (Arguments[1] is ValueRef arg2) ReturnValue.Dependencies.Add(arg2);

			var ret = base.ComputeReturnValue();
            if (ret is not null) return ret;
            return new ConditionalValue(cmd => cmd);
		}
    }

    public class EqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "eq";
        public override Comparison Op => Comparison.Equal;
        public override NBTBool Compute(NBTInt left, NBTInt right) => left == right;
    }
}
