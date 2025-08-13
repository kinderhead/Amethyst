using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;

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
			throw new NotImplementedException();
		}
	}
}
