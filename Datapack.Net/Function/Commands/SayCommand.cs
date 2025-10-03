namespace Datapack.Net.Function.Commands
{
	public class SayCommand(string message, bool macro = false) : Command(macro)
	{
		public readonly string Message = message;

		protected override string PreBuild() => $"say {Message}";
	}
}
