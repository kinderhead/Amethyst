using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class PrintInsn(IEnumerable<ValueRef> args) : Instruction(args)
	{
		public override string Name => "print";
		public override NBTType?[] ArgTypes => [];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var msg = new FormattedText();

			var tmp = 0;
			foreach (var i in Arguments)
			{
#pragma warning disable CA1859
				if (i is not ValueRef vref || vref.Value is not IValue val)
				{
					throw new InvalidOperationException($"Invalid print argument of type {i.GetType().Name}");
				}
#pragma warning restore CA1859

				if (val is RawDataTargetValue macro && macro.RawTarget.Contains("$("))
				{
					var tmpStk = new StackValue(-1, ctx.Builder.RuntimeID, $"render{tmp++}", val.Type);
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
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
