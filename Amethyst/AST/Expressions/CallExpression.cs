using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
	{
		public readonly Expression Function = func;
		public readonly List<Expression> Args = args;

		public override TypeSpecifier ComputeType(FunctionContext ctx)
		{
			var funcType = Function.ComputeType(ctx);
			if (funcType is FunctionTypeSpecifier f) return f.ReturnType;
			throw new CallError();
		}

		public override ValueRef Execute(FunctionContext ctx)
		{
			var func = Function.ExecuteWithoutLoad(ctx);
			if (func.Value is Intrinsic i) return i.Execute(ctx, Args);

			throw new NotImplementedException();
		}
	}
}
