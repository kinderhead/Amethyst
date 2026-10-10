using Amethyst.IR.Types;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
    public enum UnaryOperation
    {
        Increment,
        Decrement,
        Negate,
        Reference,
        WeakReference,
        Dereference
    }

    public class UnaryExpression(LocationRange loc, UnaryOperation op, Expression val) : Expression(loc)
    {
        public readonly UnaryOperation Op = op;
        public readonly Expression Value = val;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var val = Value.Execute(ctx, new VarType());

            switch (Op)
            {
                case UnaryOperation.Increment:
                    if (val.Type != PrimitiveType.Int) throw new InvalidTypeError(val.Type.ToString(), "int");

                    var inc = ctx.Add(new AddInsn(ctx.AddLoad(val), new LiteralValue(1)));
                    ctx.Add(new StoreInsn(val, inc));
                    return val;
                case UnaryOperation.Decrement:
                    if (val.Type != PrimitiveType.Int) throw new InvalidTypeError(val.Type.ToString(), "int");

                    var dec = ctx.Add(new SubInsn(ctx.AddLoad(val), new LiteralValue(1)));
                    ctx.Add(new StoreInsn(val, dec));
                    return val;
                case UnaryOperation.Negate:
                    return new ArithmeticExpression(Location, new ValueRefExpression(Location, val), ScoreOperation.Mul, new LiteralExpression(Location, -1))
                        .Execute(ctx, expected);
                case UnaryOperation.Reference:
                    return Value.ReferenceHandler(val, new(val.Type), ctx); // These don't actually cast between references so idk if that will cause issues
                case UnaryOperation.WeakReference:
                    return Value.ReferenceHandler(val, new WeakReferenceType(val.Type), ctx);
                case UnaryOperation.Dereference:
                    return val.Type is not ReferenceType _ ? throw new InvalidTypeError(val.Type.ToString(), "reference") : ReferenceType.Deref(val, ctx);
                default:
                    throw new NotImplementedException();
            }
        }

        protected override Equation ComputeImpl(FunctionContext ctx)
        {
            if (Op == UnaryOperation.Negate) return new ArithmeticExpression(Location, Value, ScoreOperation.Mul, new LiteralExpression(Location, -1)).Compute(ctx);

            return base.ComputeImpl(ctx);
        }
    }
}