using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.GC.Behaviors
{
	public class ReferenceBehavior : GCBehavior<ReferenceType>
	{
		public override bool CanMark(ReferenceType type) => true;

		public override void Mark(ReferenceType type, ValueRef val, FunctionContext ctx)
		{
			ctx.Call("amethyst:gc/mark", val);
		}
	}
}
