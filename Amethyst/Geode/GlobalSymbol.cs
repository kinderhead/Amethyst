using Amethyst.AST;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
	public readonly record struct GlobalSymbol(NamespacedID ID, LocationRange Location, Value Value, Node Node);
	public readonly record struct LocalSymbol(string Name, LocationRange Location, Value Value);
}
