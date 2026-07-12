using Geode.Errors;

namespace Amethyst.Errors
{
	public class CannotNegateArgumentError(string arg)
		: GeodeError($"Cannot negate argument \"{arg}\" in target selector")
	{
	}
}