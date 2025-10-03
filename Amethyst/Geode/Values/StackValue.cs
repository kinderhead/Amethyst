namespace Amethyst.Geode.Values
{
	public class StackValue(int offset, string loc, TypeSpecifier type) : StorageValue(GeodeBuilder.RuntimeID, $"stack[{offset}].{loc}", type)
	{
		public readonly int Offset = offset;
		public readonly string Location = loc;
	}
}
