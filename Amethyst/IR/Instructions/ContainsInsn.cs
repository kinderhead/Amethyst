using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Instructions
{
	public class ContainsInsn(ValueRef val, ValueRef prop) : Instruction([val, prop])
	{
		public override string Name => "contains";
		public override NBTType?[] ArgTypes => [null, NBTType.String];
		public override TypeSpecifier ReturnType => PrimitiveType.Bool;

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			var prop = Arg<ValueRef>(1).Expect();
			var ret = ReturnValue.Expect<LValue>();

			if (val.Type is EntityType)
			{
				ctx.Macroize([val, prop], (args, ctx) =>
				{
					ctx.Add(ret.StoreExecute(false).Run(new RawCommand($"data get entity @e[scores={{amethyst_id={args[0].Value}}}] {args[1].Value}")));
				});

				return;
			}

			if (val.Type is not ReferenceType && val is DataTargetValue nbt)
			{
				if (val is MacroValue)
				{
					throw new MacroPropertyError();
				}

				val = WeakReferenceType.From(nbt);
			}

			ctx.Macroize([val, prop], (args, ctx) =>
			{
				ctx.Add(ret.StoreExecute(false).Run(new RawCommand($"data get {args[0].Value}.{args[1].Value}")));
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
