using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class AssignmentExpression(LocationRange loc, Expression dest, Expression expr) : Expression(loc)
	{
		public readonly Expression Dest = dest;
		public readonly Expression Expression = expr;

		// public override TypeSpecifier ComputeType(FunctionContext ctx) => Dest.ComputeType(ctx);

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			var val = Expression.Execute(ctx);
			var dest = Dest.Execute(ctx);

			dest.Type.AssignmentOverload(dest, val, ctx);

			return val;
		}
	}
}
