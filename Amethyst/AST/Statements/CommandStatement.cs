using Geode;
using Geode.IR;
using Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, IEnumerable<Expression> cmd) : Statement(loc)
	{
		public readonly Expression[] Fragments = [.. cmd];

		public override void Compile(FunctionContext ctx) =>
			ctx.Add(new CommandInsn(Fragments.Select(i => i.Execute(ctx, null))));
	}
}