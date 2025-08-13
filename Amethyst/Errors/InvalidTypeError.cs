namespace Amethyst.Errors
{
	public class InvalidTypeError : AmethystError
	{
		public InvalidTypeError(string type) : base($"type \"{type}\" is not valid here")
		{
		}

		public InvalidTypeError(string type, string expected) : base($"expected \"{expected}\" but got type \"{type}\"")
		{
		}
	}
}
