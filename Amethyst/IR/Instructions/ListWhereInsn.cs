using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Instructions
{
	public class ListWhereInsn(ValueRef list, ValueRef predicate) : Instruction([list, predicate])
	{
		public override string Name => "where";
		public override NBTType?[] ArgTypes => [null, NBTType.Compound];
		public override TypeSpecifier ReturnType => Arg<ValueRef>(0).Type is ReferenceType r ? r.Inner : Arg<ValueRef>(0).Type;

		public override void Render(RenderContext ctx)
		{
			var ret = ReturnValue.Expect<LValue>();
			var list = Arg<ValueRef>(0).Expect();

			if (list.Type is not ReferenceType)
			{
				// Re-check list
				list = WeakReferenceType.From(Arg<ValueRef>(0).Expect<DataTargetValue>());
			}

			// Thanks Minecraft for making it work this way
			ret.Store(new LiteralValue(new NBTList()), ctx);

			ctx.Macroize([list, Arg<ValueRef>(1)], (args, ctx) =>
			{
				ret.ListAdd(new RawDataTargetValue($"{args[0].Value}[{args[1].Value}]", PrimitiveType.List), ctx);
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
