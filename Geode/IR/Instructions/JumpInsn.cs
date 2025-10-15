using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class JumpInsn(Block dest) : Instruction([dest])
	{
		public override string Name => "jump";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var block = Arg<Block>(0);
			ctx.Add(ctx.CallSubFunction(block.Function));
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
