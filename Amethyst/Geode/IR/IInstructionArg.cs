namespace Amethyst.Geode.IR
{
	public interface IInstructionArg
	{
		/// <summary>
		/// Empty string if no name
		/// </summary>
		string Name { get; }
		HashSet<ValueRef> Dependencies { get; }
	}
}
