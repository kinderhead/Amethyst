using Geode;
using Geode.IR;
using Geode.IR.Instructions;

namespace Amethyst.AST.Expressions
{
	public class NotExpression(LocationRange loc, Expression val) : Expression(loc)
	{
		public readonly Expression Value = val;

		public override void ExecuteChain(ExecuteChain chain, FunctionContext ctx, bool invert = false)
		{
			Value.ExecuteChain(chain, ctx, !invert);
		}

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var chain = new ExecuteChain();
			Value.ExecuteChain(chain, ctx);
			return ctx.Add(new NotInsn(chain));
		}
	}
}
