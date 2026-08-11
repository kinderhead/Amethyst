using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
    public class ReferenceInsn(ValueRef src) : Instruction([src])
    {
        public override string Name => "ref";
        public override NBTType?[] ArgTypes => [null];
        public override TypeSpecifier ReturnType => new ReferenceType(Arg<ValueRef>(0).Type);

        public override void Render(RenderContext ctx)
        {
            var val = Arg<ValueRef>(0).Expect<StackValue>();
            ctx.Call(ctx.Func.GetGlobalOrThrow<IMinimalFunction>("amethyst:core/ref/get-stack-ref").Get(new([new UnsafeStringType(), PrimitiveType.Int])).ID,
                new LiteralValue(val.Location),
                new LiteralValue(val.Offset));
            ReturnValue.Expect<LValue>().Store(ctx.Func.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) =>
            Arg<ValueRef>(0).Value is DataTargetValue data and not StackValue ? new LiteralValue(data.Target.GetTarget(), ReturnType) : null;
    }
}