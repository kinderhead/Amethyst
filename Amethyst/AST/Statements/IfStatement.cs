using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Statements
{
	public class IfStatement(LocationRange loc, Expression expr, Statement stmt, Statement? elseStmt) : Statement(loc)
	{
		public readonly Expression Expression = expr;
		public readonly Statement Statement = stmt;
		public readonly Statement? Else = elseStmt;

		public override void Compile(FunctionContext ctx)
		{
			var chain = new ExecuteChain();

			Expression.ExecuteChain(chain, ctx);

			if (Else is not null)
			{
				ctx.Branch(chain, "if", () => Statement.Compile(ctx), () => Else.Compile(ctx));
			}
			else
			{
				ctx.Branch(chain, "if", () => Statement.Compile(ctx));
			}
		}
	}
}
