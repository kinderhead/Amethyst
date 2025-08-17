using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class StoreInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
    {
        public override string Name => "store";
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();
        public override NBTType?[] ArgTypes => [null, null];

        public override void Render(RenderContext ctx)
        {
            Arg<ValueRef>(0).Expect<LValue>().Store(Arg<ValueRef>(1).Expect(), ctx);
        }

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
