using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc)
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
            return ctx.GetProperty(Expression.Execute(ctx, null), Property);
        }
    }
}
