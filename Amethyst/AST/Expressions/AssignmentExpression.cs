using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Expressions
{
	public class AssignmentExpression(LocationRange loc, Expression dest, Expression expr) : Expression(loc)
	{
		public readonly Expression Dest = dest;
		public readonly Expression Expression = expr;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => Dest.ComputeType(ctx);

		public override ValueRef Execute(FunctionContext ctx)
		{
			var val = Expression.Execute(ctx);
			var dest = Dest.Execute(ctx);

			ctx.Add(new StoreInsn(dest, val));

			return val;
		}
	}
}
