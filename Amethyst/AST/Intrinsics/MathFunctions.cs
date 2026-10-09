using Geode;
using Geode.Equations;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
    public abstract class MathIntrinsic(string id, Parameter[] parameters, FunctionType? type = null)
        : Intrinsic(id, type ?? new(FunctionModifiers.None, PrimitiveType.Float, parameters))
    {
        public override bool SupportsEquationCompute => true;

        public override Equation Compute(FunctionContext ctx, params Equation[] args)
        {
            if (args.Length != FuncType.Parameters.Count()) throw new MismatchedArgumentCountError(FuncType.Parameters.Count(), args.Length);
            return ComputeImpl(ctx, args);
        }

        public override ValueRef CallBehavior(FunctionContext ctx, params ValueRef[] args)
        {
            return ctx.Add(new ComputeInsn(Compute(ctx, [.. args.Select(arg => new ValueRefEquation(arg))])));
        }

        protected abstract Equation ComputeImpl(FunctionContext ctx, Equation[] args);
    }

    public abstract class UnaryMathIntrinsic(string id, UnaryOperation operation, string parameterName, FunctionType? type = null)
        : MathIntrinsic(id, [new(ParameterModifiers.None, PrimitiveType.Float, parameterName)], type)
    {
        protected override Equation ComputeImpl(FunctionContext ctx, Equation[] args) => new UnaryOperationEquation(args[0], operation);
    }

    public class Abs(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/abs", UnaryOperation.Abs, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Abs(type);
    }

    public class Sqrt(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/sqrt", UnaryOperation.Sqrt, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Sqrt(type);
    }

    public class Sin(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/sin", UnaryOperation.Sin, "angle", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Sin(type);
    }

    public class Cos(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/cos", UnaryOperation.Cos, "angle", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Cos(type);
    }

    public class Round(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/round", UnaryOperation.Round, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Round(type);
    }

    public class Floor(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/floor", UnaryOperation.Floor, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Floor(type);
    }

    public class Ceil(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/ceil", UnaryOperation.Ceil, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Ceil(type);
    }

    public class Truncate(FunctionType? type = null) : UnaryMathIntrinsic("minecraft:math/trunc", UnaryOperation.Trunc, "value", type)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new Truncate(type);
    }
}
