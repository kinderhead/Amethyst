using Geode.Errors;

namespace Amethyst.Errors
{
	public class ModifierError(string msg) : GeodeError(msg)
	{
	}
}
