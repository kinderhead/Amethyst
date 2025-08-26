using Datapack.Net.Data;
using Datapack.Net.Function;

namespace Amethyst.Geode.IR.Instructions
{
    public class CommandInsn(string cmd) : Instruction([])
    {
        public readonly string Command = cmd;
        public override string Name => "cmd";
        public override NBTType?[] ArgTypes => [];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            ctx.Add(new RawCommand(Command));
        }

        public override string Dump(Func<IInstructionArg, string> valueMap) => $"/{Command}";
        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
