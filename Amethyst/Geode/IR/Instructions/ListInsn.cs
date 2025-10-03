using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class ListInsn(TypeSpecifier type, params IEnumerable<ValueRef> vals) : Instruction(vals)
	{
		public override string Name => "list";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => type;

		public override void Render(RenderContext ctx) => ctx.StoreList(ReturnValue.Expect<DataTargetValue>(), [.. Arguments.Cast<ValueRef>()]);

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var list = new NBTList();

			for (var i = 0; i < Arguments.Length; i++)
			{
				if (Arg<ValueRef>(i).Expect() is not IConstantValue l)
				{
					return null;
				}

				list.Add(l.Value);
			}

			return new LiteralValue(list, ReturnType);
		}
	}
}
