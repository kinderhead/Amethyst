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
		public override TypeSpecifier ReturnType => PrimitiveType.Int;
		public override bool AlwaysUseScore => true;

		public override void Render(RenderContext ctx)
		{
			var arg = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>(); // Make sure the return value is a score so that /data get is called

			if (arg.Type is ReferenceType)
			{
				ctx.Macroize([arg], (args, ctx) =>
				{
					ret.Store(new RawDataTargetValue(args[0].Value.ToString(), args[0].Type), ctx);
				});
			}
			else
			{
				ret.Store(arg, ctx);
			}
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			if (Arg<ValueRef>(0).Expect() is IConstantValue c && c.Value is NBTList l)
			{
				return new LiteralValue(l.Count);
			}

			return null;
		}
	}
}
