using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class LoadInsn(ValueRef val, TypeSpecifier? type = null) : Instruction([val]), ILoadInsn
	{
		public override string Name => "load";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => type ?? PrimitiveType.Int;
		public override bool AlwaysUseScore => true;

		public ValueRef Variable => Arg<ValueRef>(0);

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>();

			if (val.Equals(ret))
			{
				return;
			}

			ret.Store(val, ctx);
		}

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
        {
            tryLink(Arg<ValueRef>(0), ReturnValue);
        }


		protected override IValue? ComputeReturnValue(FunctionContext ctx)
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
