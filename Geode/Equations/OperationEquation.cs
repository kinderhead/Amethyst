using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.NumberProviders;

namespace Geode.Equations
{
    public class OperationEquation(Equation left, ScoreOperation op, Equation right) : Equation([], [left, right])
    {
        public readonly ScoreOperation Op = op;

        public override NumberProvider Render(RenderContext ctx) => Op switch
        {
            ScoreOperation.Add => new AddProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Sub => new SubProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Mul => new MulProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Div => new DivProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Mod => new ModProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue Execute(NBTValue[] args) => Op switch
        {
            ScoreOperation.Add => args[0].CastInt() + args[1].CastInt(),
            ScoreOperation.Sub => args[0].CastInt() - args[1].CastInt(),
            ScoreOperation.Mul => args[0].CastInt() * args[1].CastInt(),
            ScoreOperation.Div => args[0].CastInt() / args[1].CastInt(),
            ScoreOperation.Mod => args[0].CastInt() % args[1].CastInt(),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue? IsConstant() => null;
    }
}