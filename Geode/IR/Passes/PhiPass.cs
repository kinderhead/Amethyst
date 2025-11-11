using Geode.IR.Instructions;

namespace Geode.IR.Passes
{
    public class PhiPass : Pass
    {
		protected override void OnInsn(FunctionContext ctx, Block block, Instruction insn)
        {
            if (insn is IPhiLike p)
            {
                p.Process(block);
            }
        }
    }

    public interface IPhiLike
    {
        void Process(Block block);
    }
}
