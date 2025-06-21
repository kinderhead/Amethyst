using System;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc)
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        protected override TypeSpecifier _ComputeType(FunctionContext ctx) => Expression.ComputeType(ctx).Property(Property) ?? throw new PropertyError(Location, Expression.ComputeType(ctx).ToString(), Property);

        protected override Value _Execute(FunctionContext ctx)
        {
            var obj = Expression.Execute(ctx);
            return obj.GetProperty(ctx, Property);
        }
    }
}
