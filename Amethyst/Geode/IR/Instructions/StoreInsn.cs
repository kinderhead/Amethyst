using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class StoreInsn(ValueRef dest, ValueRef src, bool processArgs = true) : Instruction([dest, src])
	{
		public override string Name => "store";
		public override TypeSpecifier ReturnType => new VoidTypeSpecifier();
		public override NBTType?[] ArgTypes => [null, null];
		public override bool ShouldProcessArgs => processArgs;

		public override void Render(RenderContext ctx) => Arg<ValueRef>(0).Expect<LValue>().Store(Arg<ValueRef>(1).Expect(), ctx);

		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
