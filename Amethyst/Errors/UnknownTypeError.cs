namespace Amethyst.Errors
{
	public class UnknownTypeError(string type) : AmethystError($"type \"{type}\" is not declared")
	{
	}
}
