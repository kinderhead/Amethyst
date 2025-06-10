using Amethyst.Codegen;
using Amethyst.Codegen.Functions;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using CommandLine;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
	{
		public readonly Expression Function = func;
		public readonly List<Expression> Args = args;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx)
		{
			var funcType = Function.ComputeType(ctx);
			if (funcType is FunctionTypeSpecifier f) return f.ReturnType;
			throw new CallError(Location);
		}

		protected override Value _Execute(FunctionContext ctx)
		{
			var func = Function.Execute(ctx);

			if (func is CompileTimeFunction cfunc) return cfunc.Execute(ctx, [.. Args.Select(i => i.Execute(ctx))]);
			else if (func is StaticFunctionValue f) return ctx.Call(f.ID, PrepArgs(ctx, func));
			else throw new CallError(Location);
		}

        protected override void _Store(FunctionContext ctx, MutableValue dest)
        {
			var func = Function.Execute(ctx);

			if (func is CompileTimeFunction cfunc) dest.Store(ctx, cfunc.Execute(ctx, [.. Args.Select(i => i.Execute(ctx))]));
			else if (func is StaticFunctionValue f)
			{
				if (dest is StorageValue s) ctx.Call(f.ID, PrepArgs(ctx, func), s);
				else
				{
					var tmp = ctx.AllocTemp(f.FuncType.ReturnType);
					ctx.Call(f.ID, PrepArgs(ctx, func), tmp);
					dest.Store(ctx, tmp);
				}

			}
			else throw new CallError(Location);
		}

		private List<Value> PrepArgs(FunctionContext ctx, Value func)
		{
			if (func is StaticFunctionValue f)
			{
				if (f.FuncType.Parameters.Length != Args.Count) throw new MismatchedArgumentCountError(Location, f.FuncType.Parameters.Length, Args.Count);

				var args = new List<Value>();

				if (f.FuncType.Parameters.Length != 0)
				{
					for (var i = 0; i < Args.Count; i++)
					{
						args.Add(Args[i].Cast(f.FuncType.Parameters[i].Type).Execute(ctx));
					}
				}

				return args;
			}
			else throw new CallError(Location);
		}
	}
}
