using Amethyst.AST.Expressions;
using Geode;
using Geode.IR;

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

			if (Expression is IExecuteChainExpression e)
			{
				e.ExecuteChain(chain, ctx);
			}
			else
            {
				throw new NotImplementedException();
            }

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
