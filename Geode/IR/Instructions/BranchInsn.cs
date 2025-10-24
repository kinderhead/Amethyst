using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class BranchInsn(ExecuteChain cond, Block ifTrue, Block ifFalse) : Instruction([cond, ifTrue, ifFalse])
	{
		public override string Name => "br";
		public override NBTType?[] ArgTypes => [null, null, null];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var cond = Arg<ExecuteChain>(0);
			var ifTrue = Arg<Block>(1);
			var ifFalse = Arg<Block>(2);

			cond.RunWithPropagate(macros => ctx.CallSubFunction(ifTrue.Function, macros), ctx);

			var returning = ctx.Func.GetIsFunctionReturningValue();
			ctx.Add(new Execute().Unless.Data(returning.Storage, returning.Path).Run(ctx.CallSubFunction(ifFalse.Function)));
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
