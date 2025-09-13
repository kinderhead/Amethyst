using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class CastExpression(LocationRange loc, TypeSpecifier type, Expression expr) : Expression(loc)
	{
		public readonly TypeSpecifier Type = type;
		public readonly Expression Expression = expr;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => Type;

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
