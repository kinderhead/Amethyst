using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class ReferenceInsn(ValueRef src) : Instruction([src])
	{
		public override string Name => "ref";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new ReferenceType(Arg<ValueRef>(0).Type);

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect<StackValue>();
			ctx.Call("amethyst:core/ref/get-stack-ref", new LiteralValue(val.Location), new LiteralValue(val.Offset));
			ReturnValue.Expect<LValue>().Store(ctx.Func.GetFunctionReturnValue(ReturnType, -1), ctx);
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
