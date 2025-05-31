using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public class CastExpression(LocationRange loc, AbstractTypeSpecifier type, Expression expr) : Expression(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly Expression Expression = expr;

		protected override Value _Execute(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
