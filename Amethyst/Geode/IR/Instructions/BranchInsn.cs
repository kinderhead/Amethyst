using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class BranchInsn(ValueRef cond, Block ifTrue, Block ifFalse) : Instruction([cond, ifTrue, ifFalse])
    {
        public override string Name => "br";
        public override NBTType?[] ArgTypes => [NBTType.Boolean, null, null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            throw new NotImplementedException();
        }

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
