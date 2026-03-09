using Amethyst.IR.Instructions;
using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.GC.Behaviors
{
	public class ListBehavior : GCBehavior<ListType>
	{
		public override bool CanMark(ListType type) => GCHelper.CanMark(type.Inner);

		public override void Mark(ListType type, ValueRef val, FunctionContext ctx)
		{
			var i = ctx.RegisterLocal(GeodeBuilder.UniqueString, PrimitiveType.Int, LocationRange.None);
			ctx.Add(new StoreInsn(i, new LiteralValue(0)));
			var size = ctx.Add(new ListSizeInsn(val));

			ctx.Loop(() =>
			{
				var chain = new ExecuteChain();
				chain.Add(new IfScoreChain(ctx.AddLoad(i), ComparisonOperator.Lt, ctx.AddLoad(size)));
				return chain;
			}, "list_mark",
			() =>
			{
				GCHelper.Mark(ctx.Add(new IndexInsn(val, i)), ctx);
			},
			() =>
			{
				var inc = ctx.Add(new AddInsn(ctx.AddLoad(i), new LiteralValue(1)));
				ctx.Add(new StoreInsn(i, inc));
			});
		}
	}
}
