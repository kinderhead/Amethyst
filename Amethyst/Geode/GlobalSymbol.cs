using Amethyst.AST;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
	public readonly record struct GlobalSymbol(NamespacedID ID, LocationRange Location, Value Value, Node? Node = null);
}
