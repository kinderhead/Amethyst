using Geode.Errors;

namespace Amethyst.Errors
{
	public class ReferenceError(string val) : GeodeError($"Cannot make a reference to \"{val}\"")
	{

	}
}
