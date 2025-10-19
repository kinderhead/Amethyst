using Datapack.Net.Data;

namespace Geode.Values
{
	public interface IConstantValue : IValue
	{
		NBTValue Value { get; }
	}
}
