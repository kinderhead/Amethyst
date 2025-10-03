using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Intrinsics
{
	public class ListSize(FunctionTypeSpecifier? type = null) : Intrinsic("amethyst:list/size", type ?? new(FunctionModifiers.None, PrimitiveTypeSpecifier.Int, [
			new(ParameterModifiers.None, new ReferenceTypeSpecifier(new ListTypeSpecifier(new GenericTypeSpecifier("T"))), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new ListSize(type);

        public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			return ctx.Add(new ListSizeInsn(args[0]));
        }
	}
}
