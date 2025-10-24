using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions.Utils
{
	public class CountOfInsn(string id) : Instruction([new ValueRef(new LiteralValue(id))])
	{
		public override string Name => "count_of";
		public override NBTType?[] ArgTypes => [NBTType.String];
		public override TypeSpecifier ReturnType => PrimitiveType.Int;

		public override void Render(RenderContext ctx)
		{
			// TODO: do this in ComputeReturnValue so that more optimizations can occur
			var id = ((NBTString)Arg<ValueRef>(0).Expect<LiteralValue>().Value).Value;
			ReturnValue.Expect<LValue>().Store(new LiteralValue(ctx.Builder.Functions.Sum(i => i.Tags.Contains(new(id)) ? 1 : 0)), ctx);
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
