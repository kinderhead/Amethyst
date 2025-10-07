using Geode.Errors;

namespace Amethyst.Errors
{
	public class MissingConstructorError(string type) : GeodeError($"Missing constructor call for type \"{type}\"")
	{
	}
}
