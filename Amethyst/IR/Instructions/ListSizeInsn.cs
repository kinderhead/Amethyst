using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class ListSizeInsn(ValueRef list) : Instruction([list])
	{
		public override string Name => "size";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;

		public override void Render(RenderContext ctx)
		{
			var arg = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>(); // Make sure the return value is a score so that /data get is called

			if (arg.Type is ReferenceTypeSpecifier)
			{
				ctx.Call("amethyst:core/ref/set-score", new LiteralValue($"{ret.Target.Get()} {ret.Score.Name}"), arg);
			}
			else
			{
				ret.Store(arg, ctx);
			}
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (Arg<ValueRef>(0).Expect() is IConstantValue c && c.Value is NBTList l)
			{
				return new LiteralValue(l.Count);
			}

			return null;
		}
	}
}
