using Geode.Errors;

namespace Amethyst.Errors
{
	public class CannotIndexError(string type) : GeodeError($"Cannot index type {type}")
	{
	}
}