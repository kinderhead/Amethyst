using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class StoreInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
	{
		public override string Name => "store";
		public override TypeSpecifier ReturnType => new VoidType();
		public override NBTType?[] ArgTypes => [null, null];

		public override void Render(RenderContext ctx) => Arg<ValueRef>(0).Expect<LValue>().Store(Arg<ValueRef>(1).Expect(), ctx);

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
