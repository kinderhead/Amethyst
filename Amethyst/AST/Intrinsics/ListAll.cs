using Amethyst.IR;
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
using System.Xml.Linq;

namespace Amethyst.AST.Intrinsics
{
	public class ListAll(FunctionType? type = null) : Intrinsic("amethyst:list/all", type ?? new(FunctionModifiers.None, new WeakReferenceType(new GenericType("T")), [
			new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new ListAll(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			var list = args[0];

			return ctx.Add(new PropertyInsn(list, LiteralValue.Raw("[]"), ((WeakReferenceType)FuncType.ReturnType).Inner));
		}
	}
}
