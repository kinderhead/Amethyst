using Amethyst.Errors;

namespace Amethyst.Geode.IR
{
    public abstract class Pass
    {
        protected virtual bool SkipBlocks { get; } = false;
        protected virtual bool SkipInsns { get; } = false;
        protected virtual bool Reversed { get; } = false;
        private HashSet<Block> toVisit = [];

        public void Apply(FunctionContext ctx)
        {
            if (!ctx.IsFinished) throw new InvalidOperationException($"Function {ctx.Decl.ID} is not finished");

            OnFunction(ctx);

            if (!SkipBlocks)
            {
                toVisit = [.. ctx.Blocks];
                if (Reversed) Walk(ctx, ctx.ExitBlock);
                else Walk(ctx, ctx.FirstBlock);

                while (toVisit.Count != 0)
                {
                    ProcessBlock(ctx, toVisit.First());
                }
            }
        }

        private void Walk(FunctionContext ctx, Block b)
        {
            ProcessBlock(ctx, b);

            foreach (var i in Reversed ? b.Previous : b.Next)
            {
                Walk(ctx, i);
            }
        }

        private void ProcessBlock(FunctionContext ctx, Block b)
        {
            if (!toVisit.Contains(b)) return;
            toVisit.Remove(b);

            OnBlock(ctx, b);

            if (!SkipInsns)
            {
                foreach (var i in Reversed ? b.Instructions.AsEnumerable().Reverse() : b.Instructions)
                {
                    if (!ctx.Compiler.WrapError(i.Location, () =>
                    {
                        OnInsn(ctx, b, i);
                    })) throw new EmptyAmethystError();
                }
            }

            // TODO: Make a collection for insn to be removed instead
            b.Instructions.RemoveAll(x => x.MarkedForRemoval);
        }

        protected void RevisitBlock(Block b, bool visitPredecessors = false)
        {
            toVisit.Add(b);
            if (visitPredecessors)
            {
                foreach (var i in b.Previous)
                {
                    toVisit.Add(i);
                }
            }
        }

        protected virtual void OnFunction(FunctionContext ctx) { }
        protected virtual void OnBlock(FunctionContext ctx, Block block) { }
        protected virtual void OnInsn(FunctionContext ctx, Block block, Instruction insn) { }
    }
}
