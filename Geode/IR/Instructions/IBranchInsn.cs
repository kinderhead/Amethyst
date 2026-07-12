namespace Geode.IR.Instructions
{
	public interface IBranchInsn : IBlockCapstoneInsn
	{
		Block[] Destinations { get; }
	}
}