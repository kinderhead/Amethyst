using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class NewExpression(LocationRange loc, AbstractTypeSpecifier type, List<Expression> args) : Expression(loc)
	{
		public readonly List<Expression> Arguments = args;
		public readonly AbstractTypeSpecifier Type = type;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var type = Type.Resolve(ctx);
			var ptr = ctx.Call("amethyst:gc/malloc").SetType(type);

			new CallExpression(Location, new VariableExpression(Location, type.ID.ToString()),
				[new ValueRefExpression(Location, ptr), .. Arguments]).Execute(ctx, null);

			return ptr;
		}
	}
}