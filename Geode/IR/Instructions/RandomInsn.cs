using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;

namespace Geode.IR.Instructions
{
	public class RandomInsn(ValueRef min, ValueRef max) : Instruction([min, max])
	{
		public override string Name => "random";
		public override NBTType?[] ArgTypes => [NBTType.Int, NBTType.Int];
		public override TypeSpecifier ReturnType => PrimitiveType.Int;

		public override void Render(RenderContext ctx) =>
			ctx.Macroize([Arg<ValueRef>(0), Arg<ValueRef>(1)], (args, ctx) =>
			{
				ctx.Add(ReturnValue.Expect<LValue>().StoreExecute()
					.Run(new RawCommand($"random value {args[0].Value}..{args[1].Value}")));
			});

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}