using Amethyst.AST;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
	public readonly record struct GlobalSymbol(NamespacedID ID, LocationRange Location, Value Value, Node? Node = null);
	public readonly record struct GlobalTypeSymbol(NamespacedID ID, LocationRange Location, TypeSpecifier Type, Node? Node = null);
}
