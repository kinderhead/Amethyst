using Geode.Errors;

namespace Amethyst.Errors
{
	public class ConstantReferenceError() : GeodeError("Cannot assign to a constant reference")
	{
	}
}