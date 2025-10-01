using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class WeakReferenceInsn(ValueRef src) : Instruction([src])
    {
        public override string Name => "ref";
        public override NBTType?[] ArgTypes => [null];
        public override TypeSpecifier ReturnType => new WeakReferenceTypeSpecifier(Arg<ValueRef>(0).Type);

        public override void Render(RenderContext ctx)
        {
            throw new InvalidOperationException();
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            var val = Arg<ValueRef>(0).Expect<DataTargetValue>();
            return WeakReferenceTypeSpecifier.From(val);
        }
    }
}
