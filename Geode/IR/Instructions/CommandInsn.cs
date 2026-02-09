using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;
using System.Text;

namespace Geode.IR.Instructions
{
	public class CommandInsn(params IEnumerable<ValueRef> args) : Instruction(args)
	{
		public override string Name => "cmd";
		public override NBTType?[] ArgTypes => [];
		public override TypeSpecifier ReturnType => new VoidType();
		public override bool HasSideEffects => true;

		public override void Render(RenderContext ctx)
		{
			//ctx.PossibleErrorChecker(new RawCommand(Command), text => text.Text(": Inline command failed: ").Text(Command, new FormattedText.Modifiers { Color = "red", Underlined = true, SuggestCommand = $"/{Command}" }));
			
			ctx.Macroize([.. Arguments.Where(i => i is ValueRef).Cast<ValueRef>().Select(i => i.Expect())], (args, ctx) =>
			{
				var builder = new StringBuilder();

				foreach (var i in args)
				{
					builder.Append(i.Value.ToString());
				}

				ctx.Add(new RawCommand(builder.ToString()));
			});
		}

		public override void CheckArguments() { }
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
