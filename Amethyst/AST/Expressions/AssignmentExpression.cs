using Datapack.Net.Function.Commands;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
    public enum AssignmentType
    {
        Normal,
        Addition = ScoreOperation.Add,
        Subtraction = ScoreOperation.Sub,
        Multiplication = ScoreOperation.Mul,
        Division = ScoreOperation.Div,
        Modulus = ScoreOperation.Mod
    }

    public class AssignmentExpression(LocationRange loc, Expression dest, AssignmentType type, Expression expr) : Expression(loc)
    {
        public readonly Expression Dest = dest;
        public readonly Expression Expression = expr;
        public readonly AssignmentType Type = type;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var dest = Dest.Execute(ctx, new VarType());
            var val = Expression.Execute(ctx, dest.Type.AssignmentOverloadType, false);

            if (Type != AssignmentType.Normal)
            {
                val = new ArithmeticExpression(Location, new ValueRefExpression(Location, dest), (ScoreOperation)Type, new ValueRefExpression(Location, val))
                    .Execute(ctx, dest.Type);
            }

            dest.Type.AssignmentOverload(dest, val, ctx);

            return val;
        }
    }
}