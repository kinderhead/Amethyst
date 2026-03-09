using Amethyst.GC;
using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Statements
{
	public class GCMarkStatement(LocationRange loc) : Statement(loc)
	{
		public override void Compile(FunctionContext ctx)
		{
			var self = ctx.GetVariable("this");

			if (!self.IsTypeOrRef<StructType>(out var cls))
			{
				throw new InvalidTypeError(self.Type.ToString(), "object");
			}

			foreach (var i in cls.Properties)
			{
				GCHelper.Mark(ReferenceType.TryDeref(ctx.GetProperty(new(self), i.Key), ctx), ctx);
			}
		}
	}
}
