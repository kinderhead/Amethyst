namespace Amethyst.Geode.IR.Passes
{
	public class ResolvePass : Pass
	{
		protected override void OnInsn(FunctionContext ctx, Block block, Instruction insn) => insn.Resolve(ctx);
	}
}
