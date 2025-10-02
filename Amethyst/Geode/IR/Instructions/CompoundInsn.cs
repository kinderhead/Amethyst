using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Geode.IR.Instructions
{
	// SortedDictionary so that the order is consistent and also it makes it look nice :)
	public class CompoundInsn(SortedDictionary<string, ValueRef> vals) : Instruction(vals.Values)
	{
		public override string Name => "nbt";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Compound;

		public readonly string[] Keys = [.. vals.Keys];

        public override void Render(RenderContext ctx)
		{
            ctx.StoreCompound(ReturnValue.Expect<DataTargetValue>(), new(Keys.Zip(Arguments.Cast<ValueRef>()).Select(i => new KeyValuePair<string, ValueRef>(i.First, i.Second))));
        }

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
            var nbt = new NBTCompound();

            for (int i = 0; i < Arguments.Length; i++)
            {
                if (Arg<ValueRef>(i).Expect() is not IConstantValue l) return null;
                nbt[Keys[i]] = (l.Value);
            }

            return new LiteralValue(nbt, ReturnType);
        }
	}
}
