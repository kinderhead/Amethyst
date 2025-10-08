using Amethyst.IR.Instructions;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
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

			if (func.Type is not FunctionTypeSpecifier type)
			{
				throw new InvalidTypeError(func.Type.ToString(), "function");
			}

			ValueRef[] args = [.. Args.Zip(type.Parameters).Select(i => i.First.Execute(ctx, i.Second.Type))];

			if (func.Value is FunctionValue f)
			{
				return ctx.Call(f, args);
			}
			else
			{
				ctx.Add(new PushFuncArgsInsn(type, ctx.PrepArgs(type, args)));
				return ctx.Add(new DynCallInsn(func));
			}
		}
	}
}
