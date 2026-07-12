using Datapack.Net.Utils;

namespace Geode
{
	public record GlobalSymbol(NamespacedID ID, LocationRange Location, IValue Value);

	public record GlobalTypeSymbol(NamespacedID ID, LocationRange Location, TypeSpecifier Type);
}