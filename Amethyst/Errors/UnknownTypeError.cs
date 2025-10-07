using Geode.Errors;

namespace Amethyst.Errors
{
	public class UnknownTypeError(string type) : GeodeError($"type \"{type}\" is not declared")
	{
	}
}
