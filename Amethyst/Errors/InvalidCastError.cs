using Amethyst.Geode;

namespace Amethyst.Errors
{
	public class InvalidCastError(TypeSpecifier oldType, TypeSpecifier newType) : AmethystError($"{oldType} cannot be casted to {newType}")
	{
	}
}
