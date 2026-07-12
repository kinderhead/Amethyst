namespace Geode.IR.Instructions
{
	public interface IStoreInsn : IBasicInsn
	{
		ValueRef Value { get; }
		ValueRef Variable { get; }
	}
}