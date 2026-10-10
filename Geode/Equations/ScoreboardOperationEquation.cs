using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.NumberProviders;

namespace Geode.Equations
{
    public class ScoreboardOperationEquation(Equation left, ScoreOperation op, Equation right) : Equation([], [left, right])
    {
        public readonly ScoreOperation Op = op;

        public override ProviderNumberType Type => Children[0].Type == ProviderNumberType.Float || Children[1].Type == ProviderNumberType.Float
            ? ProviderNumberType.Float
            : ProviderNumberType.Int;

        public override NumberProvider Render(RenderContext ctx) => Op switch
        {
            ScoreOperation.Add => new AddProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Sub => new SubProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Mul => new MulProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Div => new DivProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            ScoreOperation.Mod => new ModProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue Execute(NBTValue[] args) => Type == ProviderNumberType.Int
            ? (NBTInt)(Op switch
        {
            ScoreOperation.Add => args[0].CastInt() + args[1].CastInt(),
            ScoreOperation.Sub => args[0].CastInt() - args[1].CastInt(),
            ScoreOperation.Mul => args[0].CastInt() * args[1].CastInt(),
            ScoreOperation.Div => args[0].CastInt() / args[1].CastInt(),
            ScoreOperation.Mod => args[0].CastInt() % args[1].CastInt(),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        })
            : (NBTFloat)(Op switch
            {
                ScoreOperation.Add => args[0].CastFloat() + args[1].CastFloat(),
                ScoreOperation.Sub => args[0].CastFloat() - args[1].CastFloat(),
                ScoreOperation.Mul => args[0].CastFloat() * args[1].CastFloat(),
                ScoreOperation.Div => args[0].CastFloat() / args[1].CastFloat(),
                ScoreOperation.Mod => args[0].CastFloat() % args[1].CastFloat(),
                _ => throw new NotImplementedException("Operation not supported for compute yet")
            });

        public override NBTValue? IsConstant() => null;
    }
}