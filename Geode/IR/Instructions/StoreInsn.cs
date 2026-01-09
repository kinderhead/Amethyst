using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class StoreInsn(ValueRef dest, ValueRef src) : Instruction([dest, src]), IStoreInsn
	{
		public override string Name => "store";
		public override TypeSpecifier ReturnType => new VoidType();
		public override NBTType?[] ArgTypes => [null, null];

		public ValueRef Variable => Arg<ValueRef>(0);
		public ValueRef Value => Arg<ValueRef>(1);

		public override void Render(RenderContext ctx) => Arg<ValueRef>(0).Expect<LValue>().Store(Arg<ValueRef>(1).Expect(), ctx);

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();

		public override bool ContainsStoreFor(Variable variable) => Arg<ValueRef>(0).Value == variable;
	}
}
