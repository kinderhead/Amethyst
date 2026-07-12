using Geode.Errors;

namespace Amethyst.Errors
{
	public class ReferencePropertyError(string prop)
		: GeodeError($"Property \"{prop}\" cannot be a reference. Only classes can have reference properties")
	{
	}
}