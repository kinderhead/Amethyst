using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => new(ctx.GetVariable(Name));
	}
}
