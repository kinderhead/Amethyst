using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Instructions
{
	internal class StoreRefToRefInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
	{
		public override string Name => "store_ref_to_ref";
		public override NBTType?[] ArgTypes => [NBTType.String, NBTType.String];
		public override TypeSpecifier ReturnType => new VoidType();
		public override bool HasSideEffects => true;

		public override void Render(RenderContext ctx)
		{
			var dest = Arg<ValueRef>(0).Expect();
			var src = Arg<ValueRef>(1).Expect();

			ctx.Macroize([dest, src], (args, ctx) =>
			{
				new RawDataTargetValue(args[0].Value.ToString(), args[0].Type).Store(new RawDataTargetValue(args[1].Value.ToString(), args[1].Type), ctx);
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
