using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc)
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        public override TypeSpecifier ComputeType(FunctionContext ctx) => Expression.ComputeType(ctx).Property(Property) ?? throw new PropertyError(Expression.ComputeType(ctx).ToString(), Property);

        public override ValueRef Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
