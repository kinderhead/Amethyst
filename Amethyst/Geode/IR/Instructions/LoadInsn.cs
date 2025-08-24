using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Geode.IR.Instructions
{
	public class LoadInsn(ValueRef val) : Instruction([val])
	{
		public override string Name => "load";
		public override NBTType?[] ArgTypes => [NBTType.Int];
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			var ret = ReturnValue.Expect<ScoreValue>();

			if (val is ScoreValue score && score == ret) return;
			ret.Store(val, ctx);
		}

		protected override Value? ComputeReturnValue()
		{
			var val = Arg<ValueRef>(0);
			if (val.Value is ScoreValue score) return score;
			return null;
		}
	}
}
