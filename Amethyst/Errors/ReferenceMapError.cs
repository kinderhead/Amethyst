using Geode.Errors;

namespace Amethyst.Errors
{
	public class ReferenceMapError()
		: GeodeError("Native maps do not support garbage collection and cannot store references")
	{
	}
}