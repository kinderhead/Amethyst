using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class WeakReferenceInsn(ValueRef src) : Instruction([src])
	{
		public override string Name => "ref";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new WeakReferenceType(Arg<ValueRef>(0).Type);

		public override void Render(RenderContext ctx) => throw new InvalidOperationException();

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect<DataTargetValue>();
			return WeakReferenceType.From(val);
		}
	}
}
