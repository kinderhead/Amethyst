using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class ListAdd(FunctionTypeSpecifier? type = null) : Intrinsic("amethyst:list/add", type ?? new(FunctionModifiers.None, new VoidTypeSpecifier(), [
			new(ParameterModifiers.None, new ReferenceTypeSpecifier(new ListTypeSpecifier(new GenericTypeSpecifier("T"))), "this"),
			new(ParameterModifiers.None, new GenericTypeSpecifier("T"), "val")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new ListAdd(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			var list = args[0];
			var val = args[1];

			var type = list.Type is ListTypeSpecifier l ? l.Inner : ((ListTypeSpecifier)((ReferenceTypeSpecifier)list.Type).Inner).Inner;
			return ctx.Add(new ListAddInsn(list, ctx.ImplicitCast(val, type)));
		}
	}
}
