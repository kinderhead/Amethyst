using Amethyst.Codegen;
using Amethyst.Codegen.Functions;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
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
			List<Value> parameters = [.. Args.Select(i => i.Execute(ctx))];

			if (func is CompileTimeFunction cfunc) return cfunc.Execute(ctx, parameters);
			else if (func is StaticFunctionValue f)
			{
				ctx.Add(new CallInstruction(Location, f.ID));
				return new VoidValue();
			}

			throw new CallError(Location);
		}
	}
}
