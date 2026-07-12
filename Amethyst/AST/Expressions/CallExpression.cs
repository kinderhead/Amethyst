using Amethyst.Errors;
using Amethyst.IR;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
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
		public readonly List<Expression> Args = args;
		public readonly Expression Function = func;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var func = ReferenceType.TryDeref(Function.Execute(ctx, null), ctx);

			Expression[] newArgs;

			if (Function is PropertyExpression prop)
			{
				// Make sure the `this` parameter isn't dereferenced
				newArgs =
				[
					new ValueRefExpression(prop.Expression.Location, prop.Expression.Execute(ctx, new VarType())),
					.. Args
				];
			}
			else
			{
				newArgs = [.. Args];
			}

			if (func.Value is Intrinsic i)
			{
				return i.Execute(ctx, [.. newArgs.Select(i => i.Execute(ctx, null))]);
			}

			ValueRef[]? args = null;

			if (func.Value is OverloadedFunctionValue overload)
			{
				args = [.. newArgs.Select(i => i.Execute(ctx, null))];
				var types = TypeArray.From(args);
				var options = overload.Get(types);

				if (options.Length == 0)
				{
					throw new NoOverloadError(overload.ID, types);
				}

				if (options.Length > 1)
				{
					throw new AmbiguousOverloadError(overload.ID, types);
				}

				// args[0] here is the `this` parameter
				func = Function is PropertyExpression
					? ReferenceType.TryDeref(ctx.GetProperty(args[0], options[0].id
						.GetFile()), ctx)
					: new(ctx.GetVariable(options[0].id.ToString()));
			}

			if (func.Type is not FunctionType type)
			{
				throw new InvalidTypeError(func.Type.ToString(), "function");
			}

			args ??= [.. newArgs.Zip(type.Parameters).Select(i => i.First.Execute(ctx, i.Second.Type))];

			if (func.Value is FunctionValue f)
			{
				return f.CallBehavior(ctx, args);
			}

			ctx.Add(new PushFuncArgsInsn(type, ctx.PrepArgs(type, args)));
			return ctx.Add(new DynCallInsn(func));
		}
	}
}