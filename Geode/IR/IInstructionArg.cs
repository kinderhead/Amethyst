namespace Geode.IR
{
	public interface IInstructionArg
	{
		/// <summary>
		///     Empty string if no name
		/// </summary>
		string Name { get; }

		IReadOnlySet<ValueRef> Dependencies { get; }
		void ReplaceValue(ValueRef value, ValueRef with);
	}
}