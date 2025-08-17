using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class JumpInsn(Block dest) : Instruction([dest])
    {
        public override string Name => "jump";
        public override NBTType?[] ArgTypes => [null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            throw new NotImplementedException();
        }

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
