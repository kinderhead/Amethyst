using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, string cmd) : Statement(loc)
	{
		public readonly string Command = cmd;
		public override IEnumerable<Statement> Statements => [];

		public override void Compile(FunctionContext ctx)
		{
			ctx.Add(new CommandInsn(Command));
		}
	}
}
