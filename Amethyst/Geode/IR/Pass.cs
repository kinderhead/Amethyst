namespace Amethyst.Geode.IR
{
    public abstract class Pass
    {
        public void Apply(FunctionContext ctx)
        {
            OnFunction(ctx);

            foreach (var i in ctx.Blocks)
            {
                OnBlock(ctx, i);

                foreach (var e in i.Instructions)
                {
                    OnInsn(ctx, i, e);
                }

                i.Instructions.RemoveAll(x => x.MarkedForRemoval);
            }
        }

        protected virtual void OnFunction(FunctionContext ctx) { }
        protected virtual void OnBlock(FunctionContext ctx, Block block) { }
        protected virtual void OnInsn(FunctionContext ctx, Block block, Instruction insn) { }
    }
}
