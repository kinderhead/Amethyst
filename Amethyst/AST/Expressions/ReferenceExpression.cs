using Amethyst.IR.Instructions;
using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class ReferenceExpression(LocationRange loc, Expression inner) : Expression(loc)
	{
		public readonly Expression Inner = inner;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => ctx.Add(new ReferenceInsn(Inner.Execute(ctx, null)));
	}
}
