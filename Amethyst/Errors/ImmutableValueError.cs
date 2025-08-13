namespace Amethyst.Errors
{
	public class ImmutableValueError() : AmethystError("value is not mutable")
	{
	}

	public class MutableValueError() : AmethystError("value is mutable")
	{
	}
}
