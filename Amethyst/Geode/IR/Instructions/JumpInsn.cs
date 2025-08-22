using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
    public class JumpInsn(Block dest) : Instruction([dest])
    {
        public override string Name => "jump";
        public override NBTType?[] ArgTypes => [null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            var block = Arg<Block>(0);
            ctx.Add(new FunctionCommand(block.Function));
        }

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
