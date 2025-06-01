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
	public class AssignmentExpression(LocationRange loc, string name, Expression expr) : Expression(loc)
	{
		public readonly string Name = name;
		public readonly Expression Expression = expr;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => ctx.GetVariable(Name).Type;

		protected override Value _Execute(FunctionContext ctx)
		{
			var val = ctx.GetVariable(Name);
			if (val is MutableValue v) Expression.Cast(val.Type).Store(ctx, v);
			else throw new ImmutableValueError(Location);
			return val;
		}
	}
}
