using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Intrinsics
{
	public class ListWhere(FunctionType? type = null) : Intrinsic("amethyst:list/where", type ?? new(FunctionModifiers.None, new ListType(new GenericType("T")), [
			new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this"),
			new(ParameterModifiers.None, PrimitiveType.Compound, "predicate")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new ListWhere(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 2)
			{
				throw new MismatchedArgumentCountError(2, args.Length);
			}

			var list = args[0];
			var predicate = args[1];

			return ctx.Add(new ListWhereInsn(list, ctx.ImplicitCast(predicate, PrimitiveType.Compound)));
		}
	}
}
