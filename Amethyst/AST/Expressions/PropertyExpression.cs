using Amethyst.IR;
using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc), IPropertyLikeExpression
	{
		public readonly Expression Expression = expression;
		public readonly string Property = prop;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => ctx.GetProperty(Expression.Execute(ctx, null), Property);
	}
}
