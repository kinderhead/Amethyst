using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public class AssignmentExpression(LocationRange loc, string name, Expression expr) : Expression(loc)
	{
		public readonly string Name = name;
		public readonly Expression Expression = expr;

		protected override Value _Execute(FunctionContext ctx)
		{
			var val = ctx.GetVariable(Name);
			val.Store(ctx, Expression.Cast(ctx, val.Type));
			return val;
		}
	}
}
