using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
    public enum LogicalOperation
    {
        And,
        Or
    }

    public class LogicalExpression(LocationRange loc, Expression left, LogicalOperation op, Expression right) : Expression(loc)
    {
        public readonly Expression Left = left;
        public readonly LogicalOperation Op = op;
        public readonly Expression Right = right;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var ret = ctx.RegisterLocal(GeodeBuilder.UniqueString, PrimitiveType.Bool, Location);

            // TODO: Optimize for if statements after all that optimization has been implemented
            if (Op == LogicalOperation.Or)
            {
                var left = new ExecuteChain();
                Left.ExecuteChain(left, ctx);
                ctx.Branch(left, "or.left",
                    () => ctx.Add(new StoreInsn(ret, new LiteralValue(true))),
                    () =>
                    {
                        var right = new ExecuteChain();
                        Right.ExecuteChain(right, ctx);
                        ctx.Add(new StoreInsn(ret, ctx.Add(new TrueInsn(right))));
                    }
                );
            }
            else
            {
                var left = new ExecuteChain();
                Left.ExecuteChain(left, ctx);
                ctx.Branch(left, "and.left",
                    () =>
                    {
                        var right = new ExecuteChain();
                        Right.ExecuteChain(right, ctx);
                        ctx.Add(new StoreInsn(ret, ctx.Add(new TrueInsn(right))));
                    },
                    () => ctx.Add(new StoreInsn(ret, new LiteralValue(false)))
                );
            }

            return ret;
        }
    }
}