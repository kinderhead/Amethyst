namespace Amethyst.Geode.IR
{
    public interface IInstructionArg
    {
        /// <summary>
        /// Empty string if no name
        /// </summary>
        public string Name { get; }
		public HashSet<ValueRef> Dependencies { get; }
	}
}
