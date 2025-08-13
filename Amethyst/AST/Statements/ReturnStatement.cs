using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public class ReturnStatement(LocationRange loc, Expression? expr) : Statement(loc)
	{
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> Statements => [];

		public override void Compile(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
