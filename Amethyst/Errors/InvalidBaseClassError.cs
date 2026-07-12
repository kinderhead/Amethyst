using Geode.Errors;

namespace Amethyst.Errors
{
	public class InvalidBaseClassError(string type)
		: GeodeError($"Type {type} is not a valid base class in this context")
	{
	}
}