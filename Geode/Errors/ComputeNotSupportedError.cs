using Datapack.Net.Pack;

namespace Geode.Errors
{
    public class ComputeNotSupportedError(PackFormat packFormat, bool disabled) : GeodeError(disabled
        ? $"float comparisons require compute, but compute is disabled"
        : $"float comparisons require compute, which is not available for pack format {packFormat} (requires >=121.0)")
    {
    }
}
