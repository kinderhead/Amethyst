using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class LoadInsn(ValueRef val, TypeSpecifier? type = null) : Instruction([val])
	{
		public override string Name => "load";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => type ?? PrimitiveType.Int;

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>();

			if (val is ScoreValue score && score == ret)
			{
				return;
			}

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
			else if (val.Value is LiteralValue literal && val.Type is PrimitiveType)
			{
				return literal;
			}

			return null;
		}
	}
}
