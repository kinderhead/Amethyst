using Geode.Types;

namespace Geode.Values
{
	public class MethodValue(FunctionValue val, ValueRef self, FunctionTypeSpecifier? type = null) : FunctionValue(val.ID, type ?? new(val.FuncType.Modifiers, val.FuncType.ReturnType, val.FuncType.Parameters[1..]))
	{
		public readonly FunctionValue BaseFunction = val;
		public readonly ValueRef Self = self;

		public override void Call(RenderContext ctx, ValueRef[] args, bool applyGuard = true) => BaseFunction.Call(ctx, [Self, .. args], applyGuard);

		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new MethodValue(BaseFunction, Self, type);
	}
}
