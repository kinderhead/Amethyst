using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
	{
		public readonly Expression Function = func;
		public readonly List<Expression> Args = args;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var func = Function.Execute(ctx, null);
			if (func.Value is Intrinsic i)
			{
				return i.Execute(ctx, [.. Args.Select(i => i.Execute(ctx, null))]);
			}
			else if (func.Value is FunctionValue f)
			{
				return ctx.Call(f, [.. Args.Zip(f.FuncType.Parameters).Select(i => i.First.Execute(ctx, i.Second.Type))]);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
