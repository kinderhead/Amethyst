using Geode.Errors;

namespace Amethyst.Errors
{
    public class TargetSelectorArgumentError(string arg) : GeodeError($"Invalid target selector argument \"{arg}\"")
    {

    }
}
