using Geode;
using Geode.Errors;

namespace Amethyst.Errors
{
	public class InvalidCastError(TypeSpecifier oldType, TypeSpecifier newType) : GeodeError($"{oldType} cannot be casted to {newType}")
	{
	}
}
