using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class StoreInsn(Variable dest, ValueRef src) : Instruction([dest, src])
    {
        public override string Name => "store";
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override NBTType?[] ArgTypes => [null, null];

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
