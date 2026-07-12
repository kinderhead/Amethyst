namespace Geode.IR.Instructions
{
	public interface IJumpInsn : IBlockCapstoneInsn
	{
		Block DestBlock { get; }
	}
}