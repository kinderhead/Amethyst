using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class StringLength(FunctionTypeSpecifier? type = null) : Intrinsic("minecraft:string/length", type ?? new(FunctionModifiers.None, new VoidTypeSpecifier(), [
			new(ParameterModifiers.None, new ReferenceTypeSpecifier(PrimitiveTypeSpecifier.String), "this")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new StringLength(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args) => ctx.ExplicitCast(ctx.ImplicitCast(args[0], PrimitiveTypeSpecifier.String), PrimitiveTypeSpecifier.Int);
	}
}
