using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, string cmd) : Statement(loc)
	{
		public readonly string Command = cmd;
		public override IEnumerable<Statement> Statements => [];

		public override void Compile(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
