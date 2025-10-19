using Datapack.Net.Utils;

namespace Geode
{
	public record class GlobalSymbol(NamespacedID ID, LocationRange Location, IValue Value);
	public record class GlobalTypeSymbol(NamespacedID ID, LocationRange Location, TypeSpecifier Type);
}
