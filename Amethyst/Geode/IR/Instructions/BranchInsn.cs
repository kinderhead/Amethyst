using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
    public class BranchInsn(ValueRef cond, Block ifTrue, Block ifFalse) : Instruction([cond, ifTrue, ifFalse])
    {
        public override string Name => "br";
        public override NBTType?[] ArgTypes => [NBTType.Boolean, null, null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            var cond = Arg<ValueRef>(0).Expect();
            var ifTrue = Arg<Block>(1);
            var ifFalse = Arg<Block>(2);

            var returning = ctx.Func.GetIsFunctionReturningValue();

            ctx.Add(cond.If(new()).Run(new FunctionCommand(ifTrue.Function)));
            ctx.Add(new Execute().Unless.Data(returning.Storage, returning.Path).Run(new FunctionCommand(ifFalse.Function)));
        }

        protected override Value? ComputeReturnValue() => new VoidValue();
    }
}
