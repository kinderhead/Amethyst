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
			ctx.Add(ctx.JumpTo(block));
		}

		public override void OnAdd(Block block)
        {
			var next = Arg<Block>(0);
			block.LinkNext(next);
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
