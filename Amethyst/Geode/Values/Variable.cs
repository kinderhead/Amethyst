namespace Amethyst.Geode.Values
{
	public class Variable(string name, string loc, TypeSpecifier type) : StackValue(-1, loc, type)
	{
		public readonly string Name = name;
		public override string ToString() => Name;
	}
}