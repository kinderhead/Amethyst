using Amethyst.Errors;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Statements
{
	public class BlockNode(LocationRange loc) : Statement(loc)
	{
		public readonly List<Statement> Statements = [];

		public override void Compile(FunctionContext ctx)
		{
			if (!CompileWithErrorChecking(ctx))
			{
				throw new EmptyAmethystError();
			}
		}

		public void Prepend(Statement stmt) => Statements.Insert(0, stmt);
		public void Add(Statement stmt) => Statements.Add(stmt);

		public bool CompileWithErrorChecking(FunctionContext ctx)
		{
			var success = true;

			foreach (var i in Statements)
			{
				if (!ctx.Compiler.WrapError(i.Location, ctx, () => i.Compile(ctx)))
				{
					success = false;
				}
			}

			return success;
		}
	}
}
