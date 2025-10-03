using Datapack.Net.Data;

namespace Datapack.Net.Function.Commands
{
	public class RandomCommand(MCRange<int> range, bool value = true, bool macro = false) : Command(macro)
	{
		public readonly bool Value = value;
		public readonly MCRange<int> Range = range;

		protected override string PreBuild() => $"random {(Value ? "value" : "roll")} {Range}";
	}
}
