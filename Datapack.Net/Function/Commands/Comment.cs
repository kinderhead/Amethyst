namespace Datapack.Net.Function.Commands
{
	public class Comment(string comment) : Command(false)
	{
		public readonly string Value = comment;

		protected override string PreBuild() => $"#{Value}";
	}
}
