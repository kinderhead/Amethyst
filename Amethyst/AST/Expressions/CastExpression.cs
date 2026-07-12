using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class CastExpression(LocationRange loc, AbstractTypeSpecifier type, Expression expr) : Expression(loc)
	{
		public readonly Expression Expression = expr;
		public readonly AbstractTypeSpecifier Type = type;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) =>
			ctx.ExplicitCast(Expression.Execute(ctx, null), Type.Resolve(ctx));
	}
}