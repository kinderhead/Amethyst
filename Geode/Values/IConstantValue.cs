using Datapack.Net.Data;

namespace Geode.Values
{
	public interface IConstantValue : IValue
	{
		new NBTValue Value { get; }
	}
}
