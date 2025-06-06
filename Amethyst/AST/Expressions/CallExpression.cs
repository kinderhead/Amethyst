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
			else if (func is StaticFunctionValue f)
			{
				if (f.FuncType.Parameters.Length != Args.Count) throw new MismatchedArgumentCountError(Location, f.FuncType.Parameters.Length, Args.Count);

				var args = new Dictionary<string, Value>();
				var macros = new Dictionary<string, Value>();

				if (f.FuncType.Parameters.Length != 0)
				{
					//var argHolder = ctx.AllocTemp(new PrimitiveTypeSpecifier(NBTType.Compound));

					for (var i = 0; i < Args.Count; i++)
					{
						var val = Args[i].Cast(f.FuncType.Parameters[i].Type).Execute(ctx);
						if ((f.FuncType.Parameters[i].Modifiers & ParameterModifiers.Macro) == ParameterModifiers.Macro) macros[f.FuncType.Parameters[i].Name] = val;
						else args[f.FuncType.Parameters[i].Name] = val;
					}

					// if (args.Count != 0)
					// {
					// 	ctx.Add(new PopulateInstruction(Location, argHolder, args));
					// 	ctx.Add(new StackPushInstruction(Location, argHolder));
					// }
				}

				ctx.Add(new CallInstruction(Location, f.ID, args, macros));

				if (f.FuncType.ReturnType is VoidTypeSpecifier) return new VoidValue();
				else return new StorageValue(Compiler.RuntimeID, "return", f.FuncType.ReturnType);
			}

			throw new CallError(Location);
		}
	}
}
