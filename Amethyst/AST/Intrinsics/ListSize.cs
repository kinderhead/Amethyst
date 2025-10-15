using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class ListSize(FunctionType? type = null) : Intrinsic("amethyst:list/size", type ?? new(FunctionModifiers.None, PrimitiveType.Int, [
			new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new ListSize(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args) => ctx.Add(new ListSizeInsn(args[0]));
	}
}
