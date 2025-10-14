namespace Geode.Values
{
	public class Variable(string name, string baseLoc, int frame, TypeSpecifier type) : StackValue(-1, $"{baseLoc}{frame}.{name}", type)
	{
		public readonly string Name = name;
		public readonly string BaseLocation = baseLoc;
		public readonly int Frame = frame;

		public override string ToString() => Name;

		// TODO: find somewhere else to put this
		public Variable? Pointer = null;
	}
}