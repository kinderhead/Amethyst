using Datapack.Net.Data;
using Geode.Values;
using System;

namespace Geode.IR.Instructions
{
	public class RangeInsn(ValueRef left, ValueRef right, TypeSpecifier type) : Instruction([left, right])
	{
		public override string Name => "range";
		public override NBTType?[] ArgTypes => [null, null];
		public override TypeSpecifier ReturnType => type;

		public override void Render(RenderContext ctx) { }

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new RangeValue(Arg<ValueRef>(0), Arg<ValueRef>(1), ReturnType);
	}
}
