namespace Amethyst.Geode.Values
{
	public class MethodValue(FunctionValue val, ValueRef self) : FunctionValue(val.ID, new(val.FuncType.Modifiers, val.FuncType.ReturnType, val.FuncType.Parameters[1..]))
    {
        public readonly FunctionValue BaseFunction = val;
        public readonly ValueRef Self = self;

        public override void Call(RenderContext ctx, ValueRef[] args)
        {
            BaseFunction.Call(ctx, [Self, .. args]);
        }

        public override FunctionValue CloneWithType(FunctionTypeSpecifier type) => new MethodValue(BaseFunction.CloneWithType(type), Self);
    }
}
