using Datapack.Net.Function;

namespace Geode.Values
{
	public class StackValue(int offset, Storage storage, string loc, TypeSpecifier type) : StorageValue(storage, $"stack[{offset}].{loc}", type)
	{
		public readonly int Offset = offset;
		public readonly string Location = loc;
	}
}
