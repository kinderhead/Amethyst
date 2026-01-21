using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class BranchInsn(ExecuteChain cond, Block ifTrue, Block ifFalse) : Instruction([cond, ifTrue, ifFalse]), IBranchInsn
	{
		public override string Name => "br";
		public override NBTType?[] ArgTypes => [null, null, null];
		public override TypeSpecifier ReturnType => new VoidType();

		public Block[] Destinations => [Arg<Block>(1), Arg<Block>(2)];

		public override void Render(RenderContext ctx)
		{
			var cond = Arg<ExecuteChain>(0);
			var ifTrue = Arg<Block>(1);
			var ifFalse = Arg<Block>(2);

			cond.RunWithPropagate(macros => ctx.JumpTo(ifTrue, macros), ctx);

			var returning = ctx.Func.GetIsFunctionReturningValue();
			
			foreach (var i in ctx.JumpTo(ifFalse))
			{
				ctx.Add(new Execute().Unless.Data(returning.Storage, returning.Path).Run(i));
			}
			
		}

		public override void OnAdd(Block block)
        {
			var ifTrue = Arg<Block>(1);
			var ifFalse = Arg<Block>(2);

			block.LinkNext(ifTrue);
			block.LinkNext(ifFalse);
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
