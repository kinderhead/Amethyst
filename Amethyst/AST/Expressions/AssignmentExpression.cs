using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public class AssignmentExpression(LocationRange loc, Expression dest, Expression expr) : Expression(loc)
	{
		public readonly Expression Dest = dest;
		public readonly Expression Expression = expr;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => Dest.ComputeType(ctx);

		protected override Value _Execute(FunctionContext ctx)
		{
			var val = Dest.Execute(ctx);
			if (val is MutableValue v) Expression.Cast(val.Type).Store(ctx, v);
			else throw new ImmutableValueError(Location);
			return val;
		}
	}
}
