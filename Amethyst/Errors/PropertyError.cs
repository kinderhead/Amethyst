namespace Amethyst.Errors
{
	public class PropertyError : AmethystError
	{
		public PropertyError(string type, string prop) : base($"{type} does not have property with name \"{prop}\"")
		{
		}

		public PropertyError(string type) : base($"{type} cannot have properties")
		{
		}
	}
}
