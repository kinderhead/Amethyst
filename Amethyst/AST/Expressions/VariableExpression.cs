using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var val = ctx.GetVariable(Name);

			if (ctx.InForkingExecute && val is Variable v)
			{
				v.ForceStack = true;
			}

			return new(val);
		}
	}
}
