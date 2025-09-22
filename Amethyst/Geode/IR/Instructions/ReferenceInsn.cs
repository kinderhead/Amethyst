using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class ReferenceInsn(ValueRef src) : Instruction([src])
	{
		public override string Name => "ref";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new ReferenceTypeSpecifier(Arg<ValueRef>(0).Type);

		public override void Render(RenderContext ctx)
		{
            var val = Arg<ValueRef>(0).Expect<StackValue>();
			ctx.Call("amethyst:core/ref/get-stack-ref", new LiteralValue(val.Location));
            ReturnValue.Expect<LValue>().Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();

			if (val is StackValue) return null;
            else if (val is DataTargetValue data) return ReferenceTypeSpecifier.From(data);

			return null;
		}
	}
}
