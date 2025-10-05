namespace Amethyst.Errors
{
	public class MissingConstructorError(string type) : AmethystError($"Missing constructor call for type \"{type}\"")
	{
	}
}
