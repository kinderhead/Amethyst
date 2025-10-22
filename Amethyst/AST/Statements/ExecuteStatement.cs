using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Statements
{
	public class ExecuteStatement(LocationRange loc, IEnumerable<ExecuteStatementSubcommand> subCommands, Statement stmt, Statement? elseStmt) : Statement(loc)
	{
		public readonly ExecuteStatementSubcommand[] SubCommands = [.. subCommands];
		public readonly Statement Statement = stmt;
		public readonly Statement? Else = elseStmt;

		public override void Compile(FunctionContext ctx)
		{
			var chain = new ExecuteChain();

			foreach (var i in SubCommands)
			{
				i.Compute(chain, ctx);
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

	public abstract class ExecuteStatementSubcommand(LocationRange loc) : Node(loc)
	{
		public abstract void Compute(ExecuteChain chain, FunctionContext ctx);
	}

	public class IfSubcommand(LocationRange loc, Expression expr) : ExecuteStatementSubcommand(loc)
	{
		public readonly Expression Expression = expr;

		public override void Compute(ExecuteChain chain, FunctionContext ctx) => Expression.ExecuteChain(chain, ctx);
	}

	public class AsSubcommand(LocationRange loc, Expression expr) : ExecuteStatementSubcommand(loc)
	{
		public readonly Expression Expression = expr;

		public override void Compute(ExecuteChain chain, FunctionContext ctx) => chain.Add(new AsChain(Expression.Execute(ctx, new TargetSelectorType())));
	}

	public class AtSubcommand(LocationRange loc, Expression expr) : ExecuteStatementSubcommand(loc)
	{
		public readonly Expression Expression = expr;

		public override void Compute(ExecuteChain chain, FunctionContext ctx) => chain.Add(new AtChain(Expression.Execute(ctx, new TargetSelectorType())));
	}
}
