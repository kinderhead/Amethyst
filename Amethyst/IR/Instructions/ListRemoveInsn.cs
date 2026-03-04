using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Instructions
{
	public class ListRemoveInsn(ValueRef list, ValueRef index) : Instruction([list, index])
	{
		public override string Name => "remove";
		public override NBTType?[] ArgTypes => [null, NBTType.Int];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var list = Arg<ValueRef>(0).AsRef();
			var index = Arg<ValueRef>(1).Expect();

			ctx.Macroize([list, index], (args, ctx) =>
			{
				ctx.Add(new DataCommand.Remove(new RawDataTarget($"{args[0].Value}[{args[1].Value}]")));
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
