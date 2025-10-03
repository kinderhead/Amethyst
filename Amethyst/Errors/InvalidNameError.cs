namespace Amethyst.Errors
{
	public class InvalidNameError(string name) : AmethystError($"\"{name}\" is not a valid name here")
	{
	}
}
