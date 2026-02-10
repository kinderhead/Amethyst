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
	public class Contains(FunctionType? type = null) : Intrinsic("minecraft:nbt/contains", type ?? new(FunctionModifiers.None, PrimitiveType.Bool, [
			new(ParameterModifiers.None, new ReferenceType(PrimitiveType.Compound), "this"),
			new(ParameterModifiers.None, new UnsafeStringType(), "tag")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Contains(type);
		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 2)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			return ctx.Add(new ContainsInsn(args[0], ctx.ImplicitCast(args[1], new UnsafeStringType())));
		}
	}
}
