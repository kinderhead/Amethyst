using Amethyst.AST;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
	public record class GlobalSymbol(NamespacedID ID, LocationRange Location, Value Value, Node? Node = null);
	public record class GlobalTypeSymbol(NamespacedID ID, LocationRange Location, TypeSpecifier Type, Node? Node = null);
}
