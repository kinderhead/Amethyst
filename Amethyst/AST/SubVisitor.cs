using Geode;

namespace Amethyst.AST
{
	public class SubVisitor(Visitor parent, Location offset) : Visitor(parent.Filename, parent.Compiler)
	{
		public readonly Visitor Parent = parent;
		public readonly Location Offset = offset;

		public override Location LocOffset(Location loc) => new(loc.File, loc.Line + Offset.Line - 1, loc.Column + Offset.Column - 1);
	}
}
