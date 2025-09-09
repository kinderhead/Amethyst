using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amethyst.AST.Expressions;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Intrinsics
{
	public class Print() : Intrinsic("print")
	{
		public override ValueRef Execute(FunctionContext ctx, params IEnumerable<Expression> args)
		{
			var vals = args.Select(i => i.Execute(ctx));
			return ctx.Add(new PrintInsn(vals));
		}
	}
}
