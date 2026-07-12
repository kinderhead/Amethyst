namespace Datapack.Net.Function.Commands
{
	public class TellrawCommand(IEntityTarget targets, FormattedText msg, bool macro = false)
		: Command(msg.Macro || macro)
	{
		public readonly FormattedText Message = msg;
		public readonly IEntityTarget Targets = targets;

		protected override string PreBuild() => $"tellraw {Targets.Get()} {Message.Optimize()}";
	}
}