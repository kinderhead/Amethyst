using Datapack.Net.Data;
using Datapack.Net.NumberProviders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Geode.Equations
{
    public enum UnaryOperation
    { 
        Abs,
        Sqrt,
        Sin,
        Cos,
        Round,
        Floor,
        Ceil,
        Trunc,
    }

    public class UnaryOperationEquation(Equation input, UnaryOperation op): Equation([], [input])
    {
        public readonly UnaryOperation Op = op;
        public override ProviderNumberType Type => input.Type;

        public override NumberProvider Render(RenderContext ctx) => Op switch
        {
            UnaryOperation.Abs      => new AbsProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Sqrt     => new SqrtProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Sin      => new SinProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Cos      => new CosProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Round    => new RoundProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Floor    => new FloorProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Ceil     => new CeilProvider(input.Render(ctx), ctx.Builder.RandomID),
            UnaryOperation.Trunc    => new TruncateProvider(input.Render(ctx), ctx.Builder.RandomID),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue Execute(NBTValue[] args) =>
            Op switch
            {
                UnaryOperation.Abs      => Type == ProviderNumberType.Int ? Math.Abs(args[0].CastInt()) : Math.Abs(args[0].CastFloat()),
                UnaryOperation.Sqrt     => (float)Math.Sqrt(args[0].CastFloat()),
                UnaryOperation.Sin      => (float)Math.Sin(args[0].CastFloat()),
                UnaryOperation.Cos      => (float)Math.Cos(args[0].CastFloat()),
                UnaryOperation.Round    => (float)Math.Round(args[0].CastFloat()),
                UnaryOperation.Floor    => (float)Math.Floor(args[0].CastFloat()),
                UnaryOperation.Ceil     => (float)Math.Ceiling(args[0].CastFloat()),
                UnaryOperation.Trunc    => (float)Math.Truncate(args[0].CastFloat()),
                _ => throw new NotImplementedException("Operation does not support ints")
            };
        public override NBTValue? IsConstant() => null;
    }
}
