using Amethyst.IR;
using Amethyst.IR.Types;
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
			var val = ctx.GetLocalVariableOrNull(Name);

			if (val is null && ctx.GetLocalVariableOrNull("this") is { } self && self.Type.HasProperty(Name, true) is not null)
			{
				var property = ctx.GetProperty(new ValueRef(self), Name);

				if (expected is null && property.Type is ReferenceType ptr) property = ctx.ImplicitCast(property, ptr.Inner);

				return property;
			}

			val ??= ctx.GetVariable(Name);
			
			if (ctx.InForkingExecute && val is Variable v)
			{
				v.ForceStack = true;
			}

			return new(val);
		}
	}
}