using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR.Passes;
using System;

namespace Geode.IR.Instructions
{
	public class PhiInsn(Block path1, ValueRef val1, Block path2, ValueRef val2) : Instruction([path1, val1, path2, val2]), IPhiLike
	{
		public override string Name => "phi";
		public override NBTType?[] ArgTypes => [null, null, null, null];
		public override TypeSpecifier ReturnType => Arg<ValueRef>(0).Type != Arg<ValueRef>(1).Type ? throw new InvalidTypeError(Arg<ValueRef>(1).Type.ToString(), Arg<ValueRef>(0).Type.ToString()) : Arg<ValueRef>(0).Type;
		public override bool ArgumentsAliveAtInsn => false;

		public override void Render(RenderContext ctx) { }
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
        {
            var val1 = Arg<ValueRef>(0);
            var val2 = Arg<ValueRef>(1);

            tryLink(ReturnValue, val1);
            tryLink(ReturnValue, val2);
            tryLink(val1, val2);
        }

        public void Process(Block block)
        {
            block.Phi.Map(Arg<Block>(0), Arg<ValueRef>(1), ReturnValue);
            block.Phi.Map(Arg<Block>(2), Arg<ValueRef>(3), ReturnValue);
        }
	}
}
