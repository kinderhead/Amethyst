using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class CommandInsn(string cmd) : Instruction([])
	{
		public readonly string Command = cmd;
		public override string Name => "cmd";
		public override NBTType?[] ArgTypes => [];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx) =>
			//ctx.PossibleErrorChecker(new RawCommand(Command), text => text.Text(": Inline command failed: ").Text(Command, new FormattedText.Modifiers { Color = "red", Underlined = true, SuggestCommand = $"/{Command}" }));
			ctx.Add(new RawCommand(Command));

		public override string Dump(Func<IInstructionArg, string> valueMap) => $"/{Command}";
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
