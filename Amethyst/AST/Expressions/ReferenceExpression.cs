using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Expressions
{
	public class ReferenceExpression(LocationRange loc, Expression inner) : Expression(loc)
	{
		public readonly Expression Inner = inner;

		protected override ValueRef _Execute(FunctionContext ctx) => ctx.Add(new ReferenceInsn(Inner.Execute(ctx)));
	}
}
