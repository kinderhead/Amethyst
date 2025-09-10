using Amethyst.Geode.Values;
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
            var block = Arg<Block>(0);
            ctx.Add(ctx.CallFunction(block.Function));
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
