using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class StoreRefInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
    {
        public override string Name => "store_ref";
        public override NBTType?[] ArgTypes => [null, null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            var dest = Arg<ValueRef>(0).Expect();
            var src = Arg<ValueRef>(1).Expect();

            if (src is DataTargetValue data) ctx.Call("amethyst:core/ref/set-ref", dest, ReferenceTypeSpecifier.From(data));
            else ctx.Call("amethyst:core/ref/set", dest, src);
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
