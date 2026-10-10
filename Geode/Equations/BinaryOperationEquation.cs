using Datapack.Net.Data;
using Datapack.Net.NumberProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geode.Equations
{
    public enum BinaryOperation
    { 
        Pow,
    }

    public class BinaryOperationEquation(Equation left, BinaryOperation op, Equation right): Equation([], [left, right])
    {
        public readonly BinaryOperation Op;

        public override ProviderNumberType Type => Children[0].Type == ProviderNumberType.Float || Children[1].Type == ProviderNumberType.Float
            ? ProviderNumberType.Float
            : ProviderNumberType.Int;

        public override NumberProvider Render(RenderContext ctx) => Op switch
        {
            BinaryOperation.Pow => new PowProvider(Children[0].Render(ctx), Children[1].Render(ctx), ctx.Builder.RandomID),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue Execute(NBTValue[] args) => Op switch
        {
            BinaryOperation.Pow => Type == ProviderNumberType.Int ? (int)Math.Pow(args[0].CastInt(), args[1].CastInt()) : (float)Math.Pow(args[0].CastFloat(), args[1].CastFloat()),
            _ => throw new NotImplementedException("Operation not supported for compute yet")
        };

        public override NBTValue? IsConstant() => null;
    }
}
