using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class StringLength(FunctionType? type = null) : Intrinsic("minecraft:string/length", type ?? new(FunctionModifiers.None, new VoidType(), [
			new(ParameterModifiers.None, new ReferenceType(PrimitiveType.String), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new StringLength(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args) => ctx.ExplicitCast(ctx.ImplicitCast(args[0], PrimitiveType.String), PrimitiveType.Int);
	}
}
