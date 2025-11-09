using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	// SortedDictionary so that the order is consistent and also it makes it look nice :)
	public class CompoundInsn(SortedDictionary<string, ValueRef> vals, TypeSpecifier? type = null) : Instruction(vals.Values)
	{
		public override string Name => "nbt";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => type ?? PrimitiveType.Compound;

		public readonly string[] Keys = [.. vals.Keys];

		public override void Render(RenderContext ctx) => ctx.StoreCompound(ReturnValue.Expect<DataTargetValue>(), new(Keys.Zip(Arguments.Cast<ValueRef>()).Select(i => new KeyValuePair<string, IValueLike>(i.First, i.Second))));

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var nbt = new NBTCompound();

			for (var i = 0; i < Arguments.Length; i++)
			{
				// Don't use .Expect here since Value could be null
				if (Arg<ValueRef>(i).Value is not IConstantValue l)
				{
					return null;
				}

				nbt[Keys[i]] = l.Value;
			}

			return new LiteralValue(nbt, ReturnType);
		}
	}
}
