using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class IntrinsicMethod(Intrinsic func, ValueRef self, FunctionTypeSpecifier? type = null) : Intrinsic(func.ID, type ?? new(func.FuncType.Modifiers, func.FuncType.ReturnType, func.FuncType.Parameters[1..]))
	{
		public readonly Intrinsic BaseFunction = func;
		public readonly ValueRef Self = self;

		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new IntrinsicMethod(BaseFunction, Self, type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args) => BaseFunction.Execute(ctx, [Self, .. args]);
	}
}
