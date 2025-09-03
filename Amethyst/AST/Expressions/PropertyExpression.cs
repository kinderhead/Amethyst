using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc)
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        public override TypeSpecifier ComputeType(FunctionContext ctx) => Expression.ComputeType(ctx).Property(Property) ?? throw new PropertyError(Expression.ComputeType(ctx).ToString(), Property);

        public override ValueRef Execute(FunctionContext ctx)
        {
            return ctx.Add(new LoadInsn(ctx.GetProperty(Expression.ExecuteWithoutLoad(ctx), Property)));
        }

		public override ValueRef ExecuteWithoutLoad(FunctionContext ctx)
		{
			return ctx.GetProperty(Expression.ExecuteWithoutLoad(ctx), Property);
		}
    }
}
