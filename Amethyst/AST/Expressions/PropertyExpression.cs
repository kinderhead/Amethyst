using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class PropertyExpression(LocationRange loc, Expression expression, string prop) : Expression(loc), IPropertyLikeExpression
	{
		public readonly Expression Expression = expression;
		public readonly string Property = prop;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var val = Expression.Execute(ctx, null);

			if (val.Type is ReferenceType r && r.Inner is EntityType e)
			{
				val = ctx.ImplicitCast(val, e);
			}

			return ctx.GetProperty(val, Property);
		}
	}
}
