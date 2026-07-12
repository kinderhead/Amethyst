using Geode.Errors;

namespace Amethyst.Errors
{
	public class ReservedNameError(string name) : GeodeError($"Name \"{name}\" is reserved")
	{
	}
}