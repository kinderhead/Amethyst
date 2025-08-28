using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class LoadInsn(ValueRef val) : Instruction([val])
	{
		public override string Name => "load";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>();

			if (val is ScoreValue score && score == ret) return;
			ret.Store(val, ctx);
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);
			if (val.Value is ScoreValue score)
			{
				Remove();
				return score;
			}
			return null;
		}
	}
}
