using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class CastExpression(LocationRange loc, TypeSpecifier type, Expression expr) : Expression(loc)
	{
		public readonly TypeSpecifier Type = type;
		public readonly Expression Expression = expr;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			throw new NotImplementedException();
		}
	}
}
