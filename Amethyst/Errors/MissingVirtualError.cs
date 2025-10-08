using Geode.Errors;

namespace Amethyst.Errors
{
    public class MissingVirtualError(string name) : GeodeError($"Method \"{name}\" is not marked virtual")
    {

    }
}
