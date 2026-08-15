using Datapack.Net.Utils;

namespace Geode
{
    public record GlobalSymbol(NamespacedID ID, LocationRange Location, IValue Value);
}