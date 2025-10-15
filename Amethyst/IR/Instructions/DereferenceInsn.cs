using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class DereferenceInsn(ValueRef ptr) : Instruction([ptr])
	{
		public override string Name => "deref";
		public override NBTType?[] ArgTypes => [null];

		public override TypeSpecifier ReturnType => ((ReferenceType)ptr.Type).Inner;

		public override void Render(RenderContext ctx)
		{
			var ret = ReturnValue.Expect();
			if (ret is ScoreValue score)
			{
				ctx.Call("amethyst:core/ref/set-score", new LiteralValue($"{score.Target.Get()} {score.Score.Name}"), Arg<ValueRef>(0).Expect());
			}
			else
			{
				ctx.Call("amethyst:core/ref/set-ref", WeakReferenceType.From((DataTargetValue)ret), Arg<ValueRef>(0).Expect());
			}
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (Arg<ValueRef>(0).Expect() is LiteralValue l && l.Value is NBTString str)
			{
				Remove();
				if (str.Value.Contains("stack[-1]."))
				{
					return new StackValue(-1, str.Value.Split("stack[-1].")[1], ReturnType);
				}
				else
				{
					return new RawDataTargetValue(str.Value, ReturnType);
				}
			}

			return null;
		}
	}
}
