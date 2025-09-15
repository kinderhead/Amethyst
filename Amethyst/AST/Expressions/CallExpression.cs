using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;

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

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			var func = Function.Execute(ctx);
			if (func.Value is Intrinsic i) return i.Execute(ctx, Args);
			else if (func.Value is FunctionValue f)
			{
				if (f.FuncType.Parameters.Length != Args.Count) throw new MismatchedArgumentCountError(f.FuncType.Parameters.Length, Args.Count);
				return ctx.Add(new CallInsn(func, Args.Zip(f.FuncType.Parameters).Select(i => ctx.ImplicitCast(i.First.Execute(ctx), i.Second.Type))));
			}
			else throw new NotImplementedException();
		}
	}
}
