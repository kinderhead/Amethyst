using Geode.Errors;

namespace Amethyst.Errors
{
	public class InvalidNameError(string name) : GeodeError($"\"{name}\" is not a valid name here")
	{
	}
}
