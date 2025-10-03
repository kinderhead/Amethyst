namespace Amethyst.AST
{
	public abstract class Node(LocationRange loc) : ILocatable
	{
		public LocationRange Location { get; } = loc;
	}
}
