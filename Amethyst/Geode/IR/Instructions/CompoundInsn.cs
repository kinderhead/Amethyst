using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	// SortedDictionary so that the order is consistent and also it makes it look nice :)
	public class CompoundInsn(SortedDictionary<string, ValueRef> vals, TypeSpecifier? type = null) : Instruction(vals.Values)
	{
		public override string Name => "nbt";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => type ?? PrimitiveTypeSpecifier.Compound;

		public readonly string[] Keys = [.. vals.Keys];

		public override void Render(RenderContext ctx) => ctx.StoreCompound(ReturnValue.Expect<DataTargetValue>(), new(Keys.Zip(Arguments.Cast<ValueRef>()).Select(i => new KeyValuePair<string, ValueRef>(i.First, i.Second))));

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var nbt = new NBTCompound();

			for (var i = 0; i < Arguments.Length; i++)
			{
				if (Arg<ValueRef>(i).Expect() is not IConstantValue l)
				{
					return null;
				}

				nbt[Keys[i]] = l.Value;
			}

			return new LiteralValue(nbt, ReturnType);
		}
	}
}
