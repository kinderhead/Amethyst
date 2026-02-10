using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Intrinsics
{
	public class Random(FunctionType? type = null) : Intrinsic("minecraft:random", type ?? new(FunctionModifiers.None, PrimitiveType.Int, [
			new(ParameterModifiers.None, PrimitiveType.Int, "min"),
			new(ParameterModifiers.None, PrimitiveType.Int, "max")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Random(type);
		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 2)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			return ctx.Add(new RandomInsn(ctx.ImplicitCast(args[0], PrimitiveType.Int), ctx.ImplicitCast(args[1], PrimitiveType.Int)));
		}
	}
}
