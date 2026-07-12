namespace Geode.IR.Instructions
{
	public interface ILoadInsn : IBasicInsn
	{
		ValueRef Variable { get; }
	}
}