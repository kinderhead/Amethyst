using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    public class BranchInsn(ExecuteChain cond, Block ifTrue, Block ifFalse, bool inFork) : Instruction([cond, ifTrue, ifFalse]), IBranchInsn
    {
        public readonly bool InFork = inFork;
        public override NBTType?[] ArgTypes => [null, null, null];
        public override string Name => "br";
        public override TypeSpecifier ReturnType => new VoidType();

        public Block[] Destinations => [Arg<Block>(1), Arg<Block>(2)];

        public override void Render(RenderContext ctx)
        {
            var cond = Arg<ExecuteChain>(0);
            var ifTrue = Arg<Block>(1);
            var ifFalse = Arg<Block>(2);

            cond.RunWithPropagate(macros => ctx.JumpTo(ifTrue, macros), ctx);

            var returning = ctx.Func.GetIsFunctionReturningValue();
            var forking = ctx.Func.GetIsForkingEndValue();

            foreach (var i in ctx.JumpTo(ifFalse))
            {
                ctx.Add(InFork
                    ? new Execute().Unless.Data(returning.Storage, returning.Path).Unless.Data(forking.Storage, forking.Path).Run(i)
                    : new Execute().Unless.Data(returning.Storage, returning.Path).Run(i));
            }
        }

        public override void OnAdd(Block block)
        {
            var ifTrue = Arg<Block>(1);
            var ifFalse = Arg<Block>(2);

            block.LinkNext(ifTrue);
            block.LinkNext(ifFalse);
        }

        protected override IValue ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }

    public class ClearForkEndInsn() : Instruction([])
    {
        public override NBTType?[] ArgTypes => [];
        public override string Name => "clear_fork";
        public override TypeSpecifier ReturnType => new VoidType();
        public override bool HasSideEffects => true;

        public override void Render(RenderContext ctx)
        {
            ctx.Add(new DataCommand.Remove(ctx.Func.GetIsForkingEndValue().Target));
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
    }

    public class SetForkEndInsn() : Instruction([])
    {
        public override NBTType?[] ArgTypes => [];
        public override string Name => "set_fork";
        public override TypeSpecifier ReturnType => new VoidType();
        public override bool HasSideEffects => true;

        public override void Render(RenderContext ctx)
        {
            ctx.Func.GetIsForkingEndValue().Store(new LiteralValue(1), ctx);
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
    }
}