using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
	public class PrintInsn(IEnumerable<ValueRef> args) : Instruction(args)
	{
		public override string Name => "print";
		public override NBTType?[] ArgTypes => [];
		public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

		public override void Render(RenderContext ctx)
		{
			var msg = new FormattedText();

			var tmp = 0;
			foreach (var i in Arguments)
			{
				if (i is not ValueRef vref || vref.Value is not Value val)
				{
					throw new InvalidOperationException($"Invalid print argument of type {i.GetType().Name}");
				}

				if (val is RawDataTargetValue macro && macro.RawTarget.Contains("$("))
				{
					var tmpStk = new StackValue(-1, $"render{tmp++}", val.Type);
					tmpStk.Store(macro, ctx);
					val = tmpStk;
				}

				val.Render(msg, ctx);
				//msg.Text(" ");
			}

			//if (Arguments.Length > 0) msg.RemoveLast();
			msg.Optimize();

			// TODO: Make the target configurable
			ctx.Add(new TellrawCommand(new TargetSelector(TargetType.a), msg));
		}

		public override void CheckArguments() { }
		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
