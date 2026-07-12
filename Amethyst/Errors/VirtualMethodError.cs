using Geode.Errors;

namespace Amethyst.Errors
{
	public class VirtualMethodError() : GeodeError("Virtual methods are not supported in this context")
	{
	}
}