using Geode.Errors;

namespace Amethyst.Errors
{
    public class InvalidOverrideSignatureError(string name) : GeodeError($"Invalid signature to override \"{name}\"")
    {

    }
}
