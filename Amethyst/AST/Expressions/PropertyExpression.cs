using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc)
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        // public override TypeSpecifier ComputeType(FunctionContext ctx) => Expression.ComputeType(ctx).Property(Property) ?? throw new PropertyError(Expression.ComputeType(ctx).ToString(), Property);

        protected override ValueRef _Execute(FunctionContext ctx)
        {
            return ctx.GetProperty(Expression.Execute(ctx), Property);
        }
    }
}
