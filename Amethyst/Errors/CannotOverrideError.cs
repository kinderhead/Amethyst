using Geode.Errors;

namespace Amethyst.Errors
{
	public class CannotOverrideError(string name) : GeodeError($"Method \"{name}\" cannot be overridden")
	{

	}
}
