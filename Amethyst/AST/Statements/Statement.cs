using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public abstract class Statement(LocationRange loc) : Node(loc)
	{
		public abstract IEnumerable<Statement> SubStatements { get; }
		public abstract void Compile(FunctionContext ctx);
	}

	public class ExpressionStatement(LocationRange loc, Expression expr) : Statement(loc)
	{
		public readonly Expression Expression = expr;
		public override IEnumerable<Statement> SubStatements => [];

		public override void Compile(FunctionContext ctx)
		{
			Expression.Execute(ctx);
		}
	}
}
