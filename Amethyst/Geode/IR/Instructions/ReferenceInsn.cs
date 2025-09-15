using Amethyst.Geode.Types;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Geode.IR.Instructions
{
	public class ReferenceInsn(ValueRef src) : Instruction([src])
	{
		public override string Name => "ref";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new PointerTypeSpecifier(Arg<ValueRef>(0).Type);

		public override void Render(RenderContext ctx)
		{
			throw new NotImplementedException();
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
