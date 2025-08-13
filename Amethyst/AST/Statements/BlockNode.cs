using Amethyst.Errors;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public class BlockNode(LocationRange loc) : Statement(loc)
	{
		private readonly List<Statement> statements = [];
		public override IEnumerable<Statement> Statements => statements;

		public override void Compile(FunctionContext ctx)
		{
			if (!CompileWithErrorChecking(ctx)) throw new EmptyAmethystError();
		}

		public void Add(Statement stmt) => statements.Add(stmt);

		public bool CompileWithErrorChecking(FunctionContext ctx)
		{
			var success = true;

			foreach (var i in Statements)
			{
				if (!ctx.Compiler.WrapError(i.Location, () => i.Compile(ctx))) success = false;
			}

			return success;
		}
	}
}
