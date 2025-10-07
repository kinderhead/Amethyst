using Geode.Errors;

namespace Amethyst.Errors
{
	public class WeakReferenceError() : GeodeError("Cannot safely convert a non-constant weak reference into a fully qualified reference")
	{

	}
}
