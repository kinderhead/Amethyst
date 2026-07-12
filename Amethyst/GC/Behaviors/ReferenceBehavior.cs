using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;

namespace Amethyst.GC.Behaviors
{
	public class ReferenceBehavior : GCBehavior<ReferenceType>
	{
		public override bool CanMark(ReferenceType type) => true;

		public override void Mark(ReferenceType type, ValueRef val, FunctionContext ctx)
		{
			if (val.Value is not NullValue)
			{
				ctx.Call("amethyst:gc/mark", val);
			}
		}
	}
}