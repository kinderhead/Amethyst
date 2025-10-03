using Datapack.Net.Data;

namespace Datapack.Net.Function
{
	public class Team(string name) : INegatable<Team>
	{
		public readonly string Name = name;

		public Negatable<Team> Negate() => new(this, true);

		public override string ToString() => Name;

		public static Negatable<Team> operator !(Team team) => team.Negate();
		public static implicit operator Negatable<Team>(Team team) => new(team);
	}
}
