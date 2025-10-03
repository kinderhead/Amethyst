using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public class IfStatement(LocationRange loc, Expression expr, Statement stmt, Statement? elseStmt) : Statement(loc)
	{
		public readonly Expression Expression = expr;
		public readonly Statement Statement = stmt;
		public readonly Statement? Else = elseStmt;

		public override void Compile(FunctionContext ctx)
		{
			var cond = Expression.Execute(ctx, null);

			if (Else is not null)
			{
				_ = ctx.Branch(cond, "if", () => Statement.Compile(ctx), () => Else.Compile(ctx));
			}
			else
			{
				_ = ctx.Branch(cond, "if", () => Statement.Compile(ctx));
			}
		}
	}
}
