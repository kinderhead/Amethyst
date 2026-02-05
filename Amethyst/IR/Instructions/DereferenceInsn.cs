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
			var ret = ReturnValue.Expect<LValue>();
			var ptr = Arg<ValueRef>(0).Expect();

			ctx.Macroize([ptr], (args, ctx) =>
			{
				ret.Store(new RawDataTargetValue(args[0].Value.ToString(), args[0].Type), ctx);
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			if (Arg<ValueRef>(0).Expect() is LiteralValue l && l.Is<NBTString>(out var str))
			{
				Remove();
				if (str.Value.Contains("stack[-1]."))
				{
					return new StackValue(-1, ctx.Compiler.IR.RuntimeID, str.Value.Split("stack[-1].")[1], ReturnType);
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
