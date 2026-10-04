using Datapack.Net.Data;
using Geode.Types;

namespace Geode.IR.Instructions
{
    public class ComputeInsn(Equation eq) : Instruction([eq])
    {
        public override NBTType?[] ArgTypes => [null];
        public override string Name => "compute";
        public override TypeSpecifier ReturnType => PrimitiveType.Int;

        public override void Render(RenderContext ctx)
        {
            var eq = Arg<Equation>(0).Render(ctx);
            ctx.Builder.Datapack.IntProviders.Add(eq);
            ReturnValue.Expect<LValue>().Store(eq, ctx);
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
    }
}