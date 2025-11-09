using Datapack.Net.Data;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class ListInsn(TypeSpecifier type, params IEnumerable<ValueRef> vals) : Instruction(vals)
	{
		public override string Name => "list";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => type;

		public override void Render(RenderContext ctx) => ctx.StoreList(ReturnValue.Expect<DataTargetValue>(), [.. Arguments.Cast<ValueRef>()]);

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var list = new NBTList();

			for (var i = 0; i < Arguments.Length; i++)
			{
				// Don't use .Expect here since Value could be null
				if (Arg<ValueRef>(i).Value is not IConstantValue l)
				{
					return null;
				}

				list.Add(l.Value);
			}

			return new LiteralValue(list, ReturnType);
		}
	}
}
