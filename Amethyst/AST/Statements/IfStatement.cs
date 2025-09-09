using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
	public class IfStatement(LocationRange loc, Expression expr, Statement stmt, Statement? elseStmt) : Statement(loc)
	{
		public readonly Expression Expression = expr;
		public readonly Statement Statement = stmt;
		public readonly Statement? Else = elseStmt;
		public override IEnumerable<Statement> Statements => Else is null ? [Statement] : [Statement, Else];

		public override void Compile(FunctionContext ctx)
		{
			var cond = ctx.Add(new LoadInsn(Expression.Execute(ctx)));

			if (Else is not null) ctx.Branch(cond, "if", () => Statement.Compile(ctx), () => Else.Compile(ctx));
			else ctx.Branch(cond, "if", () => Statement.Compile(ctx));
		}
	}
}
