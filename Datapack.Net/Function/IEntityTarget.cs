namespace Datapack.Net.Function
{
	public interface IEntityTarget
	{
		string Get();

		bool IsOne();

		IEntityTarget RequireOne()
		{
			if (!IsOne())
			{
				throw new ArgumentException($"Entity target is not singular: {Get()}");
			}

			return this;
		}
	}

	public readonly record struct NamedTarget(string name) : IEntityTarget
	{
		public readonly string Name = name;

		public string Get() => Name;
		public bool IsOne() => true;
	}
}
