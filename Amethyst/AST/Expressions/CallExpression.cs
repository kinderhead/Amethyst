using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
	{
		public readonly Expression Function = func;
		public readonly List<Expression> Args = args;

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			var func = Function.Execute(ctx);
			if (func.Value is Intrinsic i) return i.Execute(ctx, [.. Args.Select(i => i.Execute(ctx))]);
			else if (func.Value is FunctionValue f) return ctx.Call(f, [.. Args.Select(i => i.Execute(ctx))]);
			else throw new NotImplementedException();
		}
	}
}
