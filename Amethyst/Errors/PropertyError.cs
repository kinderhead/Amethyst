namespace Amethyst.Errors
{
	public class PropertyError(string type, string prop) : AmethystError($"{type} does not have property with name \"{prop}\"")
	{
	}
}
