using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class CallInsn(ValueRef func, IEnumerable<ValueRef> args) : Instruction([func, .. args])
    {
        public override string Name => "call";
        public override NBTType?[] ArgTypes => [null, .. FuncType.Parameters.Select(i => i.Type.EffectiveType)];
        public override TypeSpecifier ReturnType => FuncType.ReturnType;
        public FunctionTypeSpecifier FuncType => Arg<ValueRef>(0).Expect<FunctionValue>().FuncType;

        public override void Render(RenderContext ctx)
        {
            var func = Arg<ValueRef>(0).Expect<FunctionValue>();
            func.Call(ctx, [.. Arguments[1..].Cast<ValueRef>()]);
            if (ReturnValue.Expect() is LValue ret) ret.Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            if (ReturnType is VoidTypeSpecifier) return new VoidValue();
            return null;
        }
    }
}
