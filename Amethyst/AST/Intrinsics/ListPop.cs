using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Intrinsics
{
	public class ListPop(FunctionType? type = null) : Intrinsic("amethyst:list/pop", type ?? new(FunctionModifiers.None, new GenericType("T"), [
			new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new ListPop(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			var list = args[0];

			var item = ctx.Add(new DereferenceInsn(ctx.Add(new IndexInsn(list, new LiteralValue(-1))), true));
			ctx.Add(new ListRemoveInsn(list, new LiteralValue(-1)));

			return item;
		}
	}
}
