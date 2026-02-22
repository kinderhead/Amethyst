using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class StoreRefInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
	{
		public override string Name => "store_ref";
		public override NBTType?[] ArgTypes => [NBTType.String, null];
		public override TypeSpecifier ReturnType => new VoidType();
		public override bool HasSideEffects => true;

		public override void Render(RenderContext ctx)
		{
			var dest = Arg<ValueRef>(0).Expect();
			var src = Arg<ValueRef>(1).Expect();

			ctx.Macroize([dest], (args, ctx) =>
			{
				new RawDataTargetValue(args[0].Value.ToString(), args[0].Type).Store(src, ctx);
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
