using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
    public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc), IPropertyLikeExpression, IMethodHolder
    {
        public readonly Expression Expression = expression;
        public readonly string Property = prop;

        private ValueRef? expr;

        public Expression GetThis(FunctionContext ctx) => new ValueRefExpression(Expression.Location, GetExpr(ctx));
        private ValueRef GetExpr(FunctionContext ctx) => expr ??= Expression.Execute(ctx, new VarType());

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var val = GetExpr(ctx);

            if (val.Type is ReferenceType { Inner: EntityType e }) val = ctx.ImplicitCast(val, e);

            var ret = ctx.GetProperty(val, Property);

            if (expected is null && ret.Type is ReferenceType ptr) ret = ctx.ImplicitCast(ret, ptr.Inner);

            return ret;
        }
    }
}