using Datapack.Net.Function;

namespace Geode.Values
{
	public class Variable(string name, Storage storage, string baseLoc, int frame, TypeSpecifier type)
		: StackValue(-1, storage, $"{baseLoc}{frame}.{name}", type)
	{
		public readonly string BaseLocation = baseLoc;
		public readonly int Frame = frame;
		public readonly string Name = name;

		// TODO: find somewhere else to put this
		public Variable? Pointer = null;
		public bool ForceStack { get => field || HasReference; set => field = value; }
		public bool HasReference { get => field || Pointer is not null; set => field = value; }

		public override string ToString() => Name;
	}
}