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
			throw new NotImplementedException();
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (Arg<ValueRef>(0).Expect() is DataTargetValue data) return ReferenceTypeSpecifier.From(data);

			return null;
		}
	}
}
