using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class BranchInsn(ValueRef cond, Block ifTrue, Block ifFalse) : Instruction([cond, ifTrue, ifFalse])
	{
		public override string Name => "br";
		public override NBTType?[] ArgTypes => [null, null, null];
		public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

		public override void Render(RenderContext ctx)
		{
			var cond = Arg<ValueRef>(0).Expect();
			var ifTrue = Arg<Block>(1);
			var ifFalse = Arg<Block>(2);

			var returning = FunctionContext.GetIsFunctionReturningValue();

			ctx.Add(cond.If(new(), ctx).Run(ctx.CallSubFunction(ifTrue.Function)));
			ctx.Add(new Execute().Unless.Data(returning.Storage, returning.Path).Run(ctx.CallSubFunction(ifFalse.Function)));
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
